import { Box, Grid, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { Add, Delete, Remove } from "@mui/icons-material";
import { LoadingButton } from "@mui/lab";
import BasketSummary from "./BasketSummary";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { addBasketItemAsync, removeBasketItemAsync } from "./basketSlice";

export default function BasketPage() {
    const { basket, addItemStatus, removeSingleItemStatus, removeAllItemsStatus } = useAppSelector(state => state.basket);
    const dispatch = useAppDispatch();

    if (!basket || !basket.items) return <Typography variant='h3'>Your basket is empty</Typography>;

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
                                                fontSize: '1rem', // Adjust font size to match other cells
                                                lineHeight: '1.5' // Adjust line height if needed
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
                                <TableCell align="right">{item?.product?.price * item?.quantity}</TableCell>
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
                </Grid>
            </Grid>
        </>
    );
}
