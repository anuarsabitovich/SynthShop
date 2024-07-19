import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { router } from "../router/Routes";
import store from "../store/configureStore";
import { logout, refreshToken } from "../../features/auth/authSlice";

axios.defaults.baseURL = import.meta.env.VITE_API_BASE_URL;

axios.defaults.withCredentials = true;

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        const correlationId = localStorage.getItem('correlationId');
        if (correlationId) {
            config.headers['x-correlation-id'] = correlationId;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

const handleError = async (error: AxiosError) => {
    console.error("API request error:", error);

    if (!error.response) {
        // Handle network error
        console.error('Network error:', error);
        toast.error('Network error. Please check your connection or server status.');
        return Promise.reject(error);
    }

    const { status, config, data } = error.response as { status: any, config: any, data: any };
         
    if (status === 401) {
        
        try {
            const resultAction = await store.dispatch(refreshToken()).unwrap();
            const newToken = resultAction.token;

            localStorage.setItem('token', newToken);
            axios.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
            config.headers['Authorization'] = `Bearer ${newToken}`;

            return axios(config);
        } catch (refreshError) {
            toast.error('Unauthorized, please log in');
            store.dispatch(logout());
            window.location.href = '/login';
            return Promise.reject(refreshError);
        }
    }

    switch (status) {
        case 400:
            if (data.errors) {
                const modelStateErrors: string[] = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modelStateErrors.push(...data.errors[key]);
                    }
                }
                toast.error(modelStateErrors.flat().join(', ') || 'Bad Request');
            } else if (Array.isArray(data)) {
                const errorMessages = data.map((err: any) => err.description).join(', ');
                toast.error(errorMessages);
            } else {

                toast.error(data.title || 'Bad Request');
            }
            break;
        case 409:
            toast.error(data.message);
            break;
        case 500:
            router.navigate('/server-error', { state: { error: data } });
            break;
        default:
            toast.error('An unexpected error occurred');
            break;
    }

    return Promise.reject(error);
};

axios.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => handleError(error)
);

const logRequest = (url: string, method: string, params: any, body?: any) => {
    console.log(`Making ${method} request to: ${url} with params:`, params, 'and body:', body);
};

const requests = {
    get: (url: string, params?: URLSearchParams) => {
        logRequest(url, 'GET', params);
        return axios.get(url, { params }).then(responseBody)
    },
    post: (url: string, body: {}) => {
        logRequest(url, 'POST', null, body);
        return axios.post(url, body).then(responseBody)
    },
    put: (url: string, body: {}) => {
        logRequest(url, 'PUT', null, body);
        return axios.put(url, body).then(responseBody)
    },
    delete: (url: string) => {
        logRequest(url, 'DELETE', null);
        return axios.delete(url).then(responseBody)
    },
};

const Catalog = {
    list: (params: URLSearchParams) =>
        requests.get("/Product", params),
    details: (ProductID: string) => requests.get(`product/${ProductID}`),
};


const Basket = {
    create: () => requests.post('basket', {}).then(response => response),
    //delete: (basketId: string) => requests.delete(`basket/${basketId}`),
    delete: (basketId: string) => requests.delete(`basket/basket/${basketId}`), // Updated endpoint
    getById: (id: string) => requests.get(`basket/${id}`),
    addItem: async (basketId: string, productId: string, quantity: number = 1) => {
        await requests.post(`basket/${basketId}/items`, { productId, quantity });
        return requests.get(`basket/${basketId}`);
    },
    deleteItem: (itemId: string) => requests.delete(`basket/items/${itemId}`),
    updateItem: (basketId: string, itemId: string, quantity: number) => requests.put(`basket/${basketId}/items/${itemId}`, { quantity }),
    removeItem: (itemId: string) => requests.delete(`basket/items/${itemId}/remove`),
    updateCustomer: (basketId: string, customerId: string) => requests.put(`basket/${basketId}/customer`, { customerId: customerId }).then(responseBody),    getLastBasketByCustomerId: async (customerId: string) => {
        console.log(`Fetching last basket for customer ID: ${customerId}`);
        const response = await requests.get(`basket/last-basket/${customerId}`);
        console.log(`Response from API: ${JSON.stringify(response)}`);
        return response;
    },
};

const Orders = {
    list: () => requests.get('/order/customer-orders'),
    details: (id: string) => requests.get(`/order/${id}`),
    create: (order: { basketId: string }) => requests.post('/order', order),
    cancel: (id: string) => requests.delete(`/order/${id}`),
    complete: (id: string) => requests.post(`/order/complete/${id}`, {}),
};

const Auth = {
    login: (email: string, password: string) => requests.post('/Auth/sign-in', { email, password }),
    register: (email: string, firstName: string, lastName: string, address: string, password: string) =>
        requests.post('/Auth/register', { email, firstName, lastName, address, password }),
    refreshToken: (token: string, refreshToken: string) => requests.post('/Auth/refresh', { token, refreshToken }),
};

const agent = {
    Catalog,
    Basket,
    Auth,
    Orders
};

export { axios };
export default agent;


