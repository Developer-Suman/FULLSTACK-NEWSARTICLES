interface NavBarProps {
  brandName: string;
  imageSrcPath: string;
  navItems: string[];
}
import { useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";

const NavBar = ({ brandName, imageSrcPath, navItems }: NavBarProps) => {

    const [selectedIndex, setSelectedIndex] = useState(-1);
  return (
    <nav className="navbar navbar-expand-md navbar-light bg-white shadow">
      <div className="container-fluid">
        <a className="navbar-brand" href="#">
          <img
            src={imageSrcPath}
            width="60"
            height="60"
            className="d-inline-block"
            alt=""
          />
          <span className="fw-bolder fs-4">{brandName}</span>
        </a>

        <button
          className="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse align-items-start d-flex flex-column flex-md-row" id="navbarSupportedContent">
    <ul className="navbar-nav me-auto mb-2 mb-md-1">
      {navItems.map((items, index)=>(
        <li key={items}
         className="nav-item active"
        onClick={()=> selectedIndex(index)}
        >
        <a className="nav-link" href="#">
            {items}
        </a>
      </li>
      ))}
      <li className="nav-item">
        <a className="nav-link" href="#">Link</a>
      </li>
      <li className="nav-item dropdown">
        <a className="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Dropdown
        </a>
        <div className="dropdown-menu" aria-labelledby="navbarDropdown">
          <a className="dropdown-item" href="#">Action</a>
          <a className="dropdown-item" href="#">Another action</a>
          <div className="dropdown-divider"></div>
          <a className="dropdown-item" href="#">Something else here</a>
        </div>
      </li>
      <li className="nav-item">
        <a className="nav-link disabled" href="#">Disabled</a>
      </li>
    </ul>
    <form className="d-flex me-2" role="search">
      <input className="form-control me-2" type="search" placeholder="Search" aria-label="Search"/>
      <button className="btn btn-outline-success" type="submit">Search</button>
    </form>
  </div>



      </div>
    </nav>
  );
};

export default NavBar;
