import axios from "axios";
export default axios.Create({
    baseURL: import.meta.VITE_BASE_URL + "/api",
    headers:{
        Authorization: "Bearer" + localStorage.getItem("token"),
        "Content-Type": "application-json",
    },
})