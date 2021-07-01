export interface Loadable<T> {
  loading: boolean;
  loaded: boolean;
  obj?: T;
}