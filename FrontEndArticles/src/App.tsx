import { useState } from 'react'
import './App.css'
import GetAllUsers from './Pages/Auth/allUsers'
import GetAllRoles from './Pages/Auth/allRoles'
import GetAllBranch from './Pages/Branch/allBranch'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div>
      <GetAllUsers/>
      <GetAllRoles/>
      <GetAllBranch/>
    </div>
     
  )
}

export default App
