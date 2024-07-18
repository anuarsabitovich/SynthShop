import { Box, Button, Grid, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { Add, Delete, Remove } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import BasketSummary from "./BasketSummary";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { addBasketItemAsync, clearBasket, initializeBasket, removeBasketItemAsync } from "./basketSlice";
import { useNavigate } from 'react-router-dom';
import { createOrder } from "../order/orderSlice";
import { refreshToken } from "../auth/authSlice";
import { jwtDecode } from 'jwt-decode';
import { ToastContainer } from "react-toastify";

export default function BasketPage() {
    const { basket, addItemStatus, removeSingleItemStatus, removeAllItemsStatus } = useAppSelector(state => state.basket);
    const { user } = useAppSelector(state => state.auth);
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    
    const handleCreateOrder = async () => {
        if (!user) {
            navigate('/login');
            return;
        }
    
        const token = localStorage.getItem('token');
        const tokenExpiration = new Date(jwtDecode(token).exp * 1000);
    
        if (tokenExpiration <= new Date()) {
            try {
                await dispatch(refreshToken()).unwrap();
            } catch {
                navigate('/login');
                return;
            }
        }
    
        const resultAction = await dispatch(createOrder({ basketId: basket.basketId }));
        if (createOrder.fulfilled.match(resultAction)) {
            dispatch(clearBasket());
            localStorage.removeItem('basketId');
            dispatch(initializeBasket());
            navigate('/orders');
        } else {
            console.log(resultAction.payload);
        }
    };
    

    if (!basket || !basket.items.length) return (
        <Box textAlign="center" mt={5}>
            <Typography variant='h4'>Your basket is empty</Typography>
            <Button variant="contained" color="primary" onClick={() => navigate('/catalog')}>
                Go to Catalog
            </Button>
        </Box>
    );

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
                                    <Box display='flex' alignItems='center' justifyContent='center' sx={{ height: 55 }}>
                                        <LoadingButton
                                            loading={removeSingleItemStatus === 'pendingRemoveItem' + item.basketItemId}
                                            onClick={() => dispatch(removeBasketItemAsync({ basketItemId: item.basketItemId, quantity: 1 }))}
                                            color="error"
                                            sx={{ minWidth: 0, padding: 0 }}
                                        >
                                            <Remove />
                                        </LoadingButton>
                                        <Typography
                                            sx={{
                                                display: 'flex',
                                                alignItems: 'center',
                                                justifyContent: 'center',
                                                textAlign: 'center',
                                                width: '40px',
                                                fontSize: '1rem', 
                                                lineHeight: '1.5' 
                                            }}
                                            color="textPrimary"
                                        >
                                            {item?.quantity}
                                        </Typography>
                                        <LoadingButton
                                            loading={addItemStatus === 'pendingAddItem' + item.productId}
                                            onClick={() => dispatch(addBasketItemAsync({ basketId: basket.basketId, productId: item.productId }))}
                                            color="secondary"
                                            sx={{ minWidth: 0, padding: 0 }}
                                        >
                                            <Add />
                                        </LoadingButton>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">{(item?.product?.price * item?.quantity).toFixed(2)}</TableCell>
                                <TableCell align="right">
                                    <LoadingButton
                                        loading={removeAllItemsStatus === 'pendingRemoveItem' + item.basketItemId}
                                        onClick={() => dispatch(removeBasketItemAsync({ basketItemId: item.basketItemId, quantity: item.quantity }))}
                                        color='error'
                                        sx={{ minWidth: 0, padding: 0 }}
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
                    <Button variant="contained" color="primary" onClick={handleCreateOrder}>
                        Create Order
                    </Button>
                </Grid>
            </Grid>
            <ToastContainer />
        </>
    );
}

