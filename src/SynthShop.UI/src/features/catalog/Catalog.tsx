import React, { useState, useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { fetchProductsAsync, productSelectors, setProductParams } from './catalogSlice';
import { Grid, Paper, TextField, Typography, Pagination, Select, MenuItem, FormControl, InputLabel, Button, Box } from '@mui/material';
import ProductCard from './ProductCard';

const Catalog = () => {
    const dispatch = useAppDispatch();
    const products = useAppSelector(productSelectors.selectAll);
    const { productsLoaded, metaData, status, productParams } = useAppSelector(state => state.catalog);
    const [searchTerm, setSearchTerm] = useState(productParams.searchTerm || '');

    useEffect(() => {
        if (!productsLoaded) {
            dispatch(fetchProductsAsync());
        }
    }, [productsLoaded, dispatch]);

    const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
    };

    const handleSearch = () => {
        dispatch(setProductParams({ searchTerm }));
        dispatch(fetchProductsAsync());
    };

    const handlePageChange = (event: React.ChangeEvent<unknown>, page: number) => {
        dispatch(setProductParams({ pageNumber: page }));
        dispatch(fetchProductsAsync());
    };

    const handlePageSizeChange = (event: React.ChangeEvent<{ value: unknown }>) => {
        dispatch(setProductParams({ pageSize: event.target.value as number }));
        dispatch(fetchProductsAsync());
    };

    const handleSortChange = (event: React.ChangeEvent<{ value: unknown }>) => {
        dispatch(setProductParams({ orderBy: event.target.value as string }));
        dispatch(fetchProductsAsync());
    };

    const handleOrderChange = (event: React.ChangeEvent<{ value: unknown }>) => {
        dispatch(setProductParams({ isAscending: event.target.value === 'ascending' }));
        dispatch(fetchProductsAsync());
    };

    if (status.includes('pending')) return <div>Loading products...</div>;

    return (
        <Grid container spacing={2}>
            <Grid item xs={12} sm={3}>
                <Paper style={{ padding: '16px' }}>
                    <Typography variant="h6">Search & Filters</Typography>
                    <TextField
                        fullWidth
                        placeholder="Search products"
                        value={searchTerm}
                        onChange={handleSearchChange}
                    />
                    <Button
                        fullWidth
                        variant="contained"
                        color="primary"
                        onClick={handleSearch}
                        style={{ marginTop: '16px' }}
                    >
                        Search
                    </Button>
                    <FormControl fullWidth style={{ marginTop: '16px' }}>
                        <InputLabel>Sort By</InputLabel>
                        <Select
                            value={productParams.orderBy || ''}
                            onChange={handleSortChange}
                        >
                            <MenuItem value="name">Name</MenuItem>
                            <MenuItem value="price">Price</MenuItem>
                        </Select>
                    </FormControl>
                    <FormControl fullWidth style={{ marginTop: '16px' }}>
                        <InputLabel>Order</InputLabel>
                        <Select
                            value={productParams.isAscending ? 'ascending' : 'descending'}
                            onChange={handleOrderChange}
                        >
                            <MenuItem value="ascending">Ascending</MenuItem>
                            <MenuItem value="descending">Descending</MenuItem>
                        </Select>
                    </FormControl>
                </Paper>
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
                <Box display="flex" justifyContent="center" mt={2}>
                    <FormControl>
                        <InputLabel>Page Size</InputLabel>
                        <Select
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
