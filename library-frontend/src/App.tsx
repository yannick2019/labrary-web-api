import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import BookList from "./components/Book/BookList";
import Login from "./components/Auth/Login";
import Register from "./components/Auth/Register";
import Header from "./components/common/Header";

const App: React.FC = () => {
  return (
    <Router>
      <div className="App">
        <Header />
        <main>
          <Routes>
            <Route path="/" element={<BookList />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
          </Routes>
        </main>
        <footer>
          <p>&copy; 2024 Library Management</p>
        </footer>
      </div>
    </Router>
  );
};

export default App;
