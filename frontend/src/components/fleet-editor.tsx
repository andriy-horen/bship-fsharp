import { Point, Ship } from '@bship/lib/models';
import { shuffleShips } from '@bship/lib/utils/shuffleShips';
import { DndContext, DragEndEvent, Modifier, useDraggable } from '@dnd-kit/core';
import { restrictToParentElement } from '@dnd-kit/modifiers';
import styled from '@emotion/styled';
import { Button } from '@mui/material';
import { PropsWithChildren, useState } from 'react';
import { Battleship } from './battleship';

const gridSize = 25;

const snapToGrid: Modifier = ({ transform }) => {
  if (!transform) return transform;

  return {
    ...transform,
    x: Math.round(transform.x / gridSize) * gridSize,
    y: Math.round(transform.y / gridSize) * gridSize,
  };
};

export type DraggableShip = {
  ship: Ship;
  position: Point;
};

export const FleetEditor: React.FC = () => {
  const [fleet, setFleet] = useState<DraggableShip[]>([
    {
      ship: [
        ['ship', 'ship', 'ship'],
        ['empty', 'ship', 'empty'],
        ['ship', 'ship', 'ship'],
        ['empty', 'ship', 'empty'],
      ],
      position: { x: 0, y: 0 },
    },
    { ship: [['ship'], ['ship'], ['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship'], ['ship'], ['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship'], ['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship'], ['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship'], ['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship']], position: { x: 0, y: 0 } },
    { ship: [['ship']], position: { x: 0, y: 0 } },
  ]);

  const handleDragEnd = (event: DragEndEvent) => {
    const { delta, active } = event;
    const { id } = active;

    const snappedDelta = {
      x: Math.round(delta.x / gridSize) * gridSize,
      y: Math.round(delta.y / gridSize) * gridSize,
    };

    setFleet((prev) =>
      prev.map((item, index) => {
        if (index === parseInt(id.toString(), 10)) {
          return {
            ...item,
            position: {
              x: item.position.x + snappedDelta.x,
              y: item.position.y + snappedDelta.y,
            },
          };
        }
        return item;
      }),
    );
  };

  return (
    <>
      <DndContext onDragEnd={handleDragEnd} modifiers={[snapToGrid, restrictToParentElement]}>
        <Editor>
          {fleet.map(({ ship, position }, index) => (
            <DraggableItem key={index} id={index.toString()} position={position}>
              <Battleship ship={ship} />
            </DraggableItem>
          ))}
        </Editor>
      </DndContext>
      <Button
        variant="contained"
        onClick={() =>
          setFleet((fleet) => {
            const shuffled = shuffleShips(
              fleet.map((item) => item.ship),
              { gridCols: 10, gridRows: 10, cellSize: 25, allowTouch: false, maxRetries: 10 },
            );
            return shuffled;
          })
        }
      >
        Shuffle Fleet
      </Button>
    </>
  );
};

export type DraggableItemProps = {
  id: string;
  position: Point;
};

const DraggableItem: React.FC<PropsWithChildren<DraggableItemProps>> = ({
  id,
  position,
  children,
}) => {
  const { attributes, listeners, setNodeRef, transform } = useDraggable({ id });

  const style = {
    left: position.x,
    top: position.y,
    // Apply the transform if available
    transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
  };

  return (
    <Position ref={setNodeRef} style={style} {...listeners} {...attributes}>
      {children}
    </Position>
  );
};

const Position = styled.div`
  cursor: move;
  display: inline-block;
  position: absolute;
  touch-action: none;
`;

const Editor = styled.div`
  position: relative;
  width: 250px;
  height: 250px;
  outline: 1px solid black;
`;
