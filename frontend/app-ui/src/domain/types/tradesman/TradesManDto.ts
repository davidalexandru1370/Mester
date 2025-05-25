import { Speciality } from "../speciality/Speciality";

export interface TradesManDto {
  id: string;
  name: string;
  description: string;
  city: string;
  county: string;
  specialities: Speciality[];
}
