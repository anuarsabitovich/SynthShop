export interface User {
    id: string;
    email: string;
    firstName?: string;
    lastName?: string;
    address?: string;
    createdAt?: Date;
    updateAt?: Date;
    role: string;
}
