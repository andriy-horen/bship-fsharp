import React from 'react';
import { createBrowserRouter, RouterProvider } from 'react-router';
import { LandingPage } from './routes';

const router = createBrowserRouter([
  {
    path: '/',
    element: <LandingPage />,
  },
]);

export const AppRouter: React.FC = () => {
  return <RouterProvider router={router} />;
};
