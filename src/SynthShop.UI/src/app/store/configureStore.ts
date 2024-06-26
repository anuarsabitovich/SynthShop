import { configureStore } from '@reduxjs/toolkit';
import { counterSlice } from '../../features/contact/counterSlice';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import basketReducer from '../../features/basket/basketSlice';
import catalogReducer from '../../features/catalog/catalogSlice';
import authReducer from '../../features/auth/authSlice';
import orderReducer from '../../features/order/orderSlice'; 

export const store = configureStore({
    reducer: {
        counter: counterSlice.reducer,
        basket: basketReducer,
        auth: authReducer,
        catalog: catalogReducer,
        orders: orderReducer
    }
})

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

export default store;