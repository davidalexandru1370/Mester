import { useContext, useEffect, useReducer, useState } from "react";
import NavMenu from "./NavMenu.tsx";
import OfferWindow from "./OfferWindow.tsx";
import axios from "axios";
import { Container } from "reactstrap";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DataStoreContext } from "@/context/DataStoreContext.tsx";
import {
  FilterAction,
  FilterActionType,
  SearchTradesManFilters,
} from "@/domain/types/filters.ts";

function updateFiltersReducer(
  state: SearchTradesManFilters,
  action: FilterAction<SearchTradesManFilters>
): SearchTradesManFilters {
  switch (action.type) {
    case FilterActionType.UPDATE:
      return {
        ...state,
        ...(action.payload ?? {}),
      };
    case FilterActionType.RESET:
      return {};
    case FilterActionType.SET_FILTER:
      return {
        ...state,
        [action.fieldName as keyof SearchTradesManFilters]:
          action.payload?.[action.fieldName as keyof SearchTradesManFilters],
      };
    case FilterActionType.REMOVE_FILTER:
      const { [action.fieldName as keyof SearchTradesManFilters]: _, ...rest } =
        state;
      return rest;
  }

  return state;
}

export default function MainPage() {
  const [tradesmen, setTradesmen] = useState<any>([]);
  const [filters, updateFilters] = useReducer(updateFiltersReducer, {});
  const { counties } = useContext(DataStoreContext);

  useEffect(() => {
    (async () => await Load())();
  }, []);

  async function Load() {
    await axios
      .post(
        "https://localhost:8081/api/TradesMan",
        {
          filter: {},
        },
        {
          timeout: 5000,
          headers: {
            "Content-Type": "application/json",
            accept: "application/json", // If you receieve JSON response.
          },
        }
      )
      .then((response) => {
        console.log("Got them!");
        setTradesmen(response.data);
      });
  }

  return (
    <section>
      <NavMenu />
      <Container className={cn("inline-flex gap-4")}>
        <Input placeholder="Ce anume cauti?" />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline">
              {filters.county === undefined ? "Toata Romania" : filters.county}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent className="w-56 max-h-[300px] overflow-auto min-w-[220px]">
            <DropdownMenuGroup>
              {counties.map((county) => {
                return (
                  <DropdownMenuItem
                    key={county.value}
                    onClick={() => {
                      updateFilters({
                        type: FilterActionType.SET_FILTER,
                        fieldName: "county",
                        payload: {
                          county: county.value,
                        },
                      });
                    }}
                  >
                    {county.displayLabel}
                  </DropdownMenuItem>
                );
              })}
            </DropdownMenuGroup>
          </DropdownMenuContent>
        </DropdownMenu>
      </Container>
      <h1>Current offers</h1>
      <br></br>
      <table className="table table-dark" align="center">
        <thead>
          <tr>
            <th scope="col">Id</th>
            <th scope="col">Name</th>
            <th scope="col">Description</th>
          </tr>
        </thead>
        {tradesmen?.map(function fn(tradesman: any) {
          return (
            <tbody>
              <tr>
                <th scope="row">{tradesman.id} </th>
                <td>{tradesman.name}</td>
                <td>{tradesman.description}</td>
              </tr>
            </tbody>
          );
        })}
      </table>
    </section>
  );
}
