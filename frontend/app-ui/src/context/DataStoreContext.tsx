import { County, City, CountyWithCities } from "@/domain/types/localization";
import { createContext, FC, useState } from "react";
import * as counties from "@/assets/data/regions.json";

interface DataStoreContextType {
  counties: CountyWithCities[];
  setCounties: React.Dispatch<React.SetStateAction<CountyWithCities[]>>;
}

const initialCounties: CountyWithCities[] = Object.entries(counties).map(
  ([key, value]): CountyWithCities => {
    const cities: City[] = Object.values(value).map((city: any): City => {
      return {
        value: city.name,
        displayLabel: city.name,
        county: {
          value: key,
          displayLabel: key,
        },
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
  const [counties, setCounties] = useState<CountyWithCities[]>(initialCounties);

  return (
    <DataStoreContext.Provider value={{ counties, setCounties }}>
      {children}
    </DataStoreContext.Provider>
  );
};
