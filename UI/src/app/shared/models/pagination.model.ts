export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortColumn?: string;
  sortOrder?: string;
  filterColumn?: string;
  filterValue?: string;
}
