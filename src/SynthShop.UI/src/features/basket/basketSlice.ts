import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Basket, BasketItem } from "../../app/models/basket";
import agent from "../../app/api/agent";

interface BasketState {
    basket: Basket | null;
    status: string;
}

const initialState: BasketState = {
    basket: null,
    status: 'idle'
};

// export const addBasketItemAsync = createAsyncThunk<Basket, {productId: string, quantity: number }>(
//     'basket/addBasketItemAsync',
//     async ({productId, quantity}) => {
//         try {
//             return await agent.Basket.addItem(basketid)
//         } catch (error) {
//             console.log(error)
//         }
//     }
// )

const basketSlice = createSlice({
    name: 'basket',
    initialState,
    reducers: {
        setBasket: (state, action: PayloadAction<Basket>) => {
            state.basket = action.payload;
        },
        addItem: (state, action: PayloadAction<BasketItem>) => {
            if (state.basket) {
                const itemIndex = state.basket.items.findIndex(i => i.basketItemId === action.payload.basketItemId);
                if (itemIndex !== -1) {
                    state.basket.items[itemIndex].quantity += action.payload.quantity;
                } else {
                    state.basket.items.push(action.payload);
                }
            }
        },
        removeItem: (state, action: PayloadAction<{ basketItemId: string; quantity: number }>) => {
            const { basketItemId, quantity } = action.payload;
            if (state.basket) {
                const existingItem = state.basket.items.find(i => i.basketItemId === basketItemId);
                if (existingItem) {
                    existingItem.quantity -= quantity;
                    if (existingItem.quantity <= 0) {
                        state.basket.items = state.basket.items.filter(i => i.basketItemId !== basketItemId);
                    }
                }
            }
        }
    }
});

export const { setBasket, addItem, removeItem } = basketSlice.actions;

export default basketSlice.reducer;
