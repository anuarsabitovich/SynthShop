import { Container, CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import Header from "./Header";
import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import agent from "../api/agent";
import Cookies from "js-cookie";
import LoadingComponent from "./LoadingComponent";
import { useAppDispatch } from "../store/configureStore";
import { setBasket } from "../../features/basket/basketSlice";

function App() {
  const dispatch = useAppDispatch();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initializeBasket = () => {
      let basketId = Cookies.get('basketId');
      if (!basketId) {
        agent.Basket.create()
          .then(newBasketId => {
            Cookies.set('basketId', newBasketId);
            basketId = newBasketId;
            return agent.Basket.getById(basketId);
          })
          .then(basket => {
            dispatch(setBasket(basket));
            setLoading(false);
          })
          .catch(error => {
            console.error('Error creating basket:', error);
            setLoading(false);
          });
      } else {
        agent.Basket.getById(basketId)
          .then(basket => {
            dispatch(setBasket(basket));
            setLoading(false);
          })
          .catch(error => {
            console.error('Error fetching basket:', error);
            setLoading(false);
          });
      }
    };

    initializeBasket();
  }, [dispatch]);

  const [darkMode, setDarkMode] = useState(false);   
  const paletteType = darkMode ? 'dark' : 'light';
  const theme = createTheme({
    palette: {
      mode: paletteType,
      background: {
        default: paletteType === 'light' ? '#eaeaea' : '#121212'
      }
    }
  });

  function handleThemeChange() {
    setDarkMode(!darkMode);
  }

  if (loading) return <LoadingComponent message='Initialising app...' />;

  return (
    <ThemeProvider theme={theme}>
      <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
      <CssBaseline />
      <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
      <Container>
        <Outlet />
      </Container>
    </ThemeProvider>
  );
}

export default App;
