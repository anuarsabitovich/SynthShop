import { Box, Grid, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { Add, Delete, Remove } from "@mui/icons-material";
import { useState } from "react";
import agent from "../../app/api/agent";
import { LoadingButton } from "@mui/lab";
import BasketSummary from "./BasketSummary";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { removeItem, setBasket, addItem } from "./basketSlice";

export default function BasketPage() {
    const { basket } = useAppSelector(state => state.basket);
    const dispatch = useAppDispatch();
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
    const handleRemoveItem = (basketItemId: string, name: string) => {
        setStatus({ loading: true, name });
        agent.Basket.removeItem(basketItemId)
            .then(() => {
                dispatch(removeItem({ basketItemId, quantity: 1 })); 
                return agent.Basket.getById(basket.basketId);
            })
            .then(updatedBasket => {
                dispatch(setBasket(updatedBasket));
            })
            .catch(error => console.log(error))
            .finally(() => setStatus({ loading: false, name: '' }));
    };

    const handleDeleteItem = (basketItemId: string, quantity: number, name: string) => {
        setStatus({ loading: true, name });
        agent.Basket.deleteItem(basketItemId)
            .then(() => {
                dispatch(removeItem({ basketItemId, quantity })); 
                return agent.Basket.getById(basket.basketId); 
            })
            .then(updatedBasket => {
                dispatch(setBasket(updatedBasket)); 
            })
            .catch(error => console.log(error))
            .finally(() => setStatus({ loading: false, name: '' }));
    };
    



    if (!basket) return <Typography variant='h3'>Your basket is empty</Typography>;

    return (
        <>
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }}>
                    <TableHead>
                        <TableRow>
                            <TableCell>Product</TableCell>
                            <TableCell align="right">Price</TableCell>
                            <TableCell align="center">Quantity</TableCell>
                            <TableCell align="right">Subtotal</TableCell>
                            <TableCell align="right"></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {basket.items.map(item => (
                            <TableRow key={item?.productId} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                <TableCell component="th" scope="row">
                                    <Box display='flex' alignItems='center'>
                                        <img src={item?.product?.pictureUrl} alt={item?.product?.name} style={{ height: 50, marginRight: 20 }} />
                                        <span>{item?.product?.name}</span>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">{item?.product?.price}</TableCell>
                                <TableCell align="center">
                                    <LoadingButton
                                        loading={status.loading && status.name === 'rem' + item?.basketItemId}
                                        onClick={() => handleRemoveItem(item?.basketItemId, 'rem' + item?.basketItemId)}
                                        color="error"
                                    >
                                        <Remove />
                                    </LoadingButton>
                                    {item?.quantity}
                                    <LoadingButton

                                        loading={status.loading && status.name === 'add' + item?.productId}
                                        onClick={() => handleAddItem(item.basketItemId, 'add' + item.productId, item.productId, item.product.name, item.product.price, item.product.pictureUrl)}
                                        color="secondary"
                                    >
                                        <Add />
                                    </LoadingButton>
                                </TableCell>
                                <TableCell align="right">{item?.product?.price * item?.quantity}</TableCell>
                                <TableCell align="right">
                                    <LoadingButton
                                        loading={status.loading && status.name === 'del' + item?.basketItemId}
                                        onClick={() => handleDeleteItem(item.basketItemId, item.quantity, 'del' + item.basketItemId)}
                                        color='error'
                                    >
                                        <Delete />
                                    </LoadingButton>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <Grid container>
                <Grid item xs={6} />
                <Grid item xs={6}>
                    <BasketSummary />
                </Grid>
            </Grid>
        </>
    );
}
