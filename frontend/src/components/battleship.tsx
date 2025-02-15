import { Ship } from '@bship/lib/models';
import { range } from '@bship/lib/utils';
import { css } from '@emotion/react';
import './Battleship.css';

const battleship = css`
  display: flex;
  margin-bottom: -1px;
  margin-right: -1px;
`;

const peg = css`
  width: 100%;
  height: 100%;
  background-color: #333;
`;

export type BattleshipProps = {
  ship: Ship;
  onClick?: (ship: Ship) => void;
};

export function Battleship({ ship, onClick }: BattleshipProps) {
  return (
    <div
      css={battleship}
      // className={classNames({
      //   battleship: true,
      //   vertical: model.orientation === 'v',
      //   horizontal: model.orientation === 'h',
      // })}
      onClick={() => onClick?.(ship)}
    >
      {[...range(ship.length)].map((index) => (
        <div
          key={index}
          // className={classNames({
          //   'battleship-section': true,
          //   head: index === 0,
          //   tail: index === model.size - 1,
          //   hit: model.hitSections?.includes(index),
          // })}
        >
          <div css={peg}></div>
        </div>
      ))}
    </div>
  );
}
