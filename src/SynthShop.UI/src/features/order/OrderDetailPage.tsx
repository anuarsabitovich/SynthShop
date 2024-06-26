import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchOrderDetails, cancelOrder, completeOrder } from './orderSlice';
import { RootState } from '../../app/store/configureStore';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Button } from '@mui/material';
import { getOrderStatusString } from '../../app/utils/orderStatus';
import agent from '../../app/api/agent';

const OrderDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { order, status, error } = useSelector((state: RootState) => state.orders);
    const [productNames, setProductNames] = useState<{ [key: string]: string }>({});

    useEffect(() => {
        if (id) {
            dispatch(fetchOrderDetails(id));
        }
    }, [id, dispatch]);

    useEffect(() => {
        if (order) {
            const fetchProductNames = async () => {
                const names: { [key: string]: string } = {};
                for (const item of order.orderItems) {
                    try {
                        const product = await agent.Catalog.details(item.productID);
                        names[item.productID] = product.name;
                    } catch (error) {
                        console.error(`Failed to fetch product name for ${item.productID}`, error);
                        names[item.productID] = 'Unknown Product';
                    }
                }
                setProductNames(names);
            };
            fetchProductNames();
        }
    }, [order]);

    const handleCancelOrder = () => {
        if (id) {
            dispatch(cancelOrder(id)).then(() => navigate('/orders'));
        }
    };

    const handleCompleteOrder = () => {
        if (id) {
            dispatch(completeOrder(id)).then(() => navigate('/orders'));
        }
    };

    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                Order Details
            </Typography>
            {status === 'loading' && <Typography>Loading...</Typography>}
            {status === 'failed' && <Typography color="error">{error}</Typography>}
            {status === 'succeeded' && order && (
                <>
                    <TableContainer component={Paper}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Order ID</TableCell>
                                    <TableCell>Date</TableCell>
                                    <TableCell>Status</TableCell>
                                    <TableCell>Total Amount</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                <TableRow>
                                    <TableCell>{order.orderID}</TableCell>
                                    <TableCell>{new Date(order.orderDate).toLocaleDateString()}</TableCell>
                                    <TableCell>{getOrderStatusString(order.status)}</TableCell>
                                    <TableCell>{order.totalAmount}</TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </TableContainer>
                    <Typography variant="h5" gutterBottom>
                        Order Items
                    </Typography>
                    <TableContainer component={Paper}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Product</TableCell>
                                    <TableCell>Quantity</TableCell>
                                    <TableCell>Price</TableCell>
                                    <TableCell>Subtotal</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {order.orderItems.map((item) => (
                                    <TableRow key={item.orderItemID}>
                                        <TableCell>{productNames[item.productID] || 'Loading...'}</TableCell>
                                        <TableCell>{item.quantity}</TableCell>
                                        <TableCell>{item.price}</TableCell>
                                        <TableCell>{item.price * item.quantity}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                    {order.status !== 1 && ( // Check if the order is not completed
                        <>
                            <Button variant="contained" color="secondary" onClick={handleCancelOrder} sx={{ mt: 2 }}>
                                Cancel Order
                            </Button>
                            <Button variant="contained" color="primary" onClick={handleCompleteOrder} sx={{ mt: 2, ml: 2 }}>
                                Complete Order
                            </Button>
                        </>
                    )}
                </>
            )}
        </Container>
    );
};

export default OrderDetailsPage;
