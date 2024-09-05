import { useState, useEffect } from "react";
import { IUser } from "../../types/IUser";
import { UserServices } from "../../api/User/UserServices";

interface IState{
    loading: boolean,
    users: IUser[],
    errorMsg: string
}

const GetAllUsers = ()=>{
    // const [users, setUsers] = useState<IUser[]>([]);

    const [state, setState] = useState<IState>({
        loading: false,
        users: [],
        errorMsg :''

    });
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [pageSize, setpageSize] = useState<number>(10);
    const [totalPages, setTotalPages] = useState<number>(1);


    useEffect(()=>{
        setState({...state, loading:true})
        UserServices.getAllUsers(pageSize, currentPage)
        .then(res=>{
            console.log('API Response:', res.data.Items);
            if(res.data && Array.isArray(res.data.Items))
            {
                setState(prevState=>({
                    ...prevState,
                    loading: false,
                    users: res.data.Items
                }));
                setTotalPages(res.data.TotalPages);
                setpageSize(res.data.PageSize);
            }else{
                console.error('Expected Items array in the response:', res.data);
            }
        })
        .catch(err=> setState({
            ...state, loading:false, errorMsg:err.message
        }));

    },[currentPage, pageSize]);

    const { loading, users, errorMsg } = state;
    return(
        <div>
        <div style={{ margin: '20px', overflowX: 'auto' }}>
            <h2>User Details</h2>
            {errorMsg && (<p>{errorMsg}</p>)}
            {loading && <h1>Loading...</h1>}
        <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: '20px' }}>
            <thead>
                <tr style={{ backgroundColor: '#f2f2f2', textAlign: 'left' }}>
                    <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>SN</th>
                    <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>ID</th>
                    <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>Name</th>
                    <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>Email</th>
                </tr>
            </thead>
            <tbody>
            {users.length > 0 ? (
                        users.map((user, index) => (
                            <tr key={user.Id} style={{ backgroundColor: index % 2 === 0 ? '#f9f9f9' : '#ffffff', cursor: 'pointer' }}>
                                <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{(currentPage - 1) * pageSize + index + 1}</td>
                                <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.Id}</td>
                                <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.UserName}</td>
                                <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.Email}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={4} style={{ textAlign: 'center', padding: '12px' }}>No users found</td>
                        </tr>
                    )}
            </tbody>
        </table>
    </div>
    

    )
};

export default GetAllUsers;