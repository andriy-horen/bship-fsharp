import { isFunction } from './is-type';

export function* range<T = number>(
  startOrLength: number,
  end?: number,
  valueOrMapper: T | ((i: number) => T) = (i) => i as T,
  step: number = 1,
): Generator<T> {
  const mapper = isFunction(valueOrMapper) ? valueOrMapper : () => valueOrMapper;
  const start = end ? startOrLength : 0;
  const final = end ?? startOrLength;
  for (let i = start; i <= final; i += step) {
    yield mapper(i);
    if (i + step > final) break;
  }
}
