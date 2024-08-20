import React from "react";
import { Genre } from "../../types/Genre";

interface GenreItemProps {
  genre: Genre;
  onEdit: (genre: Genre) => void;
  onDelete: (id: number) => void;
}

const GenreItem: React.FC<GenreItemProps> = ({ genre, onEdit, onDelete }) => {
  return (
    <div className="genre-item">
      <h3>{genre.name}</h3>
      <p>{genre.description}</p>
      <button onClick={() => onEdit(genre)}>Edit</button>
      <button onClick={() => onDelete(genre.id)}>Delete</button>
    </div>
  );
};

export default GenreItem;
