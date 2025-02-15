import AppBar from '@mui/material/AppBar';
import Stack from '@mui/material/Stack';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import React from 'react';

export const AppNavbar: React.FC = () => {
  return (
    <AppBar
      position="fixed"
      sx={{
        boxShadow: 0,
        bgcolor: 'background.paper',
        backgroundImage: 'none',
        borderBottom: '1px solid',
        borderColor: 'divider',
        top: 'var(--template-frame-height, 0px)',
      }}
    >
      <Toolbar variant="regular">
        <Stack
          direction="row"
          sx={{
            alignItems: 'center',
            flexGrow: 1,
            width: '100%',
            gap: 1,
          }}
        >
          <Stack direction="row" spacing={1} sx={{ justifyContent: 'center', mr: 'auto' }}>
            <Typography variant="h4" component="h1" sx={{ color: 'text.primary' }}>
              Battleships
            </Typography>
          </Stack>
        </Stack>
      </Toolbar>
    </AppBar>
  );
};
