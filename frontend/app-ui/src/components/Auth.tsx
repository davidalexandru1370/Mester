import { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import useToken from "./useToken";

export default function () {
  let [authMode, setAuthMode] = useState("signin");

  const changeAuthMode = () => {
    setAuthMode(authMode === "signin" ? "signup" : "signin");
  };

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");

  let currentId = 0;
  let navigate = useNavigate();

  const { token, setToken } = useToken();

  useEffect(() => {}, []);

  async function login(event: { preventDefault: () => void }) {
    event.preventDefault();
    try {
      await axios
        .post(
          "https://localhost:8081/api/user/login",
          {
            email: email,
            password: password,
          },
          {
            timeout: 5000,
            headers: {
              "Content-Type": "application/json",
              accept: "application/json", // If you receieve JSON response.
            },
          }
        )
        .then(function (response) {
          const access = response.data.jwt;
          setToken(access);
          console.log(response.data.jwt);
        });
      navigate("/main");
    } catch (err) {
      let errorMessage = "Failed to do something exceptional";
      if (err instanceof Error) {
        errorMessage = err.message;
      }
      toast(errorMessage);
    }
  }

  async function register(event: { preventDefault: () => void }) {
    event.preventDefault();
    try {
      await axios
        .post(
          "https://localhost:8081/api/user/createAccount",
          {
            email: email,
            password: password,
            phoneNumber: phoneNumber,
          },
          {
            timeout: 5000,
            headers: {
              "Content-Type": "application/json",
              accept: "application/json", // If you receieve JSON response.
            },
          }
        )
        .then(function (response) {
          currentId = response.data.success;
          console.log(response.data.success);
        });
      changeAuthMode();
    } catch (err) {
      let errorMessage = "Failed to do something exceptional";
      if (err instanceof Error) {
        errorMessage = err.message;
      }
      toast(errorMessage);
    }
  }

  if (authMode === "signin") {
    return (
      <div className="Auth-form-container">
        <form className="Auth-form">
          <div className="Auth-form-content">
            <h3 className="Auth-form-title">Sign In</h3>
            <div className="text-center">
              Not registered yet?
              <span className="link-primary" onClick={changeAuthMode}>
                Sign Up
              </span>
            </div>
            <div className="form-group mt-3">
              <label>Email address</label>
              <input
                type="email"
                className="form-control mt-1"
                placeholder="Email Address"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
            <div className="form-group mt-3">
              <label>Password</label>
              <input
                type="password"
                className="form-control mt-1"
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
            <div className="d-grid gap-2 mt-3">
              <button type="submit" className="btn btn-primary" onClick={login}>
                Submit
              </button>
            </div>
          </div>
        </form>
        <ToastContainer />
      </div>
    );
  }

  return (
    <div className="Auth-form-container">
      <form className="Auth-form">
        <div className="Auth-form-content">
          <h3 className="Auth-form-title">Sign Up</h3>
          <div className="text-center">
            Already registered?{" "}
            <span className="link-primary" onClick={changeAuthMode}>
              Sign In
            </span>
          </div>
          <div className="form-group mt-3">
            <label>Email address</label>
            <input
              type="email"
              className="form-control mt-1"
              placeholder="Email Address"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>
          <div className="form-group mt-3">
            <label>Password</label>
            <input
              type="password"
              className="form-control mt-1"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
          <div className="form-group mt-3">
            <label>Phone Number</label>
            <input
              type="last_name"
              className="form-control mt-1"
              placeholder="e.g Doe"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
            />
          </div>
          <div className="d-grid gap-2 mt-3">
            <button
              type="submit"
              className="btn btn-primary"
              onClick={register}
            >
              Submit
            </button>
          </div>
        </div>
      </form>
      <ToastContainer />
    </div>
  );
}
