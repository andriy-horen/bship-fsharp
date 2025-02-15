import { AppProvider } from './app-provider';
import { AppRouter } from './router';

export const App: React.FC = () => {
  return (
    <AppProvider>
      <AppRouter />
    </AppProvider>
  );
};
