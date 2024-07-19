import React, { useEffect } from 'react';
import { Grid, Box, Pagination, FormControl, InputLabel, MenuItem, Select, SelectChangeEvent } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { fetchProductsAsync, productSelectors, setProductParams } from './catalogSlice';
import ProductCard from './ProductCard';
import { useLocation } from 'react-router-dom';
import SearchAndFilters from './SearchAndFilters';

function useQuery() {
    return new URLSearchParams(useLocation().search);
}

const Catalog = () => {
    const dispatch = useAppDispatch();
    const products = useAppSelector(productSelectors.selectAll);
    const { productsLoaded, metaData,  productParams } = useAppSelector(state => state.catalog);
    const query = useQuery();
    const categoryId = query.get('categoryId');

    useEffect(() => {
        if (categoryId && categoryId !== productParams.categoryId) {
            dispatch(setProductParams({ categoryId }));
        }
        if (!productsLoaded) {
            dispatch(fetchProductsAsync());
        }
    }, [productsLoaded, categoryId, productParams.categoryId, dispatch]);

    const handlePageChange = (_: React.ChangeEvent<unknown>, page: number) => {
        dispatch(setProductParams({ pageNumber: page }));
        dispatch(fetchProductsAsync());
    };

    const handlePageSizeChange = (event: SelectChangeEvent<number>) => {
        dispatch(setProductParams({ pageSize: event.target.value as number }));
        dispatch(fetchProductsAsync());
    };

    return (
        <Grid container spacing={2}>
            <Grid item xs={12} sm={3}>
                <SearchAndFilters />
            </Grid>
            <Grid item xs={12} sm={9}>
                <Grid container spacing={2}>
                    {products.map(product => (
                        <Grid item xs={12} sm={6} md={4} key={product.productID}>
                            <ProductCard product={product} />
                        </Grid>
                    ))}
                </Grid>
                <Box display="flex" justifyContent="center" mt={4}>
                    <Pagination
                        count={metaData?.totalPages}
                        page={productParams.pageNumber}
                        onChange={handlePageChange}
                    />
                </Box>
                <Box display="flex" justifyContent="center" mt={4}>
                    <FormControl variant="outlined" sx={{ minWidth: 120 }}>
                        <InputLabel sx={{ fontSize: '1rem' }}>Page Size</InputLabel>
                        <Select
                            label="Page Size"
                            value={productParams.pageSize}
                            onChange={handlePageSizeChange}
                        >
                            {[6, 12, 24].map(size => (
                                <MenuItem key={size} value={size}>
                                    {size}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Box>
            </Grid>
        </Grid>
    );
};

export default Catalog;
