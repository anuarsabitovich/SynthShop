import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import agent from '../../app/api/agent';
import { User } from '../../app/models/user';
import { AuthResponse } from '../../app/models/authResponse';
import Cookies from 'js-cookie';

interface AuthState {
    username: string | null;
    token: string | null;
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
}

const initialState: AuthState = {
    username: Cookies.get('username') || null,
    token: Cookies.get('token') || null,
    status: 'idle',
    error: null,
};

export const loginUser = createAsyncThunk<AuthResponse, { email: string; password: string }>(
    'auth/loginUser',
    async (credentials, thunkAPI) => {
        try {
            const response = await agent.Auth.login(credentials.email, credentials.password);
            Cookies.set('token', response.token, { expires: 1 }); // Expires in 1 day
            Cookies.set('refreshToken', response.refreshToken, { expires: 1 }); // Expires in 1 day
            Cookies.set('username', credentials.email, { expires: 1 }); // Expires in 1 day
            return { ...response, username: credentials.email };
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data);
        }
    }
);

export const registerUser = createAsyncThunk<AuthResponse, { email: string; password: string; firstName: string; lastName: string; address: string }>(
    'auth/registerUser',
    async (registrationData, thunkAPI) => {
        try {
            const response = await agent.Auth.register(
                registrationData.email,
                registrationData.firstName,
                registrationData.lastName,
                registrationData.address,
                registrationData.password
            );
            Cookies.set('token', response.token, { expires: 1 }); // Expires in 1 day
            Cookies.set('refreshToken', response.refreshToken, { expires: 1 }); // Expires in 1 day
            Cookies.set('username', registrationData.email, { expires: 1 }); // Expires in 1 day
            return { ...response, username: registrationData.email };
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data);
        }
    }
);

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        logout: (state) => {
            state.username = null;
            state.token = null;
            Cookies.remove('username');
            Cookies.remove('token');
            Cookies.remove('refreshToken');
        },
    },
    extraReducers: (builder) => {
        builder.addCase(loginUser.pending, (state) => {
            state.status = 'loading';
        });
        builder.addCase(loginUser.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.username = action.payload.username;
            state.token = action.payload.token;
        });
        builder.addCase(loginUser.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.error.message ?? 'Login failed';
        });
        builder.addCase(registerUser.pending, (state) => {
            state.status = 'loading';
        });
        builder.addCase(registerUser.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.username = action.payload.username;
            state.token = action.payload.token;
        });
        builder.addCase(registerUser.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.error.message ?? 'Registration failed';
        });
    },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;
