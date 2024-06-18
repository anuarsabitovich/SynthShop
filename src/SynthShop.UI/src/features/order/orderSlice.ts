// src/features/order/orderSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import agent from '../../app/api/agent';
import { Order } from '../../app/models/order';

interface OrderState {
    orders: Order[];
    order: Order | null;
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
}

const initialState: OrderState = {
    orders: [],
    order: null,
    status: 'idle',
    error: null,
};

export const fetchOrders = createAsyncThunk<Order[], void, { rejectValue: string }>(
    'orders/fetchOrders',
    async (_, thunkAPI) => {
        try {
            const response = await agent.Orders.list();
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response?.data || 'Failed to fetch orders');
        }
    }
);

export const fetchOrderDetails = createAsyncThunk<Order, string, { rejectValue: string }>(
    'orders/fetchOrderDetails',
    async (id, thunkAPI) => {
        try {
            const response = await agent.Orders.details(id);
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response?.data || 'Failed to fetch order details');
        }
    }
);

export const createOrder = createAsyncThunk<Order, { basketId: string }, { rejectValue: string }>(
    'orders/createOrder',
    async (order, thunkAPI) => {
        try {
            const response = await agent.Orders.create(order);
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response?.data || error.message);
        }
    }
);

export const cancelOrder = createAsyncThunk<Order, string, { rejectValue: string }>(
    'orders/cancelOrder',
    async (id, thunkAPI) => {
        try {
            const response = await agent.Orders.cancel(id);
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response?.data || 'Failed to cancel order');
        }
    }
);

export const completeOrder = createAsyncThunk<Order, string, { rejectValue: string }>(
    'orders/completeOrder',
    async (id, thunkAPI) => {
        try {
            const response = await agent.Orders.complete(id);
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response?.data || 'Failed to complete order');
        }
    }
);

const orderSlice = createSlice({
    name: 'orders',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchOrders.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchOrders.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orders = action.payload;
            })
            .addCase(fetchOrders.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.payload || 'Failed to fetch orders';
            })
            .addCase(fetchOrderDetails.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(fetchOrderDetails.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.order = action.payload;
            })
            .addCase(fetchOrderDetails.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.payload || 'Failed to fetch order details';
            })
            .addCase(createOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(createOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                state.orders.push(action.payload);
            })
            .addCase(createOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.payload || 'Failed to create order';
            })
            .addCase(cancelOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(cancelOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.orders.findIndex(order => order.orderID === action.payload.orderID);
                if (index >= 0) {
                    state.orders[index] = action.payload;
                }
                state.order = action.payload;
            })
            .addCase(cancelOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.payload || 'Failed to cancel order';
            })
            .addCase(completeOrder.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(completeOrder.fulfilled, (state, action) => {
                state.status = 'succeeded';
                const index = state.orders.findIndex(order => order.orderID === action.payload.orderID);
                if (index >= 0) {
                    state.orders[index] = action.payload;
                }
                state.order = action.payload;
            })
            .addCase(completeOrder.rejected, (state, action) => {
                state.status = 'failed';
                state.error = action.payload || 'Failed to complete order';
            })
    },
});

export default orderSlice.reducer;
