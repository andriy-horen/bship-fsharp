import { GameTheme } from '@bship/lib/theme';
import styled from '@emotion/styled';

const size = GameTheme.squareSize;

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

export type ShipSquareState = 'placed' | 'hit';

export type ShipSquare = `ship-${ShipSquareSide}-${ShipSquareState}`;

export type WaterSquare = 'water' | 'water-hit' | 'water-miss';

export type Square = ShipSquare | WaterSquare;

const getGridSquare = (square: Square) => {
  switch (square) {
    case 'water':
      return <WaterSquare />;
    case 'water-hit':
      return (
        <WaterSquare>
          <HitMark />
        </WaterSquare>
      );
    case 'water-miss':
      return (
        <WaterSquare>
          <MissMark />
        </WaterSquare>
      );
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
      return (
        <ShipSquare>
          <Peg />
        </ShipSquare>
      );
    case 'ship-top-hit':
    case 'ship-right-hit':
    case 'ship-bottom-hit':
    case 'ship-left-hit':
    case 'ship-top-right-hit':
    case 'ship-top-left-hit':
    case 'ship-bottom-right-hit':
    case 'ship-bottom-left-hit':
    case 'ship-all-hit':
    case 'ship-none-hit':
      return (
        <HitShipSquare>
          <HitPeg />
        </HitShipSquare>
      );
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

const Peg = styled.div`
  width: ${size / 2.5}px;
  height: ${size / 2.5}px;
  /* background-color: #b3b4c1; */
  background-color: #87899b;
  border-radius: 50%;
`;

const HitMark = styled.div`
  background-color: #ff4b4b;
  width: 7px;
  height: 7px;
  border-radius: 50%;
`;

const MissMark = styled.div`
  background-color: #abc5d4;
  width: 7px;
  height: 7px;
  border-radius: 50%;
`;

const GameGridContainer = styled.div`
  display: flex;
  flex-direction: column;
`;

const GridRow = styled.div`
  display: flex;
  flex-direction: row;
`;

const WaterSquare = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #e8f7ff;
  width: ${size}px;
  height: ${size}px;
  border-radius: 4px;
`;

const ShipSquare = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #afb1c1;
  width: ${size}px;
  height: ${size}px;
`;

const HitShipSquare = styled(ShipSquare)`
  background-color: #ffaeae;
`;

const HitPeg = styled.div`
  background: transparent;
  display: flex;
  height: 14px;
  width: 14px;
  margin-left: 2px;

  &::before,
  &::after {
    content: '';
    width: 4px;
    background: #ff4b4b;
    position: relative;
  }

  &::before {
    transform: rotate(-45deg);
    left: 4px;
  }
  &::after {
    transform: rotate(45deg);
    left: 0;
  }
`;
