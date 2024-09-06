export interface IPaginationResponse<T> {
    Items: T[];
    TotalItems: number;
    PageIndex: number;
    PageSize: number;
    TotalPages: number;
    FirstPage: number;
}