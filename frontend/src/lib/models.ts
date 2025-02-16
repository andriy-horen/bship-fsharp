export type ShipPart = 'empty' | 'ship';

export type Ship = ShipPart[][];

export const defaultFleet: Ship[] = [
  [['ship'], ['ship'], ['ship'], ['ship']],
  [['ship'], ['ship'], ['ship']],
  [['ship'], ['ship'], ['ship']],
  [['ship'], ['ship']],
  [['ship'], ['ship']],
  [['ship'], ['ship']],
  [['ship']],
  [['ship']],
  [['ship']],
  [['ship']],
];

export type Point = {
  x: number;
  y: number;
};
