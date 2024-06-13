export const getOrderStatusString = (status: number): string => {
    switch (status) {
        case 0:
            return 'Pending';
        case 1:
            return 'Completed';
        case 2:
            return 'Cancelled';
        default:
            return 'Unknown';
    }
};
