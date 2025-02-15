import { BrowserRouter } from 'react-router';
import { App } from './app';

export const AppProvider: React.FC = () => {
  return (
    <BrowserRouter>
      <App />
    </BrowserRouter>
  );
};
