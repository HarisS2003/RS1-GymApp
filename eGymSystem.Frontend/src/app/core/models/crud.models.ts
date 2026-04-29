export interface CrudEntity {
  id: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
}
