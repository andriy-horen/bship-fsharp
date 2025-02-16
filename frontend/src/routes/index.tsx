import { AppNavbar } from '@bship/components/app-navbar';
import { Battleship } from '@bship/components/battleship';
import { GameGrid } from '@bship/components/grid';
import Box from '@mui/material/Box';
import React from 'react';

export const LandingPage: React.FC = () => {
  return (
    <>
      <AppNavbar />
      <Box sx={{ p: 2, marginTop: 8 }}>
        {/* prettier-ignore */}
        <GameGrid grid={[
          ['water', 'water', 'ship-all-placed', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'ship-top-placed', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'ship-none-placed', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'ship-none-placed', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
          ['water', 'ship-bottom-placed', 'water', 'water', 'water', 'water', 'water', 'water', 'water', 'water'],
        ]} onClick={() => {}} />

        <Battleship ship={[['ship'], ['ship'], ['ship'], ['ship']]} />
        <Battleship ship={[['ship', 'ship', 'ship', 'ship']]} />
      </Box>
    </>
  );
};
