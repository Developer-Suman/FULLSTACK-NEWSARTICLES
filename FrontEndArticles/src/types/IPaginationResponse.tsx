export interface IPaginationResponse<T> {
    Items: T[];
    PageIndex: number;
    PageSize: number;
    TotalItems: number;
    TotalPages: number;
}