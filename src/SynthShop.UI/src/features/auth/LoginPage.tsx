import React from 'react';
import { Container, Paper, Typography, TextField, Button } from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { loginUser } from './authSlice';
import { useNavigate } from 'react-router-dom';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

interface LoginFormInputs {
    email: string;
    password: string;
}

const LoginPage = () => {
    const { control, handleSubmit } = useForm<LoginFormInputs>();
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { status, error } = useAppSelector(state => state.auth);

    const onSubmit = async (data: LoginFormInputs) => {
        const resultAction = await dispatch(loginUser(data));
        if (loginUser.fulfilled.match(resultAction)) {
            navigate('/catalog');
        } else {
            toast.error('Password or email is incorrect!');
        }
    };

    return (
        <Container component={Paper} maxWidth="sm" sx={{ p: 4 }}>
            <Typography variant="h4" component="h1" gutterBottom>
                Login
            </Typography>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Controller
                    name="email"
                    control={control}
                    defaultValue=""
                    render={({ field }) => <TextField {...field} label="Email" fullWidth margin="normal" />}
                />
                <Controller
                    name="password"
                    control={control}
                    defaultValue=""
                    render={({ field }) => <TextField {...field} label="Password" type="password" fullWidth margin="normal" />}
                />
                <Button type="submit" variant="contained" color="primary" fullWidth disabled={status === 'loading'}>
                    {status === 'loading' ? 'Logging in...' : 'Login'}
                </Button>
            </form>
            <ToastContainer position="bottom-right" hideProgressBar />
        </Container>
    );
};

export default LoginPage;
