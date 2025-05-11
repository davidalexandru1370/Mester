export interface County {
  value: string;
  displayLabel: string;
}

export interface CountyWithCities extends County {
  cities: City[];
}

export interface City {
  value: string;
  displayLabel: string;
  county: County;
}
