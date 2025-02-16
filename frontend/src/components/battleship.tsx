import { Point, Ship } from '@bship/lib/models';
import { GameTheme } from '@bship/lib/theme';
import { css } from '@emotion/react';
import styled from '@emotion/styled';

const size = GameTheme.squareSize;

const BattleshipSection = styled.div<Point>(({ x, y }) => {
  return {
    transform: `translate(${size * x}px, ${size * y}px)`,
  };
});

const styles = {
  container: css`
    display: flex;

    > div {
      background-color: #afb1c1;
      height: ${size}px;
      width: ${size}px;
      display: flex;
      justify-content: center;
      align-items: center;
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

export type BattleshipProps = {
  ship: Ship;
  onClick?: (ship: Ship) => void;
};

export function Battleship({ ship, onClick }: BattleshipProps) {
  return (
    <div css={styles.container} onClick={() => onClick?.(ship)}>
      {ship.map((p, index) => (
        <BattleshipSection key={index} x={p.x - index} y={p.y}>
          <div css={styles.peg}></div>
        </BattleshipSection>
      ))}
    </div>
  );
}
