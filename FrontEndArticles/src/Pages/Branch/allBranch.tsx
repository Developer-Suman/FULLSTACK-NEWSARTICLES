import { useState, useEffect, useCallback } from "react";
import { IBranch } from "../../types/IBranch";
import { BranchServices } from "../../api/Branch/BranchServices";

interface IState {
    loading: boolean;
    users: IBranch[];
    errorMsg: string;
}

const allBranch: React.FC = () => {
    const [state, setState] = useState<IState>({
        loading: false,
        users: [],
        errorMsg: ''
    });
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [pageSize, setPageSize] = useState<number>(10);
    const [totalPages, setTotalPages] = useState<number>(1);

    const fetchUsers = useCallback(() => {
        setState(prevState => ({ ...prevState, loading: true, errorMsg: '' }));
        
        BranchServices.getAllBranch(pageSize, currentPage)
            .then(res => {
                if (res.data && Array.isArray(res.data.Items)) {
                    setState({
                        loading: false,
                        users: res.data.Items,
                        errorMsg: ''
                    });
                    setTotalPages(res.data.TotalPages);
                    setPageSize(res.data.PageSize);
                } else {
                    console.error('Expected Items array in the response:', res.data);
                    setState({
                        ...state,
                        loading: false,
                        errorMsg: 'Invalid data format from API'
                    });
                }
            })
            .catch(err => setState({
                ...state,
                loading: false,
                errorMsg: err.message || 'Failed to fetch users'
            }));
    }, [currentPage, pageSize]);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const { loading, users, errorMsg } = state;

    const handlePageChange = (newPage: number) => {
        if (newPage > 0 && newPage <= totalPages) {
            setCurrentPage(newPage);
        }
    };

    return (
        <div style={{ margin: '20px', overflowX: 'auto' }}>
            <h2>User Details</h2>
            {errorMsg && (<p style={{ color: 'red' }}>{errorMsg}</p>)}
            {loading ? (
                <h1>Loading...</h1>
            ) : (
                <>
                    <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: '20px' }}>
                        <thead>
                            <tr style={{ backgroundColor: '#f2f2f2', textAlign: 'left' }}>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>SN</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>ID</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>BranchNameInNepali</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>BranchNameInEnglish</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>BranchHeadNameInEnglish</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>BranchHeadNameInNepali</th>
                                <th style={{ padding: '12px', borderBottom: '2px solid #ddd' }}>IsActive</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.length > 0 ? (
                                users.map((user, index) => (
                                    <tr
                                        key={user.Id}
                                        style={{
                                            backgroundColor: index % 2 === 0 ? '#f9f9f9' : '#ffffff',
                                            cursor: 'pointer'
                                        }}
                                    >
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>
                                            {(currentPage - 1) * pageSize + index + 1}
                                        </td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.Id}</td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.branchNameInNepali}</td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.branchNameInEnglish}</td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.branchHeadNameInEnglish}</td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.branchHeadNameInNepali}</td>
                                        <td style={{ padding: '12px', borderBottom: '1px solid #ddd' }}>{user.isActive}</td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan={4} style={{ textAlign: 'center', padding: '12px' }}>
                                        No users found
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <div style={{ display: 'flex', justifyContent: 'center', gap: '10px' }}>
                        <button
                            onClick={() => handlePageChange(currentPage - 1)}
                            disabled={currentPage === 1}
                            style={{ padding: '8px 12px', cursor: 'pointer', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px' }}
                        >
                            Previous
                        </button>
                        <span>{` Page ${currentPage} of ${totalPages} `}</span>
                        <button
                            onClick={() => handlePageChange(currentPage + 1)}
                            disabled={currentPage === totalPages}
                            style={{ padding: '8px 12px', cursor: 'pointer', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px' }}
                        >
                            Next
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default allBranch;
