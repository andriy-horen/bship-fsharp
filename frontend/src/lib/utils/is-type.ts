// eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
export const isFunction = (value: unknown): value is Function => {
  return !!(value && typeof value === 'function');
};
