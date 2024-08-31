

import React, { useEffect, useState } from 'react'
import { IRoles } from '../../types/IRoles'
import { UserServices } from '../../api/User/UserServices';

const allRoles = () => {

    const[roles, setRoles] = useState<IRoles[]>([]);
    const[currentPage, setCurrentPage] = useState<number>(1);
    const[pageSize, setPageSize] = useState<number>(1);



    useEffect(()=>{
        UserServices.getAllRoles(pageSize, currentPage)
        .then(res => {
            console.log('Api Response: ', res.data.Items);
            if(res.data && Array.isArray(res.data.Items))
            {
                setRoles(res.data.Items)
            }else{
                console.error('Expected Item array in the response', res.data)
            }
        })
        .catch(err => console.error('Error feching users:', err))
        ;

    },[pageSize, currentPage])

  return (
    <div style={{ margin: '20px', overflowX: 'auto' }}>
            <h2>User Details</h2>
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
                {roles.map((role, index) => (
                    <tr key={role.Id} style={{ backgroundColor: index % 2 === 0 ? '#f9f9f9' : '#ffffff', cursor: 'pointer' }}>
                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{(currentPage - 1) * pageSize + index + 1}</td>
                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{role.Id}</td>
                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{role.Name}</td>
                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{role.NormalizedName}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    </div>
  )
}

export default allRoles