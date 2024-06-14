import React, { useState, useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { fetchProductsAsync, productSelectors, setProductParams, resetProductParams } from './catalogSlice';
import { Grid, Paper, TextField, Typography, Pagination, Select, MenuItem, FormControl, InputLabel, Button, Box } from '@mui/material';
import ProductCard from './ProductCard';
import { useLocation, useNavigate } from 'react-router-dom';

function useQuery() {
    return new URLSearchParams(useLocation().search);
}

const Catalog = () => {
    const dispatch = useAppDispatch();
    const products = useAppSelector(productSelectors.selectAll);
    const { productsLoaded, metaData, status, productParams } = useAppSelector(state => state.catalog);
    const [searchTerm, setSearchTerm] = useState(productParams.searchTerm || '');
    const query = useQuery();
    const navigate = useNavigate();
    const categoryId = query.get('categoryId');

    useEffect(() => {
        if (categoryId && categoryId !== productParams.categoryId) {
            dispatch(setProductParams({ categoryId }));
        }
        if (!productsLoaded) {
            dispatch(fetchProductsAsync());
        }
    }, [productsLoaded, categoryId, productParams.categoryId, dispatch]);

    useEffect(() => {
        if (productsLoaded) {
            dispatch(fetchProductsAsync());
        }
    }, [productParams, dispatch]);

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

    const handleCategoryFilter = (event: React.ChangeEvent<{ value: unknown }>) => {
        const selectedCategoryId = event.target.value as string;
        navigate(`?categoryId=${selectedCategoryId}`);
        dispatch(setProductParams({ categoryId: selectedCategoryId }));
        dispatch(fetchProductsAsync());
    };

    if (status.includes('pending')) return <div>Loading products...</div>;

    return (
        <Grid container spacing={2}>
            <Grid item xs={12} sm={3}>
                <Paper style={{ padding: '16px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
                    <Typography variant="h6" style={{ marginBottom: '16px' }}>Search & Filters</Typography>
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
                    >
                        Search
                    </Button>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel>Sort By</InputLabel>
                        <Select
                            label="Sort By"
                            value={productParams.orderBy || ''}
                            onChange={handleSortChange}
                        >
                            <MenuItem value="name">Name</MenuItem>
                            <MenuItem value="price">Price</MenuItem>
                        </Select>
                    </FormControl>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel>Order</InputLabel>
                        <Select
                            label="Order"
                            value={productParams.isAscending ? 'ascending' : 'descending'}
                            onChange={handleOrderChange}
                        >
                            <MenuItem value="ascending">Ascending</MenuItem>
                            <MenuItem value="descending">Descending</MenuItem>
                        </Select>
                    </FormControl>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel>Category</InputLabel>
                        <Select
                            label="Category"
                            value={categoryId || ''}
                            onChange={handleCategoryFilter}
                        >
                            <MenuItem value="">All Categories</MenuItem>
                            <MenuItem value="E46616A9-A02F-4C47-91B8-AD7E0EA4C535">DIGITAL</MenuItem>
                            <MenuItem value="C322A30B-7E37-46FC-9CA5-22BC0FB96D6E">ANALOG</MenuItem>
                            <MenuItem value="CEF242C3-AFDC-4C4A-87FE-E71B6AE4C64E">VIRTUAL</MenuItem>
                            <MenuItem value="D6DC84B5-EA5C-4366-8CE3-9E1B3444AFEB">MIDI</MenuItem>
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
                    <FormControl variant="outlined">
                        <InputLabel>Page Size</InputLabel>
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
