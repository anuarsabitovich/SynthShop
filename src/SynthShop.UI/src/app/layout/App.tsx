import Catalog from "../../features/catalog/Catalog";
import { Container, CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import Header from "./Header";
import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css'
import { useStoreContext } from "../context/StoreContext";
import agent from "../api/agent";
import Cookies from "js-cookie"
import LoadingComponent from "./LoadingComponent";

function App() {
  const {setBasket} = useStoreContext();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initializeBasket = async () => {
      let basketId = Cookies.get('basketId');
      if (!basketId) {
        try {
          // Create a new basket if it does not exist
          const newBasketId = await agent.Basket.create();
          Cookies.set('basketId', newBasketId);
          basketId = newBasketId;
        } catch (error) {
          console.error('Error creating basket:', error);
          setLoading(false);
          return;
        }
      }

      try {
        const basket = await agent.Basket.getById(basketId);
        setBasket(basket);
      } catch (error) {
        console.error('Error fetching basket:', error);
      } finally {
        setLoading(false);
      }
    };

    initializeBasket();
  }, [setBasket]);

  const [darkMode, setDarkMode] = useState(false);   
  const paletteType = darkMode ? 'dark' : 'light';
  const theme = createTheme({
    palette:{
      mode: paletteType,
      background:{
        default: paletteType === 'light' ?  '#eaeaea' : '#121212'
      }
    }
  })

  function handleThemeChange() {
    setDarkMode(!darkMode);
  }

  if (loading) return <LoadingComponent/>

  return (
    <ThemeProvider theme={theme}>
      <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
      <CssBaseline />
      <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
      <Container>
        <Outlet/>
      </Container>
    </ThemeProvider>
  );
}

export default App;
