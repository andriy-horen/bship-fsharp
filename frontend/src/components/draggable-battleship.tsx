import React from 'react';

type DraggableBattleshipProps = {
  id: string;
  name: string;
  position: { x: number; y: number };
  onDrag: (id: string, newPosition: { x: number; y: number }) => void;
};

export const DraggableBattleship: React.FC<DraggableBattleshipProps> = ({
  id,
  name,
  position,
  onDrag,
}) => {
  const handleDrag = (event: React.DragEvent<HTMLDivElement>) => {
    const newPosition = {
      x: event.clientX,
      y: event.clientY,
    };
    onDrag(id, newPosition);
  };

  return (
    <div
      draggable
      onDrag={handleDrag}
      style={{ position: 'absolute', left: position.x, top: position.y }}
    >
      {name}
    </div>
  );
};
