import { Ship, ShipPart as ShipPartType } from '@bship/lib/models';
import { GameTheme } from '@bship/lib/theme';
import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Fragment } from 'react';

const size = GameTheme.squareSize;

const EmptyPart = styled.div`
  width: ${size}px;
  height: ${size}px;
`;

const ShipPart = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: ${size}px;
  height: ${size}px;
  background-color: #afb1c1;
`;

const styles = {
  container: css`
    display: flex;
    flex-direction: column;

    > div {
      display: flex;
      flex-direction: row;
    }
  `,
  peg: css`
    content: '';
    width: ${size / 2.5}px;
    height: ${size / 2.5}px;
    /* background-color: #b3b4c1; */
    background-color: #87899b;
    border-radius: 50%;
  `,
};

const getShipPart = (part: ShipPartType) => {
  switch (part) {
    case 'empty':
      return <EmptyPart />;
    case 'ship':
      return (
        <ShipPart>
          <div css={styles.peg}></div>
        </ShipPart>
      );
  }
};

export type BattleshipProps = {
  ship: Ship;
  onClick?: (ship: Ship) => void;
};

export function Battleship({ ship, onClick }: BattleshipProps) {
  return (
    <div css={styles.container} onClick={() => onClick?.(ship)}>
      {ship.map((row, i) => (
        <div key={i}>
          {row.map((part, j) => (
            <Fragment key={j}>{getShipPart(part)}</Fragment>
          ))}
        </div>
      ))}
    </div>
  );
}
