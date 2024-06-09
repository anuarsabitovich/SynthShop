import { configureStore } from '@reduxjs/toolkit';
import { counterSlice } from '../../features/contact/counterSlice';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import basketReducer from '../../features/basket/basketSlice';
import catalogReducer from '../../features/catalog/catalogSlice';

export const store = configureStore({
    reducer: {
        counter: counterSlice.reducer,
        basket: basketReducer,
        catalog: catalogReducer
    }
})

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
