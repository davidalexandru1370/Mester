import {
  Navbar,
  NavbarBrand,
  NavItem,
  NavLink,
  Dropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import { useState } from "react";
import { useUser } from "@/context/UserContext";

export default function NavMenu() {
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const toggle = () => setDropdownOpen((prevState) => !prevState);
  const { user } = useUser();

  return (
    <header>
      <Navbar
        className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3 d-flex justify-content-between align-items-center"
        container
        light
      >
        <NavbarBrand tag={Link} to="/main">
          Mester
        </NavbarBrand>
        <ul className="navbar-nav flex-grow">
          <NavItem>
            <NavLink tag={Link} className="text-dark" to="/main">
              Main
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink tag={Link} className="text-dark" to="/user">
              Profile
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink tag={Link} className="text-dark" to="/logout">
              Logout
            </NavLink>
          </NavItem>
          <NavItem>
            <NavLink tag={Link} className="text-dark" to="/conversations">Conversations</NavLink>
          </NavItem>
        </ul>
        <div className="ms-auto">
          <Dropdown isOpen={dropdownOpen} toggle={toggle} direction="down">
            <DropdownToggle caret color="secondary">
              Hello, {user?.name}
            </DropdownToggle>
            <DropdownMenu end>
              <DropdownItem tag={Link} to="/user">
                My Account
              </DropdownItem>
              <DropdownItem tag={Link} to="/logout">
                Logout
              </DropdownItem>
            </DropdownMenu>
          </Dropdown>
        </div>

      </Navbar>
    </header>
  );
}
