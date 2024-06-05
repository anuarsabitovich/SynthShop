import { Box, Divider, Grid, Table, TableBody, TableCell, TableContainer, TableRow, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Product } from "../../app/models/product";
import agent from "../../app/api/agent";
import NotFound from "../../app/errors/NotFound";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import LoadingButton from "@mui/lab/LoadingButton/LoadingButton";
import { addItem, removeItem, setBasket } from "../basket/basketSlice";

type Params = {
    id: string;
};

export default function ProductDetails() {
    const { basket } = useAppSelector(state => state.basket);
    const dispatch = useAppDispatch();
    const { id } = useParams<Params>();
    const [product, setProduct] = useState<Product | null>(null);
    const [loading, setLoading] = useState(true);
    const item = basket?.items.find(i => i.productId === product?.productID)
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        id && agent.Catalog.details(id)
            .then(response => setProduct(response))
            .catch(error => console.log(error.response))
            .finally(() => setLoading(false))

    }, [id]);

    const [status, setStatus] = useState({
        loading: false,
        name: ''
    });

    const handleAddItem = (basketItemId: string, name: string, productId: string, productName: string, price: number, pictureUrl: string) => {
        setStatus({ loading: true, name });
        agent.Basket.addItem(basket.basketId, productId)
            .then(() => {
                dispatch(addItem({
                    basketItemId,
                    quantity: 1,
                    productId,
                    product: {
                        productId,
                        name: productName,
                        price,
                        pictureUrl
                    }
                }));
                return agent.Basket.getById(basket.basketId);
            })
            .then(updatedBasket => {
                dispatch(setBasket(updatedBasket));
            })
            .catch(error => console.log(error))
            .finally(() => setStatus({ loading: false, name: '' }));
    };


    if (loading) return <LoadingComponent message="Loading product..." />;
    if (error) return <Typography>{error}</Typography>;
    if (!product) return <NotFound />

    return (
        <Grid container spacing={6}>
            <Grid item xs={6}>
                <img src={product.pictureUrl} alt={product.name} style={{ width: '100%' }} />
            </Grid>
            <Grid item xs={6}>
                <Typography variant="h3">{product.name}</Typography>
                <Divider />
                <Typography variant="h4" color='secondary' >{product.price}</Typography>
                <TableContainer>
                    <Table>
                        <TableBody>
                            <TableRow>
                                <TableCell>Name</TableCell>
                                <TableCell>{product.name}</TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>Description</TableCell>
                                <TableCell>{product.description}</TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>Quantity in stock</TableCell>
                                <TableCell>{product.stockQuantity}</TableCell>
                            </TableRow>
                        </TableBody>
                    </Table>
                </TableContainer>
                <Grid container spacing={2}>
                    <Grid item xs={6}>
                        <Box
                            sx={{
                                height: '55px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center'
                            }}
                        >
                            <Typography
                                color="secondary"
                                variant="h5"
                                textAlign="center"
                            >
                             {item?.quantity}
                            </Typography>
                        </Box>
                    </Grid>
                    <Grid item xs={6}>
                        <LoadingButton
                            sx={{ height: '55px' }}
                            color={'primary'}
                            size={'large'}
                            variant={'contained'}
                            loading={status.loading && status.name === 'add' + item?.productId}
                            onClick={() => handleAddItem(item.basketItemId, 'add' + item.productId, item.productId, item.product.name, item.product.price, item.product.pictureUrl)}
                            color="secondary"
                        >
                            Add Product
                        </LoadingButton>
                    </Grid>
                </Grid>
            </Grid>
        </Grid>

    )
}

function setError(arg0: string) {
    throw new Error("Function not implemented.");
}
