import axios from "axios";

const instance = axios.create({
    baseURL: 'https://putalibazartest.hamrosystem.com/api'
  });

export default instance;