import { IBranch } from '../../types/IBranch'
import { IPaginationResponse } from '../../types/IPaginationResponse'
import api from '../instance'


export class BranchServices{
    private static BASE_URL:string = '/Branch'

    public static getAllBranch(pageSize:number=2,pageIndex:number=3)
    {
        return api.get<IPaginationResponse<IBranch>>(`${this.BASE_URL}?pageSize=${pageSize}&pageIndex=${pageIndex})`);
    }
}