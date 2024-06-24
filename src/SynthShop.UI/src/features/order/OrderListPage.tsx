import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchOrders } from './orderSlice';
import { RootState } from '../../app/store/configureStore';
import { Container, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import { Link } from 'react-router-dom';
import { getOrderStatusString } from '../../app/utils/orderStatus';

const OrderListPage = () => {
    const dispatch = useDispatch();
    const { orders, status, error } = useSelector((state: RootState) => state.orders);

    useEffect(() => {
        if (status === 'idle') {
            dispatch(fetchOrders());
        }
    }, [ dispatch]);

    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                My Orders
            </Typography>
            {status === 'loading' && <Typography>Loading...</Typography>}
            {status === 'failed' && <Typography color="error">{error}</Typography>}
            {status === 'succeeded' && orders && (
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
                            {orders.map((order) => (
                                order && order.orderID ? (
                                    <TableRow key={order.orderID}>
                                        <TableCell>
                                            <Link to={`/orders/${order.orderID}`} style={{ textDecoration: 'none', color: '#1976d2', fontWeight: 'bold' }}>
                                                {order.orderID}
                                            </Link>
                                        </TableCell>
                                        <TableCell>{new Date(order.orderDate).toLocaleDateString()}</TableCell>
                                        <TableCell>{getOrderStatusString(order.status)}</TableCell>
                                        <TableCell>{order.totalAmount}</TableCell>
                                    </TableRow>
                                ) : null
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Container>
    );
};

export default OrderListPage;
