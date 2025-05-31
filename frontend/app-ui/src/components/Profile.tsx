import React, { useEffect, useState } from "react"
import NavMenu from "./NavMenu"
import { ToastContainer, toast } from 'react-toastify';
import axios from "axios";
import useToken from './useToken';

export default function () {

    const { token, setToken } = useToken();
    const [specialities, setSpecialities] = useState<any>([]);
    const [addedSpecialities, setAddedSpecialities] = useState<any>([]);
    const [name, setName] = useState("");
    const [price, setPrice] = useState("");
    const [unitOfMeasure, setUnitOfMeasure] = useState("");
    const [description, setDescription] = useState("");
    const [city, setCity] = useState("");
    const [county, setCounty] = useState("");

    const isNumeric = (string: string) => Number.isFinite(+string)

    useEffect(() => {
      (async () => await Load())();
    }, []);

    async function Load() {
      await axios.get(
        "https://localhost:8081/api/TradesMan/specialities",
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
        setSpecialities(response.data);
      });
    } 

    async function becomer(event: { preventDefault: () => void; }) {  

        event.preventDefault();
        try {
            await axios.post("https://localhost:8081/api/User/createTradesManProfile", {
    
                specialities: addedSpecialities,
                description: description,
                city: city,
                county: county,
            }, {
                timeout: 5000,
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    accept: 'application/json', // If you receieve JSON response.
                }
            }).then(function () {
              console.log("Success!");
          });
          toast("Success!");
    
        } catch (err) {
            let errorMessage = "Failed to do something exceptional";
        if (err instanceof Error) {
            errorMessage = err.message;
        }
        toast(errorMessage);
        }
    }

    async function addSpeciality(event: { preventDefault: () => void; }) {  

        event.preventDefault();
        if(!specialities.includes(name))
        {
          toast("Speciality not found!");
          return;
        }
        if(!isNumeric(price))
        {
          toast("Price is not numeric!");
          return;
        }
        setAddedSpecialities([
          ...addedSpecialities,
          { price: Number(price), name: name, unitOfMeasure: unitOfMeasure }
        ]);
    }

  return (
    <div>
      <NavMenu/>
    <div className="Auth-form-container">
      <form className="Auth-form">
        <div className="Auth-form-content">
          <h3 className="Auth-form-title">Update Profile</h3>
          <form className="Auth-form">
          <div className="Auth-form-content">
            <h4 className="Auth-form-title">Add specialities</h4>
            <div className="d-grid gap-2 mt-3">
              <div className="form-group mt-3">
              <label>Speciality</label>
              <input
                type="name"
                className="form-control mt-1"
                placeholder="Speciality"
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </div>
            <div className="form-group mt-3">
              <label>Price</label>
              <input
                type="price"
                className="form-control mt-1"
                placeholder="Price"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
              />
            </div>
            <div className="form-group mt-3">
              <label>Unit of Measure</label>
              <input
                type="unitOfMeasure"
                className="form-control mt-1"
                placeholder="Unit of Measure"
                value={unitOfMeasure}
                onChange={(e) => setUnitOfMeasure(e.target.value)}
              />
            </div>
              <button type="submit" className="btn btn-primary" onClick={addSpeciality}>
                Add speciality
              </button>
            </div>
          </div>
          </form>
          <h4 className="Auth-form-title">Currently added specialities</h4>
           <table className="table table-dark" align="center">
                <thead>
                    <tr>
                        <th scope="col">
                                Name
                        </th>
                        <th scope="col">
                                Price
                        </th>
                        <th scope="col">
                                Unit of measure
                        </th>
                    </tr>
                </thead>
                {addedSpecialities?.map(function fn(added: any) {
                    return (
                        <tbody>
                            <tr>
                                <th scope="row">{added.name} </th>
                                <td>{added.price}</td>
                                <td>{added.unitOfMeasure}</td>
                            </tr>
                        </tbody>
                    );
                })}
            </table>
          <div className="d-grid gap-2 mt-3">
              <div className="form-group mt-3">
              <label>Description</label>
              <input
                type="description"
                className="form-control mt-1"
                placeholder="Description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </div>
            <div className="form-group mt-3">
              <label>City</label>
              <input
                type="city"
                className="form-control mt-1"
                placeholder="City"
                value={city}
                onChange={(e) => setCity(e.target.value)}
              />
            </div>
            <div className="form-group mt-3">
              <label>County</label>
              <input
                type="county"
                className="form-control mt-1"
                placeholder="County"
                value={county}
                onChange={(e) => setCounty(e.target.value)}
              />
            </div>
          <div className="d-grid gap-2 mt-3">
            <button type="submit" className="btn btn-primary" onClick={becomer}>
              Become a tradesman
            </button>
          </div>
        </div>
        </div>
      </form>
    </div>
    <ToastContainer />
    </div>
  )
}