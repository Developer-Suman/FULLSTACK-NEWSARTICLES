import { IPaginationResponse } from '../../types/IPaginationResponse';
import { IRoles } from '../../types/IRoles';
import { IUser } from '../../types/IUser';
import api from '../instance'

export class UserServices{
    private static BASE_URL: string = '/Account';

    public static getAllUsers(pageSize:number=2,pageIndex:number=3)
    {
        return api.get<IPaginationResponse<IUser>>(`${this.BASE_URL}/GetAllUser?pageSize=${pageSize}&pageIndex=${pageIndex}`);
    }

    public static getAllRoles(pageSize:number=2, pageIndex:number=3)
    {
        return api.get<IPaginationResponse<IRoles>>(`${this.BASE_URL}/GetAllRoles?PageSize=${pageSize}&PageIndex=${pageIndex}`)
    }
<<<<<<< HEAD
=======
    
>>>>>>> 4c852fbf6b71355783841faf3f4a307909e347af
}