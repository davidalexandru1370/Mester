import { useNavigate } from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import NavMenu from "./NavMenu";
import useToken from "./useToken";

export default function () {
  const { setToken } = useToken();

  let navigate = useNavigate();

  async function logout(event: { preventDefault: () => void }) {
    event.preventDefault();

    setToken("");

    navigate("/auth");
  }

  return (
    <div>
      <NavMenu />
      <div className="Auth-form-container">
        <form className="Auth-form">
          <div className="Auth-form-content">
            <h3 className="Auth-form-title">
              Are you sure you want to log out?
            </h3>
            <div className="d-grid gap-2 mt-3">
              <button
                type="submit"
                className="btn btn-primary"
                onClick={logout}
              >
                Log Out
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
