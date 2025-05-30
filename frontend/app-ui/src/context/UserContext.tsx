import { createContext, useContext, useState, ReactNode } from "react";

export interface UserDetailsDto {
  id: string;
  name: string;
  isTradesman: boolean;
}

interface UserContextType {
  user: UserDetailsDto | null;
  setUser: (user: UserDetailsDto | null) => void;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserContextProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserDetailsDto | null>(null);
  return (
    <UserContext.Provider value={{ user, setUser }}>
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
