export interface County {
  value: string;
  displayLabel: string;
  cities: City[];
}

export interface City {
  value: string;
  displayLabel: string;
}
