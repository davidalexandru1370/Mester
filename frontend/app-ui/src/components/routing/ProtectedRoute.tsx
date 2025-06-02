import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import useToken from "../useToken";
import { useUser } from "@/context/UserContext";

interface ProtectedRouteProps {
  children: React.ReactNode;
  redirectPath?: string;
}

export default function ProtectedRoute({
  children,
  redirectPath = "/auth",
}: ProtectedRouteProps) {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const authEndpoint: string = "https://localhost:8081/api/user/authorize";
  const { token } = useToken();
  const { user, setUser } = useUser();

  useEffect(() => {
    const fetchUserDetails = async () => {
      try {
        const response = await axios.get(
          "https://localhost:8081/api/user/info",
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        if (response.status === 200) {
          setUser(response.data);
        } else {
          setUser(null);
        }
      } catch (error) {
        console.error("Failed to fetch user details:", error);
        setUser(null);
        throw error;
      }
    };

    const checkAuth = async () => {
      try {
        const response = await axios.get(authEndpoint, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.status === 200) {
          setIsAuthenticated(true);
          if (!user) {
            await fetchUserDetails();
          }
        } else {
          setIsAuthenticated(false);
        }
      } catch {
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };
    checkAuth();
  }, [authEndpoint]);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      navigate(redirectPath, { replace: true });
    }
  }, [isLoading, isAuthenticated, navigate, redirectPath]);

  if (isLoading || !isAuthenticated) {
    return null;
  }

  return <>{children}</>;
}
