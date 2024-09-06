export interface IPaginationResponse<T> {
    Items: T[];
<<<<<<< HEAD
    PageIndex: number;
    PageSize: number;
    TotalItems: number;
    TotalPages: number;
=======
    TotalItems: number;
    PageIndex: number;
    PageSize: number;
    TotalPages: number;
    FirstPage: number;
>>>>>>> 4c852fbf6b71355783841faf3f4a307909e347af
}