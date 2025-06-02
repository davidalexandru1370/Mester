import "./App.css";
import "bootstrap/dist/css/bootstrap.min.css";
import MainPage from "./components/MainPage.tsx";
import Auth from "./components/Auth.tsx";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Profile from "./components/Profile.tsx";
import Logout from "./components/Logout.tsx";
import ProtectedRoute from "./components/routing/ProtectedRoute.tsx";
import Conversations from "./components/message/Conversations.tsx";
import Requests from "./components/message/Requests.tsx";
import TradesmanProfile from "./components/TradesmanProfile.tsx";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <MainPage />
            </ProtectedRoute>
          }
        />
        <Route path="/auth" element={<Auth />} />
        <Route
          path="/main"
          element={
            <ProtectedRoute>
              <MainPage />
            </ProtectedRoute>
          }
        />
        <Route path="/user" element={<Profile />} />
        <Route path="/logout" element={<Logout />} />
        <Route
          path="/conversations"
          element={
            <ProtectedRoute>
              <Conversations />
            </ProtectedRoute>
          }
        />
        <Route
          path="/requests"
          element={
            <ProtectedRoute>
              <Requests />
            </ProtectedRoute>
          }
        />
        <Route
          path="/tradesman/:tradesmanId"
          element={
            <ProtectedRoute>
              <TradesmanProfile />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
