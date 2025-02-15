import { Ship } from '@bship/lib/models';
import { range } from '@bship/lib/utils';
import { css } from '@emotion/react';

const battleship = css`
  display: flex;
  margin-bottom: -1px;
  margin-right: -1px;
`;

const battleshipSection = css`
  background-color: #afb1c1;
  height: 25px;
  width: 25px;
  display: flex;
  justify-content: center;
  align-items: center;
`;

const peg = css`
  content: '';
  width: 10px;
  height: 10px;
  /* background-color: #b3b4c1; */
  background-color: #87899b;
  border-radius: 50%;
`;

export type BattleshipProps = {
  ship: Ship;
  onClick?: (ship: Ship) => void;
};

export function Battleship({ ship, onClick }: BattleshipProps) {
  return (
    <div css={battleship} onClick={() => onClick?.(ship)}>
      {[...range(ship.length - 1)].map((index) => (
        <div key={index} css={battleshipSection}>
          <div css={peg}></div>
        </div>
      ))}
    </div>
  );
}
