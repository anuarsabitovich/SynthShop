import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Basket } from "../../app/models/basket";
import agent from "../../app/api/agent";

interface BasketState {
    basket: Basket | null;
    addItemStatus: string;
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    removeSingleItemStatus: string;
    removeAllItemsStatus: string;
    error: string | null;
}

const initialState: BasketState = {
    basket: null,
    addItemStatus: 'idle',
    removeSingleItemStatus: 'idle',
    removeAllItemsStatus: 'idle',
    status: 'idle',
    error: null,
};

export const fetchBasketById = createAsyncThunk<Basket, string, { rejectValue: string }>(
    'basket/fetchBasketById',
    async (basketId, thunkAPI) => {
        try {
            const response = await agent.Basket.getById(basketId);
            return response;
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data.message || 'Failed to fetch basket');
        }
    }
);


export const initializeBasket = createAsyncThunk<Basket, void, { rejectValue: string }>(
    'basket/initializeBasket',
    async (_, thunkAPI) => {
        try {
            let basketId = localStorage.getItem('basketId');
            if (!basketId) {
                const newBasketId = await agent.Basket.create();
                localStorage.setItem('basketId', newBasketId);
                basketId = newBasketId;
            }
            const basket = await agent.Basket.getById(basketId!);
            return basket;
        } catch (error) {
            console.error('Error initializing basket:', error);
            return thunkAPI.rejectWithValue('Failed to initialize basket');
        }
    }
);

export const addBasketItemAsync = createAsyncThunk<Basket, { basketId: string, productId: string, quantity?: number }>(
    'basket/addBasketItemAsync',
    async ({ basketId, productId, quantity = 1 }) => {
        try {
            const response = await agent.Basket.addItem(basketId, productId, quantity);
            console.log('API response:', response);
            return response;
        } catch (error) {
            console.log(error);
            throw error;
        }
    }
);

export const removeBasketItemAsync = createAsyncThunk<void, { basketItemId: string, quantity: number }>(
    'basket/removeBasketItemAsync',
    async ({ basketItemId }) => {
        try {
            await agent.Basket.removeItem(basketItemId);
        } catch (error) {
            console.log(error);
            throw error;
        }
    }
);

const basketSlice = createSlice({
    name: 'basket',
    initialState,
    reducers: {
        setBasket: (state, action: PayloadAction<Basket>) => {
            state.basket = action.payload;
        }, 
        clearBasket: (state) => {
            state.basket = null;
        }
    },
    extraReducers: (builder) => {
         builder.addCase(initializeBasket.fulfilled, (state, action) => {
            state.basket = action.payload;
        });
        builder.addCase(initializeBasket.rejected, (state, action) => {
            console.error(action.payload);
            console.error(state);
        });
        builder.addCase(addBasketItemAsync.pending, (state, action) => {
            state.addItemStatus = 'pendingAddItem' + action.meta.arg.productId;
        });
        builder.addCase(addBasketItemAsync.fulfilled, (state, action) => {
            console.log('Fulfilled action:', action);
            state.basket = action.payload; 
            state.addItemStatus = 'idle';
        });
        builder.addCase(addBasketItemAsync.rejected, (state) => {
            state.addItemStatus = 'idle';
        });

        builder.addCase(removeBasketItemAsync.pending, (state, action) => {
            const { quantity } = action.meta.arg;
            if (quantity === 1) {
                state.removeSingleItemStatus = 'pendingRemoveItem' + action.meta.arg.basketItemId;
            } else {
                state.removeAllItemsStatus = 'pendingRemoveItem' + action.meta.arg.basketItemId;
            }
        });
        builder.addCase(removeBasketItemAsync.fulfilled, (state, action) => {
            const { basketItemId, quantity } = action.meta.arg;
            if (state.basket) {
                const existingItem = state.basket.items.find(i => i.basketItemId === basketItemId);
                if (existingItem) {
                    existingItem.quantity -= quantity;
                    if (existingItem.quantity <= 0) {
                        state.basket.items = state.basket.items.filter(i => i.basketItemId !== basketItemId);
                    }
                }
            }
            if (quantity === 1) {
                state.removeSingleItemStatus = 'idle';
            } else {
                state.removeAllItemsStatus = 'idle';
            }
        });
        builder.addCase(removeBasketItemAsync.rejected, (state, action) => {
            const { quantity } = action.meta.arg;
            if (quantity === 1) {
                state.removeSingleItemStatus = 'idle';
            } else {
                state.removeAllItemsStatus = 'idle';
            }
        });
        builder.addCase(fetchBasketById.pending, (state) => {
            state.status = 'loading';
        });
        builder.addCase(fetchBasketById.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.basket = action.payload;
        });
        builder.addCase(fetchBasketById.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.error.message || 'Failed to fetch basket';
        });
    }
});

export const { setBasket, clearBasket } = basketSlice.actions;

export default basketSlice.reducer;
