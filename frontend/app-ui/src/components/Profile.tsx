import React, { useState } from "react"
import NavMenu from "./NavMenu"
import { ToastContainer, toast } from 'react-toastify';
import axios from "axios";
import useToken from './useToken';

export default function () {

    const { token, setToken } = useToken();

    async function becomer(event: { preventDefault: () => void; }) {

        event.preventDefault();
        try {
            await axios.post("https://localhost:8081/identity/createTradesManProfile", {
    
                specialties: [],
                description: "I trade.",
    
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

  return (
    <div>
      <NavMenu/>
    <div className="Auth-form-container">
      <form className="Auth-form">
        <div className="Auth-form-content">
          <h3 className="Auth-form-title">Update Profile</h3>
          <div className="d-grid gap-2 mt-3">
            <button type="submit" className="btn btn-primary" onClick={becomer}>
              Become a tradesman
            </button>
          </div>
        </div>
      </form>
    </div>
    <ToastContainer />
    </div>
  )
}