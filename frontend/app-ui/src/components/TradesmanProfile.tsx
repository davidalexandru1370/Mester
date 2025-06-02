import { FC, useEffect, useState } from "react";
import axios from "axios";
import NavMenu from "./NavMenu";
import { ImageDTO } from "@/domain/types/ImageDTO";
import { useUser } from "@/context/UserContext";
import {
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  Form,
  FormGroup,
  Label,
  Input,
} from "reactstrap";
import { RequestDto } from "@/domain/types/requestDto";
import useToken from "./useToken";
import { toast } from "react-toastify";

interface Speciality {
  specialityId: string;
  tradesManId: string;
  type: string;
  imageUrl: string;
  price: number;
  unitOfMeasure: string;
}

interface TradesmanProfileData {
  id: string;
  name: string;
  description: string;
  city: string;
  county: string;
  imageUrl: string;
  specialities: Speciality[];
  images: ImageDTO[];
}

const TradesmanProfile: FC = () => {
  const tradesmanId: string = window.location.pathname.split("/").pop() || "";
  const [profile, setProfile] = useState<TradesmanProfileData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const { user } = useUser();
  const { token } = useToken();
  const [modalOpen, setModalOpen] = useState(false);
  const [requests, setRequests] = useState<RequestDto[]>([]);

  useEffect(() => {
    async function fetchProfile() {
      try {
        const response = await axios.get(
          `https://localhost:8081/api/TradesMan/${tradesmanId}`
        );
        setProfile(response.data);
      } catch (err) {
        setError("Eroare la incarcarea profilului.");
      } finally {
        setLoading(false);
      }
    }
    async function fetchRequests() {
      try {
        const response = await axios.get(
          `https://localhost:8081/api/requests`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        setRequests(response.data);
      } catch (err) {
        console.error("Eroare la incarcarea cererilor:", err);
      }
    }

    if (tradesmanId) {
      fetchProfile();
      fetchRequests();
    }
  }, [tradesmanId]);

  function handleOpenModal() {
    setModalOpen(true);
  }
  function handleCloseModal() {
    setModalOpen(false);
  }
  function handleSendRequest(requestId: string) {
    setModalOpen(false);
    if (!requestId) {
      setError("Selecteaza o cerere pentru a o trimite.");
      return;
    }
    axios
      .post(
        `https://localhost:8081/api/requests/${requestId}/send/tradesmen/${tradesmanId}`,
        {},
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      )
      .then(() => {
        alert("Cererea a fost trimisa cu succes!");
      })
      .catch((err) => {
        console.error("Eroare la trimiterea cererii:", err);
        toast.error("Eroare la trimiterea cererii.");
      });
  }

  if (loading) {
    return <div>Se incarca...</div>;
  }

  if (error) {
    return <div>{error}</div>;
  }

  if (!profile) {
    return <div>Niciun profil gasit.</div>;
  }

  return (
    <div>
      <NavMenu />
      <div className="container mt-4">
        {user?.id === tradesmanId ? (
          <></>
        ) : (
          <div>
            <div className="mb-4 d-flex justify-content-end">
              <Button color="primary" onClick={handleOpenModal}>
                Ofera o cerere acestui mester
              </Button>
            </div>
            <Modal isOpen={modalOpen} toggle={handleCloseModal}>
              <ModalHeader toggle={handleCloseModal}>
                Trimite o cerere
              </ModalHeader>
              <ModalBody>
                <Form>
                  {requests.map((request) => (
                    <div className="card">
                      <div className="card-body">
                        <h5 className="card-title">Cerere: {request.name}</h5>
                        <p className="card-text">{request.description}</p>
                        <Button
                          color="primary"
                          onClick={() => {
                            handleSendRequest(request.id);
                          }}
                        >
                          Trimite cererea
                        </Button>
                      </div>
                    </div>
                  ))}
                </Form>
              </ModalBody>
              <ModalFooter>
                <Button color="secondary" onClick={handleCloseModal}>
                  Anuleaza
                </Button>
              </ModalFooter>
            </Modal>
          </div>
        )}
        <div className="d-flex flex-column justify-content-between align-items-center mb-4">
          <img
            src={profile.imageUrl}
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
          <h2>{profile.name}</h2>
        </div>
        <p>{profile.description}</p>
        <p>
          <strong>Oras:</strong> {profile.city}
        </p>
        <p>
          <strong>Judet:</strong> {profile.county}
        </p>
        <div>
          <strong>Specialitati:</strong>
          <div className="row">
            {profile.specialities && profile.specialities.length > 0 ? (
              profile.specialities.map((spec) => (
                <div className="col-md-4 mb-3" key={spec.specialityId}>
                  <div className="card h-100">
                    <img
                      src={spec.imageUrl}
                      alt={spec.type}
                      className="card-img-top"
                      style={{ height: 180, objectFit: "contain" }}
                    />
                    <div className="card-body">
                      <h5 className="card-title">{spec.type}</h5>
                      <p className="card-text">
                        <strong>Pret:</strong> {spec.price} RON /
                        {spec.unitOfMeasure}
                      </p>
                    </div>
                  </div>
                </div>
              ))
            ) : (
              <div className="col-12">Nicio specialitate gasita.</div>
            )}
          </div>
        </div>
        <div>
          <p>
            <strong>Lucrari anterioare:</strong>
          </p>
          <div className="row mb-4">
            {profile.images && profile.images.length > 0 ? (
              profile.images.map((image, index) => (
                <div className="col-md-4 mb-3" key={index}>
                  <img
                    src={image.imageUrl}
                    alt={`Work ${index + 1}`}
                    className="img-fluid"
                    style={{ height: 300, width: 300, objectFit: "cover" }}
                  />
                </div>
              ))
            ) : (
              <div className="col-12">Nicio lucrare anterioara gasita.</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default TradesmanProfile;
