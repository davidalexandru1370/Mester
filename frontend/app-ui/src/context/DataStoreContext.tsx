import { County, City } from "@/domain/types/localization";
import { createContext, FC, useState } from "react";
import * as counties from "@/assets/data/regions.json";

interface DataStoreContextType {
  counties: County[];
  setCounties: React.Dispatch<React.SetStateAction<County[]>>;
}

const initialCounties: County[] = Object.entries(counties).map(
  ([key, value]): County => {
    const cities: City[] = Object.values(value).map((city: any): City => {
      return {
        value: city.name,
        displayLabel: city.name,
      };
    });

    return {
      value: key,
      displayLabel: key,
      cities: cities,
    };
  }
);

export const DataStoreContext = createContext<DataStoreContextType>({
  counties: initialCounties,
  setCounties: () => {},
});

export const DataStoreContextProvider: FC<{ children: any }> = ({
  children,
}) => {
  const [counties, setCounties] = useState<County[]>(initialCounties);

  return (
    <DataStoreContext.Provider value={{ counties, setCounties }}>
      {children}
    </DataStoreContext.Provider>
  );
};
