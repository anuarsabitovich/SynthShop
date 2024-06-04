import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { router } from "../router/Routes";

const sleep = () => new Promise(resolve => setTimeout(resolve, 500));

axios.defaults.baseURL = 'https://localhost:7281/api/';

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.response.use(async response => {
    await sleep();
    return response;
}, (error: AxiosError) => {
    const { data, status } = error.response as AxiosResponse;
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
            toast.error(data.title);
            break;
        case 401:
            toast.error(data.title);
            break;
        case 500:
            router.navigate('/server-error', { state: { error: data } });
            break;
        default:
            break;
    }
    return Promise.reject(error.response);
});

const requests = {
    get: (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body?: object) => axios.post(url, body).then(responseBody),
    put: (url: string, body: object) => axios.put(url, body).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody)
};

const Catalog = {
    list: () => requests.get('product'),
    details: (ProductID: string) => requests.get(`product/${ProductID}`)
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
    addItem: (basketId: string, productId: string, quantity: number = 1) => requests.post(`basket/${basketId}/items`, { productId, quantity }),
    deleteItem: (itemId: string) => requests.delete(`basket/${itemId}`), // Обновленный метод для удаления элемента
    updateItem: (basketId: string, itemId: string, quantity: number) => requests.put(`basket/${basketId}/items/${itemId}`, { quantity }),
    removeItem: (itemId: string) => requests.post(`basket/items/${itemId}/remove`, {})
};

const agent = {
    Catalog,
    TestErrors,
    Basket
};

export default agent;
