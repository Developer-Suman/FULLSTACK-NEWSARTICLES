import { createBrowserRouter, Outlet } from "react-router-dom";
import HomePages from "../Pages/HomePages";
import About from "../Pages/About";
import Navbar from "../Components/Navbar";
import { RightContent } from "../Components/Right-Content";


export const router = createBrowserRouter([
    {
      path: "/",
      element: <div><Navbar/>
        <div>
          <Outlet/>
        </div>
      </div>,
      children: [{
        path: "/",
        index: true,
        element: <RightContent/>,
      },
      {
        path: "about",
        element: <About></About>,
      },{
        path: "yy",
        element: <About></About>,
      }]
    }
    ,{
      path: "*",
      element: <div>404</div>,
    }
  ]);