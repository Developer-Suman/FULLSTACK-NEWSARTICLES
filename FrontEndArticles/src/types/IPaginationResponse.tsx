export interface IPaginationResponse<T> {
    Items: T[];

    PageIndex: number;
    PageSize: number;
    TotalItems: number;
    TotalPages: number;
    TotalItems: number;
    PageIndex: number;
    PageSize: number;
    TotalPages: number;
    FirstPage: number;
}