import React, { useState } from 'react';
import { Paper, TextField, Button, FormControl, InputLabel, Select, MenuItem, Typography } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { setProductParams, fetchProductsAsync } from './catalogSlice';
import { useNavigate } from 'react-router-dom';

const SearchAndFilters = () => {
    const dispatch = useAppDispatch();
    const { productParams } = useAppSelector(state => state.catalog);
    const [searchTerm, setSearchTerm] = useState(productParams.searchTerm || '');
    const [selectedCategoryId, setSelectedCategoryId] = useState(productParams.categoryId || '');
    const navigate = useNavigate();

    const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(event.target.value);
    };

    const handleSearch = () => {
        dispatch(setProductParams({ searchTerm }));
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
        setSelectedCategoryId(selectedCategoryId);
        navigate(`?categoryId=${selectedCategoryId}`);
        dispatch(setProductParams({ categoryId: selectedCategoryId }));
        dispatch(fetchProductsAsync());
    };

    return (
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
                    value={selectedCategoryId}
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
    );
};

export default SearchAndFilters;
