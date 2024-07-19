import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Box, Button, TextField, Typography } from '@mui/material';
import { registerUser } from './authSlice';
import { AppDispatch, RootState } from '../../app/store/configureStore';
import { useNavigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';

const RegisterPage = () => {
    const dispatch = useDispatch<AppDispatch>();
    const navigate = useNavigate();
    const { status } = useSelector((state: RootState) => state.auth);
    const [emailError, setEmailError] = useState<string | null>(null);
    const [passwordError, setPasswordError] = useState<string | null>(null);
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
        validateForm({ ...form, [name]: value });
    };

    const validateEmail = (email: string) => {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    };

    const validatePassword = (password: any) => {
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{9,}$/;
        return regex.test(password);
    };

    const validateForm = (form: any) => {
        setEmailError(validateEmail(form.email) ? null : 'Invalid email address.');
        setPasswordError(validatePassword(form.password) ? null : 'Password must be at least 9 characters long, contain an uppercase letter, a lowercase letter, a number, and a special character (!@#$%^&*).');
    };

    const isFormValid = () => {
        return validateEmail(form.email) && validatePassword(form.password);
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (isFormValid()) {
            dispatch(registerUser(form))
                .unwrap()
                .then(() => {
                    navigate('/');
                });
        } else {
            validateForm(form);
        }
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
                disabled={!isFormValid() || status === 'loading'}
            >
                Register
            </Button>
            {status === 'loading' && <Typography>Loading...</Typography>}
            <ToastContainer position="bottom-right" hideProgressBar />
        </Box>
    );
};

export default RegisterPage;
