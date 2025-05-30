import NavMenu from "./NavMenu";
import { ToastContainer, toast } from "react-toastify";
import axios from "axios";
import useToken from "./useToken";
import { useEffect, useRef, useState } from "react";
import { UserDetailsDto } from "@/context/UserContext";

export default function () {
  const { token } = useToken();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [userDetails, setUserDetails] = useState<UserDetailsDto | null>(null);

  async function becomer(event: { preventDefault: () => void }) {
    event.preventDefault();
    try {
      await axios
        .post(
          "https://localhost:8081/identity/createTradesManProfile",
          {
            specialties: [],
            description: "I trade.",
          },
          {
            timeout: 5000,
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "application/json",
              accept: "application/json", // If you receieve JSON response.
            },
          }
        )
        .then(function () {
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
        toast("Profile image updated successfully!");
      } catch (error) {
        let errorMessage = "Failed to upload image";
        if (error instanceof Error) {
          errorMessage = error.message;
        }
        toast(errorMessage);
      }
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

  return (
    <div>
      <NavMenu />
      <div className="Auth-form-container">
        <form className="Auth-form">
          <div className="Auth-form-content">
            {userDetails === null ? (
              <></>
            ) : (
              <div>
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
                      userDetails?.imageUrl ||
                      "https://via.placeholder.com/120x120.png?text=Profile"
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
                <h3 className="Auth-form-title">Update Profile</h3>
                <div className="d-grid gap-2 mt-3">
                  <button
                    type="submit"
                    className="btn btn-primary"
                    onClick={becomer}
                  >
                    Become a tradesman
                  </button>
                </div>
              </div>
            )}
          </div>
        </form>
      </div>
      <ToastContainer />
    </div>
  );
}
