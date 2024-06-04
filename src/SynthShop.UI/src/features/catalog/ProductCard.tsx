import { Avatar, Card, CardActions, CardContent, CardMedia, Typography, CardHeader, Button } from "@mui/material";
import { Product } from "../../app/models/product";
import { Link } from "react-router-dom";
import { useState } from "react";
import agent from "../../app/api/agent";
import { LoadingButton } from "@mui/lab";
import Cookies from "js-cookie";
import { useAppDispatch } from "../../app/store/configureStore";
import { setBasket } from "../basket/basketSlice";

interface Props {
    product: Product;
}

export default function ProductCard({ product }: Props) {
    const [loading, setLoading] = useState(false);
    const dispatch = useAppDispatch();

    function handleAddItem(productId: string) {
        setLoading(true);
        let basketId = Cookies.get('basketId');

        if (!basketId) {
            agent.Basket?.create()
                .then(newBasketId => {
                    Cookies.set('basketId', newBasketId);
                    basketId = newBasketId;
                    return agent.Basket.addItem(basketId, productId, 1);
                })
                .then(
                    () => {
                        return agent.Basket.getById(basketId);
                    }
                )
                .then(basket => {
                    console.log('Basket updated:', basket);
                    dispatch(setBasket(basket));
                })
                .catch(error => {
                    console.error('Error adding item to basket:', error);
                })
                .finally(() => {
                    setLoading(false);
                });
        } else {
            agent.Basket.addItem(basketId, productId, 1)
                .then(
                    () => {
                        return agent.Basket.getById(basketId);
                    }
                )
                .then(basket => {
                    console.log('Basket updated:', basket);
                    dispatch(setBasket(basket));
                })
                .catch(error => {
                    console.error('Error adding item to basket:', error);
                })
                .finally(() => {
                    setLoading(false);
                });
        }
    }

    return (
        <Card>
            <CardHeader
                avatar={
                    <Avatar sx={{ bgcolor: 'secondary.main' }} >
                        {product.name.charAt(0).toUpperCase()}
                    </Avatar>
                }
                title={product.name}
                titleTypographyProps={{
                    sx: { fontWeight: 'bold', color: 'primary.main' }
                }}
            />
            <CardMedia
                sx={{ height: 140, backgroundSize: 'contain' }}
                image={product.pictureUrl}
                title={product.name}
            />
            <CardContent>
                <Typography gutterBottom color='secondary' variant="h5">
                    {product.price}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    {product.category} / {product.categoryID}
                </Typography>
            </CardContent>
            <CardActions>
                <LoadingButton
                    loading={loading}
                    onClick={() => handleAddItem(product.productID)}
                    size="small">Add to cart
                </LoadingButton>
                <Button component={Link} to={`/catalog/${product.productID}`} size="small">View</Button>
            </CardActions>
        </Card>
    );
}
