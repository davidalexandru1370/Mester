import { useState, useEffect, useContext } from "react";
import "./App.css";
import "bootstrap/dist/css/bootstrap.min.css";
import MainPage from "./components/MainPage.tsx";
import useToken from "./components/useToken.tsx";
import Auth from "./components/Auth.tsx";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Profile from "./components/Profile.tsx";
import Logout from "./components/Logout.tsx";

function App() {
  const { token, setToken } = useToken();

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={token ? <MainPage /> : <Auth />} />
        <Route path="/auth" element={<Auth />} />
        <Route path="/main" element={<MainPage />} />
        <Route path="/user" element={<Profile />} />
        <Route path="/logout" element={<Logout />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
