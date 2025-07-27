import { Point, Ship } from '@bship/lib/models';

export interface ShuffleOptions {
  /** number of cells horizontally */
  gridCols: number;
  /** number of cells vertically */
  gridRows: number;
  /** size of one cell in px (to multiply into final x/y) */
  cellSize: number;
  /** if false, ships must have at least one empty cell between them */
  allowTouch: boolean;
  /** how many attempts before giving up */
  maxRetries?: number;
}

type Placement = { ship: Ship; position: Point };

function rotate90(ship: Ship): Ship {
  const rows = ship.length;
  const cols = ship[0].length;
  const result: Ship = Array.from({ length: cols }, () => Array(rows).fill('empty'));

  for (let y = 0; y < rows; y++) {
    for (let x = 0; x < cols; x++) {
      result[x][rows - 1 - y] = ship[y][x];
    }
  }

  return result;
}

function uniqueRotations(ship: Ship): Ship[] {
  const rotations: Ship[] = [];
  let current = ship;
  for (let i = 0; i < 4; i++) {
    const key = JSON.stringify(current);
    if (!rotations.some((r) => JSON.stringify(r) === key)) rotations.push(current);
    current = rotate90(current);
  }
  return rotations;
}

function shuffle<T>(arr: T[]) {
  for (let i = arr.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [arr[i], arr[j]] = [arr[j], arr[i]];
  }
}

export function shuffleShips(ships: Ship[], opts: ShuffleOptions): Placement[] {
  const { gridCols, gridRows, allowTouch, cellSize, maxRetries = 500 } = opts;

  for (let attempt = 0; attempt < maxRetries; attempt++) {
    // occupancy and adjacency grids
    const occ = Array.from({ length: gridRows }, () => Array(gridCols).fill(false));
    const adj = allowTouch
      ? occ
      : Array.from({ length: gridRows }, () => Array(gridCols).fill(false));

    const placed: Placement[] = [];
    const order = [...ships];
    shuffle(order);

    let failed = false;
    for (const ship of order) {
      const rots = uniqueRotations(ship);
      const candidates: { rot: Ship; x: number; y: number }[] = [];

      for (const rot of rots) {
        const h = rot.length,
          w = rot[0].length;
        for (let y = 0; y <= gridRows - h; y++) {
          for (let x = 0; x <= gridCols - w; x++) {
            candidates.push({ rot, x, y });
          }
        }
      }
      shuffle(candidates);

      let placedOne = false;
      for (const { rot, x, y } of candidates) {
        // check if all ship-cells can go into empty (and non-adjacent if required)
        let ok = true;
        for (let dy = 0; dy < rot.length && ok; dy++) {
          for (let dx = 0; dx < rot[0].length; dx++) {
            if (rot[dy][dx] === 'ship') {
              if (occ[y + dy][x + dx] || (!allowTouch && adj[y + dy][x + dx])) {
                ok = false;
                break;
              }
            }
          }
        }
        if (!ok) continue;

        // mark it
        for (let dy = 0; dy < rot.length; dy++) {
          for (let dx = 0; dx < rot[0].length; dx++) {
            if (rot[dy][dx] === 'ship') {
              occ[y + dy][x + dx] = true;
              if (!allowTouch) {
                // bump adjacency
                for (let ay = y + dy - 1; ay <= y + dy + 1; ay++) {
                  for (let ax = x + dx - 1; ax <= x + dx + 1; ax++) {
                    if (ay >= 0 && ay < gridRows && ax >= 0 && ax < gridCols) {
                      adj[ay][ax] = true;
                    }
                  }
                }
              }
            }
          }
        }

        placed.push({
          ship: rot,
          position: { x: x * cellSize, y: y * cellSize },
        });
        placedOne = true;
        break;
      }

      if (!placedOne) {
        failed = true;
        break;
      }
    }

    if (!failed && placed.length === ships.length) {
      return placed;
    }
  }

  throw new Error('Unable to shuffle ships within retry limit');
}
