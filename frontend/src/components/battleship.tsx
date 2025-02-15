import { Point, Ship } from '@bship/lib/models';
import { css } from '@emotion/react';
import styled from '@emotion/styled';

const size = 25;

const BattleshipSection = styled.div<Point>(({ x, y }) => {
  return {
    backgroundColor: '#afb1c1',
    height: size,
    width: size,
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    transform: `translate(${size * x}px, ${size * y}px)`,
  };
});

const styles = {
  container: css`
    display: flex;
  `,
  peg: css`
    content: '';
    width: 10px;
    height: 10px;
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
