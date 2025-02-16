import { GameTheme } from '@bship/lib/theme';
import styled from '@emotion/styled';

const size = GameTheme.squareSize;

const GameGridContainer = styled.div`
  display: flex;
  flex-direction: column;
`;

const GridRow = styled.div`
  display: flex;
  flex-direction: row;
`;

const WaterSquare = styled.div`
  background-color: #0077be;
  width: ${size}px;
  height: ${size}px;
`;

const ShipSquare = styled.div`
  background-color: #333;
  width: ${size}px;
  height: ${size}px;
`;

export type ShipSquareSide =
  | 'top'
  | 'right'
  | 'bottom'
  | 'left'
  | 'top-right'
  | 'top-left'
  | 'bottom-right'
  | 'bottom-left'
  | 'all'
  | 'none';

export type ShipSquareState = 'placed' | 'hit' | 'sunken';

export type ShipSquare = `ship-${ShipSquareSide}-${ShipSquareState}`;

export type WaterSquare = 'water' | 'water-hit';

export type Square = ShipSquare | WaterSquare;

const getGridSquare = (square: Square) => {
  switch (square) {
    case 'water':
    case 'water-hit':
      return <WaterSquare />;
    case 'ship-top-placed':
    case 'ship-right-placed':
    case 'ship-bottom-placed':
    case 'ship-left-placed':
    case 'ship-top-right-placed':
    case 'ship-top-left-placed':
    case 'ship-bottom-right-placed':
    case 'ship-bottom-left-placed':
    case 'ship-all-placed':
    case 'ship-none-placed':
      return <ShipSquare />;
    default:
      return <WaterSquare />;
  }
};

export type GameGridProps = {
  grid: Square[][];
  onClick: (row: number, col: number) => void;
};

export const GameGrid: React.FC<GameGridProps> = ({ grid, onClick }) => {
  return (
    <GameGridContainer>
      {grid.map((row, rowIndex) => (
        <GridRow key={rowIndex}>
          {row.map((square, colIndex) => (
            <div key={colIndex} onClick={() => onClick(rowIndex, colIndex)}>
              {getGridSquare(square)}
            </div>
          ))}
        </GridRow>
      ))}
    </GameGridContainer>
  );
};
