export interface SearchTradesManFilters {
  city?: string;
  county?: string;
  searchText?: string;
  category?: string;
}

export enum FilterActionType {
  UPDATE,
  RESET,
  SET_FILTER,
  REMOVE_FILTER,
}

export interface FilterAction<T> {
  type: FilterActionType;
  payload?: T;
  fieldName?: keyof T;
}
