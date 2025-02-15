import CssBaseline from '@mui/material/CssBaseline';
import { AppProvider } from './app-provider';
import { AppRouter } from './router';

export const App: React.FC = () => {
  return (
    <AppProvider>
      {/* TODO: CssBaseLine probably should be moved elsewhere */}
      <CssBaseline />
      <AppRouter />
    </AppProvider>
  );
};
