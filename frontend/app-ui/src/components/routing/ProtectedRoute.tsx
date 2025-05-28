import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import useToken from "../useToken";

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

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await axios.get(authEndpoint, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.status === 200) {
          setIsAuthenticated(true);
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
