import { Speciality } from "../speciality/Speciality";

export interface TradesManDto {
  id: string;
  name: string;
  description: string;
  specialities: Speciality[];
}
