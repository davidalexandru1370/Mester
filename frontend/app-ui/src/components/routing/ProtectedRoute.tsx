import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";

interface ProtectedRouteProps {
  children: React.ReactNode;
  redirectPath?: string;
  authEndpoint: string; // e.g., "/api/auth/check"
}

export default function ProtectedRoute({
  children,
  redirectPath = "/login",
  authEndpoint,
}: ProtectedRouteProps) {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const response = await axios.get(authEndpoint, {
          withCredentials: true,
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
