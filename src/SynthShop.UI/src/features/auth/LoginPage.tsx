import { Container, Paper, Typography, TextField, Button, Box } from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { useAppDispatch, useAppSelector } from '../../app/store/configureStore';
import { loginUser } from './authSlice';
import { useNavigate } from 'react-router-dom';
import 'react-toastify/dist/ReactToastify.css';
import { useState } from 'react';

interface LoginFormInputs {
    email: string;
    password: string;
}

const LoginPage = () => {
    const { control, handleSubmit } = useForm<LoginFormInputs>();
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { status } = useAppSelector(state => state.auth);
    const [error, setError] = useState<string | null>(null);

    const onSubmit = async (data: LoginFormInputs) => {
        setError(null); // Clear previous errors
        const resultAction = await dispatch(loginUser(data));
        if (loginUser.fulfilled.match(resultAction)) {
            navigate('/catalog');
        } else {
            setError(resultAction.payload as string || 'Password or email is incorrect!');
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
            {error && (
                <Box mt={2}>
                    <Typography color="error">{error}</Typography>
                </Box>
            )}
        </Container>
    );
};

export default LoginPage;
