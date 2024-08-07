
export interface OrderItem {
    orderItemID: string;
    productID: string;
    quantity: number;
    price: number;
}

export interface Order {
    orderID: string;
    orderDate: Date;
    userId: string;
    isDeleted: boolean;
    totalAmount: number;
    status: number;
    createdAt: Date;
    updateAt?: Date;
    orderItems: OrderItem[];
}
