import { Avatar, Card, CardActions, CardContent, CardMedia, Typography, CardHeader, Button } from "@mui/material";
import { Product } from "../../app/models/product";
import { Link } from "react-router-dom";
import { LoadingButton } from "@mui/lab";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { addBasketItemAsync, setBasket } from "../basket/basketSlice";

interface Props {
    product: Product;
}

export default function ProductCard({ product }: Props) {
    const { basket, addItemStatus } = useAppSelector(state => state.basket);
    const dispatch = useAppDispatch();

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
                    loading={addItemStatus.includes('pendingAddItem' + product.productID)}
                    onClick={() => dispatch(addBasketItemAsync({ basketId: basket.basketId, productId: product.productID }))}
                    size="small">Add to cart
                </LoadingButton>
                <Button component={Link} to={`/catalog/${product.productID}`} size="small">View</Button>
            </CardActions>
        </Card>
    );
}
