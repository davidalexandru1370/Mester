import useToken from "@/components/useToken";
import { createContext, useContext, useState, ReactNode } from "react";

export interface UserDetailsDto {
  id: string;
  name: string;
  email: string;
  imageUrl: string;
  isTradesman: boolean;
}

interface UserContextType {
  user: UserDetailsDto | null;
  setUser: (user: UserDetailsDto | null) => void;
  refetchUser: () => Promise<void>;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserContextProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserDetailsDto | null>(null);
  const { token } = useToken();

  const refetchUser = async () => {
    try {
      const response = await fetch("https://localhost:8081/api/user/info", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      if (!response.ok) {
        throw new Error("Failed to fetch user details");
      }
      const data: UserDetailsDto = await response.json();
      setUser(data);
    } catch (error) {
      console.error("Error fetching user details:", error);
      setUser(null);
    }
  };

  return (
    <UserContext.Provider value={{ user, setUser, refetchUser }}>
      {children}
    </UserContext.Provider>
  );
};

export const useUser = (): UserContextType => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error("useUser must be used within a UserProvider");
  }
  return context;
};
