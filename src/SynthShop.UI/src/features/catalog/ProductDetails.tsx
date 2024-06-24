import { Box, Divider, Grid, Table, TableBody, TableCell, TableContainer, TableRow, Typography, Button } from "@mui/material";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import NotFound from "../../app/errors/NotFound";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import LoadingButton from "@mui/lab/LoadingButton/LoadingButton";
import { addBasketItemAsync } from "../basket/basketSlice";
import { fetchProductAsync, productSelectors } from "./catalogSlice";

type Params = {
    id: string;
};

export default function ProductDetails() {
    const { basket, addItemStatus } = useAppSelector(state => state.basket);
    const dispatch = useAppDispatch();
    const { id } = useParams<Params>();
    const product = useAppSelector(state => productSelectors.selectById(state, id!));
    const [error, setError] = useState<string | null>(null);
    const item = basket?.items.find(i => i.productId === product?.productID);
    const { status: productStatus } = useAppSelector(state => state.catalog);

    useEffect(() => {
        if (!product) dispatch(fetchProductAsync(id!));
    }, [id, dispatch, product]);

    const handleAddItem = (productId: string) => {
        if (basket) {
            dispatch(addBasketItemAsync({ basketId: basket.basketId, productId }));
        }
    };

    if (productStatus.includes('pending')) return <LoadingComponent message="Loading product..." />;

    if (error) return <Typography>{error}</Typography>;
    if (!product) return <NotFound />;

    return (
        <Grid container spacing={6}>
            <Grid item xs={6}>
                <img src={product.pictureUrl} alt={product.name} style={{ width: '100%' }} />
            </Grid>
            <Grid item xs={6}>
                <Typography variant="h3">{product.name}</Typography>
                <Divider />
                <Typography variant="h4" color='secondary' >{product.price}</Typography>
                <TableContainer>
                    <Table>
                        <TableBody>
                            <TableRow>
                                <TableCell>Name</TableCell>
                                <TableCell>{product.name}</TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>Description</TableCell>
                                <TableCell>{product.description}</TableCell>
                            </TableRow>
                            <TableRow>
                                <TableCell>Category</TableCell>
                                <TableCell>{product.categoryName || 'No Category'}</TableCell> 
                            </TableRow>
                            <TableRow>
                                <TableCell>Quantity in stock</TableCell>
                                <TableCell>{product.stockQuantity}</TableCell>
                            </TableRow>
                        </TableBody>
                    </Table>
                </TableContainer>
                <Grid container spacing={2}>
                    <Grid item xs={6}>
                        <Box
                            sx={{
                                height: '55px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center'
                            }}
                        >
                            <Typography
                                color="secondary"
                                variant="h5"
                                textAlign="center"
                            >
                                {item?.quantity}
                            </Typography>
                        </Box>
                    </Grid>
                    <Grid item xs={6}>
                        {product.stockQuantity > 0 ? (
                            <LoadingButton
                                sx={{ height: '55px' }}
                                size={'large'}
                                variant={'contained'}
                                loading={addItemStatus === 'pendingAddItem' + product.productID}
                                onClick={() => handleAddItem(product.productID)}
                                color="secondary"
                            >
                                Add Product
                            </LoadingButton>
                        ) : (
                            <Button disabled size="small">Out of stock</Button>
                        )}
                    </Grid>
                </Grid>
            </Grid>
        </Grid>
    );
}
