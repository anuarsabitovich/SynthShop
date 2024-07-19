import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import agent from '../../app/api/agent';
import { User } from '../../app/models/user';
import { DecodedToken } from '../../app/models/decodedToken';
import { AuthResponse } from '../../app/models/authResponse';
import { jwtDecode } from 'jwt-decode';
import { generateCorrelationId } from '../../app/utils/correlationIdGenerator';
import { fetchBasketById } from '../basket/basketSlice';


interface AuthState {
    user: User | null;
    token: string | null;
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
}

const initialState: AuthState = {
    user: localStorage.getItem('user') ? JSON.parse(decodeURIComponent(localStorage.getItem('user')!)) : null,
    token: localStorage.getItem('token') || null,
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
                createdAt: new Date(decodedToken.exp! * 1000).toISOString(), 
                updateAt: new Date(decodedToken.exp! * 1000).toISOString() 
            };
            localStorage.setItem('token', response.token)
            localStorage.setItem('refreshToken', response.refreshToken)
            localStorage.setItem('user', encodeURIComponent(JSON.stringify(user)))
            localStorage.setItem('correlationId', generateCorrelationId())
            try {
                const lastBasket = await agent.Basket.getLastBasketByCustomerId(user.id);
                console.log("Retrieved last basket:", lastBasket);
                if (lastBasket && lastBasket.basketId) {
                    localStorage.removeItem('basketId');
                    localStorage.setItem('basketId', lastBasket.basketId);
                    thunkAPI.dispatch(fetchBasketById(lastBasket.basketId));
                }
            } catch (error) {
                const localBasketId = localStorage.getItem('basketId');
                if (localBasketId) {
                    await agent.Basket.updateCustomer(localBasketId, user.id);
                }
            }
            
            return { ...response, user };
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data.message || 'Login failed');
        }
    }
);

export const registerUser = createAsyncThunk<{ user: any; token: string }, { email: string; password: string; firstName: string; lastName: string; address: string }, { rejectValue: string }>(
    'auth/registerUser',
    async (registrationData, thunkAPI) => {
        try {
            // Register the user
            await agent.Auth.register(
                registrationData.email,
                registrationData.firstName,
                registrationData.lastName,
                registrationData.address,
                registrationData.password
            );

            const loginResponse = await thunkAPI.dispatch(loginUser({ email: registrationData.email, password: registrationData.password })).unwrap();

            return loginResponse as { user: any; token: string };
        } catch (error: any) {
            if (error.response && error.response.data) {
                return thunkAPI.rejectWithValue(error.response.data);
            } else {
                return thunkAPI.rejectWithValue(error.message);
            }
        }
    }
);

export const refreshToken = createAsyncThunk<AuthResponse, void, { rejectValue: string }>(
    'auth/refreshToken',
    async (_, thunkAPI) => {
        try {
            const token = localStorage.getItem('token');
            const refreshToken = localStorage.getItem('refreshToken');
            if (!token || !refreshToken) {
                throw new Error('No token or refresh token found');
            }

            const response = await agent.Auth.refreshToken(token, refreshToken);
            const decodedToken: DecodedToken = jwtDecode(response.token);

            const user: User = {
                id: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
                email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
                role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
                firstName: "",
                lastName: "", 
                address: "", 
                createdAt: new Date(decodedToken.exp! * 1000).toISOString(), 
                updateAt: new Date(decodedToken.exp! * 1000).toISOString() 
            };

            localStorage.setItem('token', response.token);
            localStorage.setItem('refreshToken', response.refreshToken);
            localStorage.setItem('user', encodeURIComponent(JSON.stringify(user)));

            return { ...response, user };
        } catch (error: any) {
            return thunkAPI.rejectWithValue(error.response.data.message || 'Token refresh failed');
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
            localStorage.removeItem('user')
            localStorage.removeItem('token')
            localStorage.removeItem('refreshToken')
            localStorage.removeItem('basketId')
            localStorage.removeItem('correlationId')
            window.location.href = '/';
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
            state.error = action.error.message || 'Login failed';
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
            state.error = action.error.message || 'Registration failed';
        });
        builder.addCase(refreshToken.pending, (state) => {
            state.status = 'loading';
        });
        builder.addCase(refreshToken.fulfilled, (state, action) => {
            state.status = 'succeeded';
            state.user = action.payload.user;
            state.token = action.payload.token;
        });
        builder.addCase(refreshToken.rejected, (state, action) => {
            state.status = 'failed';
            state.error = action.error.message || 'Token refresh failed';
        });
    },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;
