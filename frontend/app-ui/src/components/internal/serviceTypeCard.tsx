import { Speciality } from "@/domain/types/speciality/Speciality";
import { FC } from "react";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { TradesManDto } from "@/domain/types/tradesman/TradesManDto";

const ServiceTypeCard: FC<ServiceTypeCardProps> = ({
  title,
  tradesmanId,
  tradesman,
  specialities,
}: ServiceTypeCardProps) => {
  return (
    <div>
      <div className="flex flex-col items-center justify-center">
        {/* Tradesman profile image */}
        {tradesman.imageUrl && (
          <img
            src={tradesman.imageUrl}
            alt="Profile"
            style={{
              width: 64,
              height: 64,
              borderRadius: "50%",
              objectFit: "cover",
              marginBottom: 16,
              border: "1.5px solid #ccc",
            }}
          />
        )}
        <a href={`/tradesman/${tradesmanId}`}>{title}</a>
        <p>{`${tradesman.county}, ${tradesman.city}`}</p>
        <Carousel className="w-full max-w-xs">
          <CarouselContent>
            {Array.from({ length: specialities.length }).map((_, index) => {
              const speciality: Speciality = specialities[index];
              return (
                <CarouselItem key={index}>
                  <div className="p-1 d-flex flex-column align-items-center">
                    <img
                      src={speciality.imageUrl}
                      style={{ width: "150px", height: "150px" }}
                    />
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
  tradesman: TradesManDto;
  specialities: Speciality[];
}
