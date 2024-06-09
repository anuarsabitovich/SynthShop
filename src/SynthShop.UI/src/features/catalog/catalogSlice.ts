import { createAsyncThunk, createEntityAdapter, createSlice } from "@reduxjs/toolkit";
import agent from "../../app/api/agent";
import { Product } from "../../app/models/product";
import { RootState } from '../../app/store/configureStore';

const validateProducts = (products: Product[]) => {
    return products.filter(product => product.productID);
};

const productsAdapter = createEntityAdapter<Product>({
    selectId: (product) => product.productID 
});

// Fetch all products async thunk
export const fetchProductsAsync = createAsyncThunk<Product[]>(
    'catalog/fetchProductsAsync',
    async (_, thunkAPI) => {
        try {
            const response = await agent.Catalog.list();
            console.log("Fetched products:", response);
            const validProducts = validateProducts(response.items);
            if (response.items.length !== validProducts.length) {
                console.warn("Some products were invalid and omitted:", response.items.length - validProducts.length);
            }
            return validProducts;
        } catch (error: any) {
            return thunkAPI.rejectWithValue({ error: error.data });
        }
    }
);

// Fetch a single product async thunk
export const fetchProductAsync = createAsyncThunk<Product, string>(
    'catalog/fetchProductAsync',
    async (productId, thunkAPI) => {
        try {
            const product = await agent.Catalog.details(productId);
            console.log("Fetched product:", product);
            if (!product.productID) {
                console.warn("Fetched product is invalid:", product);
                throw new Error("Invalid product data");
            }
            return product;
        } catch (error: any) {
            return thunkAPI.rejectWithValue({ error: error.data });
        }
    }
);

// Create the slice
export const catalogSlice = createSlice({
    name: 'catalog',
    initialState: productsAdapter.getInitialState({
        productsLoaded: false,
        status: 'idle'
    }),
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(fetchProductsAsync.pending, (state) => {
            state.status = 'pendingFetchProducts';
        });
        builder.addCase(fetchProductsAsync.fulfilled, (state, action) => {
            productsAdapter.setAll(state, action.payload);
            state.status = 'idle';
            state.productsLoaded = true;
        });
        builder.addCase(fetchProductsAsync.rejected, (state, action) => {
            console.log(action.payload);
            state.status = 'idle';
        });
        builder.addCase(fetchProductAsync.pending, (state) => {
            state.status = 'pendingFetchProduct';
        });
        builder.addCase(fetchProductAsync.fulfilled, (state, action) => {
            productsAdapter.upsertOne(state, action.payload);
            state.status = 'idle';
        });
        builder.addCase(fetchProductAsync.rejected, (state, action) => {
            console.log(action);
            state.status = 'idle';
        });
    }
});

export const productSelectors = productsAdapter.getSelectors((state: RootState) => state.catalog);
export default catalogSlice.reducer;
