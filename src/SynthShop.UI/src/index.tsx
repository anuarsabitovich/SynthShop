import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './app/layout/App.tsx'
import '../src/app/layout/styles.css'
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { router } from './app/router/Routes.tsx';
import { RouterProvider } from 'react-router-dom';
import { StoreProvider } from './app/context/StoreContext.tsx';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <StoreProvider>
      <RouterProvider router={router} />
    </StoreProvider>
  </React.StrictMode>
)
