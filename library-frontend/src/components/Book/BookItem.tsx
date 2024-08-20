import React from "react";
import { Book } from "../../types/Book";

interface BookItemProps {
  book: Book;
  onEdit: (book: Book) => void;
  onDelete: (id: number) => void;
}

const BookItem: React.FC<BookItemProps> = ({ book, onEdit, onDelete }) => {
  return (
    <div className="book-item">
      <h3>{book.title}</h3>
      <p>Author: {book.author}</p>
      <p>ISBN: {book.isbn}</p>
      <p>Publication Year: {book.publicationYear}</p>
      {book.imageUrl && (
        <img
          src={book.imageUrl}
          alt={book.title}
          style={{ maxWidth: "100px" }}
        />
      )}
      <div>
        <button onClick={() => onEdit(book)}>Edit</button>
        <button onClick={() => onDelete(book.id)}>Delete</button>
      </div>
    </div>
  );
};

export default BookItem;
