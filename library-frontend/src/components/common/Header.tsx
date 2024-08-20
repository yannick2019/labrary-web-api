import { Link } from "react-router-dom";

const Header: React.FC = () => {
  return (
    <>
      <header>
        <h1>Library Management</h1>
        <nav>
          <ul>
            <li>
              <Link to="/">Home</Link>
            </li>
            <li>
              <Link to="/register">Register</Link>
            </li>
            <li>
              <Link to="/login">Login</Link>
            </li>
            <li>
              <Link to="/profile">Profile</Link>
            </li>
          </ul>
        </nav>
      </header>
    </>
  );
};

export default Header;
