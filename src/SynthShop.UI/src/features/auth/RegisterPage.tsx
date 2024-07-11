import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Alert, Box, Button, TextField, Typography } from '@mui/material';
import { loginUser, registerUser } from './authSlice';
import { RootState } from '../../app/store/configureStore';
import { unwrapResult } from '@reduxjs/toolkit';
import { useNavigate } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';

const RegisterPage = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { status } = useSelector((state: RootState) => state.auth);
    const [emailError, setEmailError] = useState<string | null>(null);
    const [passwordError, setPasswordError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);
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
    const validateEmail = (email: string) => {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    };
    const validatePassword = (password: any) => {
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{9,}$/;
        return regex.test(password);
    };
    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateEmail(form.email)) {
            setEmailError('Invalid email address.');
        } else {
            setEmailError(null);
        }

        if (!validatePassword(form.password)) {
            setPasswordError('Password must be at least 9 characters long, contain an uppercase letter, a lowercase letter, a number, and a special character (!@#$%^&*).');
        } else {
            setPasswordError(null);
        }
        if (!emailError && !passwordError) {
            dispatch(registerUser(form))
                .unwrap()
                .then(() => {
                    setSuccess(true);
                    toast.success('Registration successful! Logging you in...');
                    navigate('/');
                })
                .catch((error) => {
                    setSuccess(false);
                    toast.error(`Registration failed: ${error}`);
                    console.error('Registration failed:', error);
                });
        }
    };
    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
            <Typography variant="h4">Register</Typography>
            {success && <Alert severity="success">Registration successful! You can now log in.</Alert>}
            <TextField
                required
                fullWidth
                label="Email"
                name="email"
                value={form.email}
                error={!!emailError}
                helperText={emailError}
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
                error={!!passwordError}
                helperText={passwordError}
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
            <ToastContainer />

        </Box>
    );
};

export default RegisterPage;
