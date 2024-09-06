import * as React from "react";
import * as ReactDOM from "react-dom/client";
import {
  RouterProvider,
} from "react-router-dom";
import "./index.css";
import { ChakraProvider } from '@chakra-ui/react'
import { router } from "./Routes/Router";


ReactDOM.createRoot(document.getElementById("root") as ReactDOM.Container).render(
  // <React.StrictMode>
     <ChakraProvider>
    <RouterProvider router={router}/>
    </ChakraProvider>
  // </React.StrictMode>
);