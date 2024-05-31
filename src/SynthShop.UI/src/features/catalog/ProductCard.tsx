import { Avatar, Card, CardActions, CardContent, CardMedia, Typography, CardHeader, Button } from "@mui/material";
import { Product } from "../../app/models/product";
import { Link } from "react-router-dom";
import { useState } from "react";
import agent from "../../app/api/agent";
import { LoadingButton } from "@mui/lab";
import Cookies from "js-cookie";
import { useStoreContext } from "../../app/context/StoreContext";

interface Props {
    product: Product;
}

export default function ProductCard({ product }: Props) {
    const [loading, setLoading] = useState(false);
    const { setBasket } = useStoreContext();

    async function handleAddItem(productId: string) {
        setLoading(true);
        try {
            let basketId = Cookies.get('basketId');

            if (!basketId) {
                // Create a new basket if it does not exist
                const newBasketId = await agent.Basket.create();
                Cookies.set('basketId', newBasketId);
                basketId = newBasketId;
            }

            if (basketId) {
                const basket = await agent.Basket.addItem(basketId, productId, 1);
                console.log('Basket updated:', basket);
                setBasket(basket);
            }
        } catch (error) {
            console.error('Error adding item to basket:', error);
        } finally {
            setLoading(false);
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
                    size="small">Add to cart</LoadingButton>
                <Button component={Link} to={`/catalog/${product.productID}`} size="small">View</Button>
            </CardActions>
        </Card>
    );
}
