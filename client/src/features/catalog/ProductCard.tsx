import { Avatar, Button, Card, CardActions, CardContent, CardMedia, Typography, CardHeader } from "@mui/material";
import { Product } from "../../app/models/product";
import { Link } from "react-router-dom";

interface Props {
    product: Product;
}

export default function ProductCard({ product }: Props) {
    return (
        <Card>
            <CardHeader
                avatar= {
                    <Avatar sx={{bgcolor: 'secondary.main'}} >
                        {product.name.charAt(0).toUpperCase()}
                    </Avatar>
                }
                title={product.name}
                titleTypographyProps={{
                    sx: {fontWeight: 'bold', color: 'primary.main'}
                }}
            />
            <CardMedia
                sx={{ height: 140, backgroundSize: 'contain' }}
                image={product.pictureUrl}
                title="green iguana"
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
                <Button size="small">Add to cart</Button>
                <Button component={Link} to={`/catalog/${product.productID}`} size="small">View</Button>
            </CardActions>
        </Card>
    )
}