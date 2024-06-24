import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { router } from "../router/Routes";
import { getCookies } from '../utils/utils';
const sleep = () => new Promise(resolve => setTimeout(resolve, 500));

axios.defaults.baseURL = 'https://localhost:7281/api';
axios.defaults.withCredentials = true;

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.request.use(
    (config) => {
        const token = getCookies('token');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        } else {
            console.warn("No token found in Cookies");
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

axios.interceptors.response.use(
    async (response) => {
        await sleep();
        return response;
    },
    (error: Error) => {
        const { data, status } = error.response as AxiosResponse;
        console.error("API request error:", error);
        switch (status) {
            case 400:
                if (data.errors) {
                    const modelStateErrors: string[] = [];
                    for (const key in data.errors) {
                        if (data.errors[key]) {
                            modelStateErrors.push(data.errors[key]);
                        }
                    }
                    throw modelStateErrors.flat();
                }
                toast.error(data.title || 'Bad Request');
                break;
            case 401:
                toast.error(data.title || 'Unauthorized');
                break;
            case 500:
                router.navigate('/server-error', { state: { error: data } });
                break;
            default:
                toast.error('An unexpected error occurred');
                break;
        }
        return Promise.reject(error.response);
    }
);

const logRequest = (url: string, method: string, params: any, body?: any) => {
    console.log(`Making ${method} request to: ${url} with params:`, params, 'and body:', body);
};

const logError = (error: AxiosError) => {
    console.error('API request error:', error);
    if (error.response) {
        console.error('Response data:', error.response.data);
    }
};

const requests = {
    get: (url: string, params?: URLSearchParams) => {
        logRequest(url, 'GET', params);
        return axios.get(url, { params }).then(responseBody).catch(logError);
    },
    post: (url: string, body: {}) => {
        logRequest(url, 'POST', null, body);
        return axios.post(url, body).then(responseBody).catch(logError);
    },
    put: (url: string, body: {}) => {
        logRequest(url, 'PUT', null, body);
        return axios.put(url, body).then(responseBody).catch(logError);
    },
    delete: (url: string) => {
        logRequest(url, 'DELETE', null);
        return axios.delete(url).then(responseBody).catch(logError);
    },
};

const Catalog = {
    list: (params: URLSearchParams) =>
        requests.get("/Product", params),
    details: (ProductID: string) => requests.get(`product/${ProductID}`),
};

const TestErrors = {
    get400Error: () => requests.get('Buggy/bad-request'),
    get401Error: () => requests.get('Buggy/unauthorised'),
    get404Error: () => requests.get('Buggy/not-found'),
    get500Error: () => requests.get('Buggy/server-error'),
    getValidationError: () => requests.get('Buggy/validation-error')
};

const Basket = {
    create: () => requests.post('basket', {}).then(response => response),
    delete: (basketId: string) => requests.delete(`basket/${basketId}`),
    getById: (id: string) => requests.get(`basket/${id}`),
    addItem: async (basketId: string, productId: string, quantity: number = 1) => {
        await requests.post(`basket/${basketId}/items`, { productId, quantity });
        return requests.get(`basket/${basketId}`);
    },
    deleteItem: (itemId: string) => requests.delete(`basket/${itemId}`),
    updateItem: (basketId: string, itemId: string, quantity: number) => requests.put(`basket/${basketId}/items/${itemId}`, { quantity }),
    removeItem: (itemId: string) => requests.post(`basket/items/${itemId}/remove`, {})
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
};

const agent = {
    Catalog,
    TestErrors,
    Basket,
    Auth,
    Orders
};

export { axios };
export default agent;


