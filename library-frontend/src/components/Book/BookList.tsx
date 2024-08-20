import React, { useState, useEffect } from "react";
import { Book, NewBook } from "../../types/Book";
import { getBooks, updateBook, deleteBook } from "../../services/bookService";
import BookForm from "./BookForm";

const BookList: React.FC = () => {
  const [books, setBooks] = useState<Book[]>([]);
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);

  useEffect(() => {
    fetchBooks();
  }, []);

  const fetchBooks = async () => {
    const fetchedBooks = await getBooks();
    setBooks(fetchedBooks);
  };

  // const handleAddBook = async (newBook: NewBook) => {
  //   const createdBook = await createBook(newBook);
  //   setBooks([...books, createdBook]);
  // };

  const handleUpdateBook = async (updatedBook: NewBook) => {
    if (selectedBook) {
      const updated = await updateBook(selectedBook.id, updatedBook);
      setBooks(books.map((book) => (book.id === updated.id ? updated : book)));
      setSelectedBook(null);
    }
  };

  const handleDeleteBook = async (id: number) => {
    await deleteBook(id);
    setBooks(books.filter((book) => book.id !== id));
  };

  return (
    <div>
      <h1>Book List</h1>
      {/* <BookForm onSubmit={handleAddBook} /> */}
      {books.map((book) => (
        <div key={book.id}>
          <h3>{book.title}</h3>
          <p>Author: {book.author}</p>
          <p>ISBN: {book.isbn}</p>
          <p>Year: {book.publicationYear}</p>
          {book.imageUrl && (
            <img
              src={book.imageUrl}
              alt={book.title}
              style={{ maxWidth: "100px" }}
            />
          )}
          <button onClick={() => setSelectedBook(book)}>Edit</button>
          <button onClick={() => handleDeleteBook(book.id)}>Delete</button>
        </div>
      ))}
      {selectedBook && (
        <div>
          <h2>Edit Book</h2>
          <BookForm book={selectedBook} onSubmit={handleUpdateBook} />
        </div>
      )}
    </div>
  );
};

export default BookList;
