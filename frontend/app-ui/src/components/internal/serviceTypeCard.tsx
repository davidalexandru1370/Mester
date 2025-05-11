import { City } from "@/domain/types/localization";
import { FC } from "react";

const ServiceTypeCard: FC<ServiceTypeCardProps> = ({
  title,
  price,
  location,
  imageUrl,
}: ServiceTypeCardProps) => {
  return (
    <div>
      <div className="flex flex-col items-center justify-center">
        <img
          src={imageUrl}
          alt={title}
          className="w-32 h-32 object-cover rounded-full"
        />
        <h2 className="text-lg font-semibold mt-2">{title}</h2>
        {price && <p className="text-gray-500">{price}</p>}
        <p className="text-gray-500">{location.displayLabel}</p>
      </div>
    </div>
  );
};

export default ServiceTypeCard;

export interface ServiceTypeCardProps {
  title: string;
  price?: string;
  location: City;
  imageUrl: string;
}
