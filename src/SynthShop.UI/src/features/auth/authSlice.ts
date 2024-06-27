import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import agent from '../../app/api/agent';
import { User } from '../../app/models/user';
import { DecodedToken } from '../../app/models/decodedToken';
import { AuthResponse } from '../../app/models/authResponse';
import Cookies from 'js-cookie';
import jwt_decode, { jwtDecode } from 'jwt-decode';


interface AuthState {
    user: User | null;
    token: string | null;
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
}

const initialState: AuthState = {
    user: Cookies.get('user') ? JSON.parse(decodeURIComponent(Cookies.get('user')!)) : null,
    token: Cookies.get('token') || null,
    status: 'idle',
    error: null,
};



export const loginUser = createAsyncThunk<AuthResponse, { email: string; password: string }, {rejectValue: string}>(
    'auth/loginUser',
    async (credentials, thunkAPI) => {
        try {
            const response = await agent.Auth.login(credentials.email, credentials.password);
            const decodedToken: DecodedToken = jwtDecode(response.token);
            console.log(decodedToken)
            const user: User = {
                id: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
                email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
                role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
                firstName: "",
                lastName: "", 
                address: "", 
                createdAt: new Date(decodedToken.exp * 1000).toISOString(), 
                updateAt: new Date(decodedToken.exp * 1000).toISOString() 
            };
            Cookies.set('token', response.token, { expires: 1 }); 
            Cookies.set('refreshToken', response.refreshToken, { expires: 1 }); 
            Cookies.set('user', encodeURIComponent(JSON.stringify(user)), { expires: 1 }); 
            return { ...response, user };
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data.message || 'Login failed');
        }
    }
);

export const registerUser = createAsyncThunk<AuthResponse, { email: string; password: string; firstName: string; lastName: string; address: string }, {rejectValue: string}>(
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
            const decodedToken: DecodedToken = jwtDecode(response.token);

            const user: User = {
                id: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
                email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
                role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
                firstName: registrationData.firstName,
                lastName: registrationData.lastName,
                address: registrationData.address,
            };

            Cookies.set('token', response.token, { expires: 1 }); // Expires in 1 day
            Cookies.set('refreshToken', response.refreshToken, { expires: 1 }); // Expires in 1 day
            Cookies.set('user', encodeURIComponent(JSON.stringify(user)), { expires: 1 }); // Expires in 1 day
            return { ...response, user };
        } catch (error: any) {
            if (error.response && error.response.data) {
                return thunkAPI.rejectWithValue(error.response.data);
            } else {
                return thunkAPI.rejectWithValue({ message: error.message });
            }
        }
    }
);


const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        logout: (state) => {
            state.user = null;
            state.token = null;
            Cookies.remove('user');
            Cookies.remove('token');
            Cookies.remove('refreshToken');
            Cookies.remove('basketId')
        },
    },
    extraReducers: (builder) => {
        builder.addCase(loginUser.pending, (state) => {
            state.status = 'loading';
        });
        builder.addCase(loginUser.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.user = action.payload.user;
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
            state.user = action.payload.user;
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
