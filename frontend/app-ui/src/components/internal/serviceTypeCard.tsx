import { Speciality } from "@/domain/types/speciality/Speciality";
import { FC } from "react";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { useNavigate } from "react-router-dom";

const ServiceTypeCard: FC<ServiceTypeCardProps> = ({
  title,
  tradesmanId,
  specialities,
}: ServiceTypeCardProps) => {
  const navigate = useNavigate();

  return (
    <div>
      <div className="flex flex-col items-center justify-center">
        <a href={`/tradesman/${tradesmanId}`}>{title}</a>
        <Carousel className="w-full max-w-xs">
          <CarouselContent>
            {Array.from({ length: specialities.length }).map((_, index) => {
              const speciality: Speciality = specialities[index];
              return (
                <CarouselItem key={index}>
                  <div className="p-1">
                    <img src={speciality.imageUrl} />
                    <p>
                      Pret: {speciality.price} / {speciality.unitOfMeasure}
                    </p>
                  </div>
                </CarouselItem>
              );
            })}
          </CarouselContent>
          <CarouselPrevious />
          <CarouselNext />
        </Carousel>
      </div>
    </div>
  );
};

export default ServiceTypeCard;

export interface ServiceTypeCardProps {
  title: string;
  tradesmanId: string;
  specialities: Speciality[];
}
