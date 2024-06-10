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

export interface CatalogState {
    productsLoaded: boolean;
    status: string;
    productParams: ProductParams;
    metaData: MetaData | null;
}

export interface MetaData {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalCount: number;
}

export interface ProductParams {
    orderBy: string;
    searchTerm?: string;
    pageNumber: number;
    pageSize: number;
    isAscending: boolean;
}

const initialState: CatalogState = {
    productsLoaded: false,
    status: 'idle',
    productParams: {
        orderBy: 'name',
        searchTerm: '',
        pageNumber: 1,
        pageSize: 6,
        isAscending: true
    },
    metaData: null
};

const getAxiosParams = (productParams: ProductParams) => {
    const params = new URLSearchParams();
    params.append('pageNumber', productParams.pageNumber.toString());
    params.append('pageSize', productParams.pageSize.toString());
    params.append('sortBy', productParams.orderBy);
    params.append('isAscending', productParams.isAscending.toString());
    if (productParams.searchTerm) params.append('searchTerm', productParams.searchTerm);
    return params;
};

export const fetchProductsAsync = createAsyncThunk<any, void, { state: RootState }>(
    'catalog/fetchProductsAsync',
    async (_, thunkAPI) => {
        const params = getAxiosParams(thunkAPI.getState().catalog.productParams);
        try {
            console.log('Fetching products with params:', params.toString());
            const response = await agent.Catalog.list(params);
            console.log('API response:', response);
            thunkAPI.dispatch(setMetaData({
                currentPage: response.page,
                totalPages: Math.ceil(response.totalItems / response.pageSize),
                pageSize: response.pageSize,
                totalCount: response.totalItems
            }));
            const validProducts = validateProducts(response.items);
            console.log('Validated products:', validProducts);
            return validProducts;
        } catch (error: any) {
            console.error('Error fetching products:', error);
            return thunkAPI.rejectWithValue({ error: error.response.data });
        }
    }
);

export const fetchProductAsync = createAsyncThunk<Product, string>(
    'catalog/fetchProductAsync',
    async (productId, thunkAPI) => {
        try {
            const product = await agent.Catalog.details(productId);
            return product;
        } catch (error: any) {
            return thunkAPI.rejectWithValue({ error: error.response.data });
        }
    }
);

export const catalogSlice = createSlice({
    name: 'catalog',
    initialState: productsAdapter.getInitialState(initialState),
    reducers: {
        setProductParams: (state, action) => {
            state.productsLoaded = false;
            state.productParams = { ...state.productParams, ...action.payload };
        },
        setMetaData: (state, action) => {
            state.metaData = action.payload;
        },
        resetProductParams: (state) => {
            state.productParams = initialState.productParams;
        }
    },
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
            console.error(action.payload);
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
            state.status = 'idle';
        });
    }
});

export const { setProductParams, setMetaData, resetProductParams } = catalogSlice.actions;
export const productSelectors = productsAdapter.getSelectors((state: RootState) => state.catalog);
export default catalogSlice.reducer;
