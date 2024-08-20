import React, { useState, useEffect } from "react";
import { Genre, NewGenre } from "../../types/Genre";
import {
  getGenres,
  createGenre,
  updateGenre,
  deleteGenre,
} from "../../services/genreService";
import GenreItem from "./GenreItem";
import { genreSchema } from "../../validation/genreSchema";
import { ValidationError } from "yup";

const GenreList: React.FC = () => {
  const [genres, setGenres] = useState<Genre[]>([]);
  const [selectedGenre, setSelectedGenre] = useState<Genre | null>(null);
  const [newGenre, setNewGenre] = useState<NewGenre>({
    name: "",
    description: "",
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [isValid, setIsValid] = useState(false);

  useEffect(() => {
    fetchGenres();
  }, []);

  const fetchGenres = async () => {
    const fetchedGenres = await getGenres();
    setGenres(fetchedGenres);
  };

  const validateGenre = async (genre: NewGenre) => {
    try {
      await genreSchema.validate(genre, { abortEarly: false });
      setErrors({});
      setIsValid(true);
      return true;
    } catch (error) {
      if (error instanceof ValidationError) {
        const newErrors: { [key: string]: string } = {};
        error.inner.forEach((err) => {
          if (err.path) {
            newErrors[err.path] = err.message;
          }
        });
        setErrors(newErrors);
      }
      setIsValid(false);
      return false;
    }
  };

  const handleCreateGenre = async (e: React.FormEvent) => {
    e.preventDefault();
    if (await validateGenre(newGenre)) {
      try {
        const createdGenre = await createGenre(newGenre);
        setGenres([...genres, createdGenre]);
        setNewGenre({ name: "", description: "" });
        setIsValid(false);
      } catch (error) {
        console.error("Error creating genre:", error);
      }
    }
  };

  const handleUpdateGenre = async (updatedGenre: NewGenre) => {
    if (selectedGenre && (await validateGenre(updatedGenre))) {
      try {
        const updated = await updateGenre(selectedGenre.id, updatedGenre);
        setGenres(
          genres.map((genre) => (genre.id === updated.id ? updated : genre))
        );
        setSelectedGenre(null);
      } catch (error) {
        console.error("Error updating genre:", error);
      }
    }
  };

  const handleDeleteGenre = async (id: number) => {
    await deleteGenre(id);
    setGenres(genres.filter((genre) => genre.id !== id));
  };

  return (
    <div>
      <h2>Genres</h2>
      <form onSubmit={handleCreateGenre}>
        <input
          type="text"
          value={newGenre.name}
          onChange={(e) => setNewGenre({ ...newGenre, name: e.target.value })}
          placeholder="Genre Name"
          required
        />
        {errors.name && <p className="error">{errors.name}</p>}
        <input
          type="text"
          value={newGenre.description}
          onChange={(e) =>
            setNewGenre({ ...newGenre, description: e.target.value })
          }
          placeholder="Description"
          required
        />
        {errors.description && <p className="error">{errors.description}</p>}
        <button type="submit" disabled={!isValid}>
          Add Genre
        </button>
      </form>
      {genres.map((genre) => (
        <GenreItem
          key={genre.id}
          genre={genre}
          onEdit={setSelectedGenre}
          onDelete={handleDeleteGenre}
        />
      ))}
      {selectedGenre && (
        <div>
          <h3>Edit Genre</h3>
          <form
            onSubmit={(e) => {
              e.preventDefault();
              handleUpdateGenre({
                name: selectedGenre.name,
                description: selectedGenre.description,
              });
            }}
          >
            <input
              type="text"
              value={selectedGenre.name}
              onChange={(e) =>
                setSelectedGenre({ ...selectedGenre, name: e.target.value })
              }
              required
            />
            {errors.name && <p className="error">{errors.name}</p>}
            <textarea
              value={selectedGenre.description}
              onChange={(e) =>
                setSelectedGenre({
                  ...selectedGenre,
                  description: e.target.value,
                })
              }
            />
            {errors.description && (
              <p className="error">{errors.description}</p>
            )}
            <button type="submit">Update Genre</button>
            <button onClick={() => setSelectedGenre(null)}>Cancel</button>
          </form>
        </div>
      )}
    </div>
  );
};

export default GenreList;
