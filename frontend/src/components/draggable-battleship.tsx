import { Ship } from '@bship/lib/models';
import { DndContext, Modifier, useDraggable } from '@dnd-kit/core';
import { restrictToParentElement } from '@dnd-kit/modifiers';
import React, { PropsWithChildren } from 'react';
import { Battleship } from './battleship';

export type DraggableBattleshipProps = {
  ship: Ship;
};

export const DraggableBattleship: React.FC<DraggableBattleshipProps> = ({ ship }) => {
  return (
    <DndContext modifiers={[snapToGrid, restrictToParentElement]}>
      <DraggableItem>
        <Battleship ship={ship} />
      </DraggableItem>
    </DndContext>
  );
};

const GRID_SIZE = 25;

// Modifier function to snap the transform to the nearest grid point
const snapToGrid: Modifier = ({ transform }) => {
  if (!transform) return transform;

  return {
    ...transform,
    x: Math.round(transform.x / GRID_SIZE) * GRID_SIZE,
    y: Math.round(transform.y / GRID_SIZE) * GRID_SIZE,
  };
};

const DraggableItem: React.FC<PropsWithChildren> = ({ children }) => {
  const { attributes, listeners, setNodeRef, transform } = useDraggable({
    id: 'draggable-item',
  });

  const style = {
    cursor: 'move',
    display: 'inline-block',
    // Apply the transform if available
    transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
  };

  return (
    <div ref={setNodeRef} style={style} {...listeners} {...attributes}>
      {children}
    </div>
  );
};
