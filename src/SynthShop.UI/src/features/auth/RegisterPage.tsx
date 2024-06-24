import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Box, Button, TextField, Typography } from '@mui/material';
import { registerUser } from './authSlice';
import { RootState } from '../../app/store/configureStore';
import { unwrapResult } from '@reduxjs/toolkit';
import { useNavigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';

const RegisterPage = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { status, error } = useSelector((state: RootState) => state.auth);
    const [form, setForm] = useState({
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        address: '',
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm({ ...form, [name]: value });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        dispatch(registerUser(form)).catch((error) => {
                console.error('Registration failed:', error);
            });
    };
    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
            <Typography variant="h4">Register</Typography>
            <TextField
                required
                fullWidth
                label="Email"
                name="email"
                value={form.email}
                onChange={handleChange}
                sx={{ mt: 2 }}
            />
            <TextField
                required
                fullWidth
                label="Password"
                name="password"
                type="password"
                value={form.password}
                onChange={handleChange}
                sx={{ mt: 2 }}
            />
            <TextField
                required
                fullWidth
                label="First Name"
                name="firstName"
                value={form.firstName}
                onChange={handleChange}
                sx={{ mt: 2 }}
            />
            <TextField
                required
                fullWidth
                label="Last Name"
                name="lastName"
                value={form.lastName}
                onChange={handleChange}
                sx={{ mt: 2 }}
            />
            <TextField
                required
                fullWidth
                label="Address"
                name="address"
                value={form.address}
                onChange={handleChange}
                sx={{ mt: 2 }}
            />
            <Button
                type="submit"
                fullWidth
                variant="contained"
                color="primary"
                sx={{ mt: 2 }}
            >
                Register
            </Button>
            {status === 'loading' && <Typography>Loading...</Typography>}
            {status === 'failed' && <Typography color="error">{error}</Typography>}
            <ToastContainer />

        </Box>
    );
};

export default RegisterPage;
