import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import * as authService from "../../services/authService";

const Login: React.FC = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = await authService.login({ username, password });
      // Utilisation explicite du token
      if (token) {
        setIsLoggedIn(true);
        // Dans une application réelle, vous pourriez stocker le token dans un contexte global
        // ou l'utiliser pour faire des requêtes authentifiées immédiatement
        console.log("Token received:", token.slice(0, 10) + "..."); // Affiche les 10 premiers caractères du token
        setTimeout(() => navigate("/profile"), 1500); // Redirige après 1.5 secondes
      }
    } catch (err) {
      setError("Login failed. Please check your credentials.");
      console.error(err);
    }
  };

  if (isLoggedIn) {
    return <div>Login successful! Redirecting to profile...</div>;
  }

  return (
    <div>
      <h2>Login</h2>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Username:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Login</button>
      </form>
    </div>
  );
};

export default Login;
