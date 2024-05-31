import { Box, IconButton, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
import { Add, Delete, Remove } from "@mui/icons-material";
import { useStoreContext } from "../../app/context/StoreContext";
import { useState } from "react";
import agent from "../../app/api/agent";

export default function BasketPage() {
    const { basket, setBasket, removeItem } = useStoreContext();
    const [loading, setLoading] = useState(false);

    function handleAddItem(productId: string){
        setLoading(true);
        agent.Basket.addItem(basket, productId)
        .then(basket => setBasket(basket))
        .catch(error => console.log(error))
        .finally(() => setLoading(false))
    }

    function handleRemoveItem(productId: string) {
      setLoading(true);
      agent.Basket.updateItem(basket,productId, )
    }

    if (!basket || basket.items.length === 0) return <Typography variant='h3'>Your basket is empty</Typography>;

    return (
        <TableContainer component={Paper}>
            <Table sx={{ minWidth: 650 }}>
                <TableHead>
                    <TableRow>
                        <TableCell>Product</TableCell>
                        <TableCell align="right">Price</TableCell>
                        <TableCell align="center">Quantity</TableCell>
                        <TableCell align="right">Subtotal</TableCell>
                        <TableCell align="right"></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {basket.items.map(item => (
                        <TableRow
                            key={item.productId}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell component="th" scope="row">
                              <Box display='flex' alignItems='center'>
                                <img src={item.product.pictureUrl} alt={item.product.name} style={{height:50, marginRight:20} } />
                                <span>{item.product.name}</span>
                              </Box>
                            </TableCell>
                            <TableCell align="right">{item.product.price}</TableCell>
                            <TableCell align="center">
                              <IconButton color="error">
                                  <Remove/>
                              </IconButton>
                              {item.quantity}
                              <IconButton color="secondary">
                                  <Add/>
                              </IconButton>
                              </TableCell>
                            <TableCell align="right">{item.product.price * item.quantity}</TableCell>
                            <TableCell align="right">
                                <IconButton color='error'>
                                    <Delete />
                                </IconButton>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}
