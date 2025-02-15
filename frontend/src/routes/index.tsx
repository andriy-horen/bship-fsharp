import { AppNavbar } from '@bship/components/app-navbar';
import { Battleship } from '@bship/components/battleship';
import Box from '@mui/material/Box';
import React from 'react';

export const LandingPage: React.FC = () => {
  return (
    <>
      <AppNavbar />
      <Box sx={{ p: 2, marginTop: 8 }}>
        <Battleship
          ship={[
            { x: 0, y: 0 },
            { x: 0, y: 1 },
            { x: 0, y: 2 },
            { x: 1, y: 1 },
            { x: 2, y: 1 },
            { x: 3, y: 0 },
            { x: 3, y: 1 },
            { x: 3, y: 2 },
            { x: 4, y: 1 },
          ]}
        />
      </Box>
    </>
  );
};
