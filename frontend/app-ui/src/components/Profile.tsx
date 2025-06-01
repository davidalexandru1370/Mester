import NavMenu from "./NavMenu"
import { ToastContainer, toast } from 'react-toastify';
import axios from "axios";
import useToken from "./useToken";
import { useEffect, useRef, useState } from "react";
import { UserDetailsDto } from "@/context/UserContext";
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

export default function () {
  
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [userDetails, setUserDetails] = useState<UserDetailsDto | null>(null);
    const [tradesmanShower, setTradesmanShower] = useState(false);
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

    async function handleImageChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) {
      const formData = new FormData();
      formData.append("image", file);

      try {
        const response = await axios.put(
          "https://localhost:8081/api/user/update-avatar",
          formData,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "multipart/form-data",
            },
          }
        );
        toast("Profile image updated successfully!", {
          type: "success",
        });
        setUserDetails({
          ...userDetails,
          imageUrl: response.data.imageUrl,
        } as UserDetailsDto);
      } catch (error) {
        let errorMessage = "Failed to upload image";
        if (error instanceof Error) {
          errorMessage = error.message;}
        toast(errorMessage);
      }
    }
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

  function handleUpdateImageClick() {
    fileInputRef.current?.click();
  }

  useEffect(() => {
    async function fetchUserDetails() {
      const response = await axios.get("https://localhost:8081/api/user/info", {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
          accept: "application/json",
        },
      });
      if (response.status === 200) {
        setUserDetails(response.data);
      } else {
        toast("Failed to fetch user details");
      }
    }

    fetchUserDetails();
  }, []);

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
      <NavMenu />
      <div className="Auth-form-container">
        <form className="Auth-form">
          <div className="Auth-form-content">
            {userDetails === null ? (
              <></>
            ) : (
              <Container>
                <Row>
                <Col xs={2}>
                <div
                  style={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    marginBottom: 20,
                  }}
                >
                  <img
                    src={
                      userDetails?.imageUrl &&
                      /^https?:\/\/[^\s]+$/.test(userDetails.imageUrl)
                        ? userDetails.imageUrl
                        : "https://via.placeholder.com/120x120.png?text=Profile"
                    }
                    alt="Profile"
                    style={{
                      width: 120,
                      height: 120,
                      borderRadius: "50%",
                      objectFit: "cover",
                      marginBottom: 10,
                      border: "2px solid #ccc",
                    }}
                  />
                  <input
                    type="file"
                    accept="image/*"
                    style={{ display: "none" }}
                    ref={fileInputRef}
                    onChange={handleImageChange}
                  />
                  <button
                    type="button"
                    className="btn btn-secondary mb-2"
                    onClick={handleUpdateImageClick}
                  >
                    Update Profile Image
                  </button>
                </div>
                </Col>
          {tradesmanShower === false ? (
              <Col>
                <h4>Do you want to become a tradesman?</h4>
                <br></br>
                <h5>Unlock the ability to show off your skills to potential customers and get paid for your work!</h5>
                <br></br>
                <button className="btn btn-warning" onClick={() => {setTradesmanShower(true)}}>
                START OFF TODAY BY COMPLETING THIS FORM
                </button>
              </Col>
            ) : (
          <Col xs={5}>
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
          </Col>
          )}
          {tradesmanShower === false ? (
              <></>
            ) : (
          <Col xs={5}>
          <h4 className="Auth-form-title">Specialities that can be added</h4>
          <Container>
            <Row>
          {specialities?.map(function fn(sp: any) {
                    return (
                        <Col>
                            <button type="button" className="btn btn-info" onClick={() => {setName(sp)}} style={{marginBottom: 10,}}>
                                {sp}
                            </button>
                        </Col>
                    );
                })}
            </Row>
          </Container>
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
          </Col>)}
          </Row>
          {tradesmanShower === false ? (
              <></>
            ) : (
          <Row>
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
            <button type="submit" className="btn btn-success" onClick={becomer}>
              Become a tradesman
            </button>
          </div>
              </div>
              </Row>)}
              </Container>
            )}
          </div>
        </form>
      </div>
      <ToastContainer />
    </div>
  );
}
