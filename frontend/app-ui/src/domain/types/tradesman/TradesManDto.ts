import { Speciality } from "../speciality/Speciality";

export interface TradesManDto {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
  city: string;
  county: string;
  specialities: Speciality[];
}
