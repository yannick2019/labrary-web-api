import React, { useEffect, useState } from "react";
import { Book, NewBook } from "../../types/Book";
import { bookSchema } from "../../validation/bookSchema";
import { ValidationError } from "yup";

interface BookFormProps {
  book?: Book;
  onSubmit: (book: NewBook) => void;
}

const BookForm: React.FC<BookFormProps> = ({ book, onSubmit }) => {
  const [formData, setFormData] = useState<NewBook>({
    title: book?.title || "",
    author: book?.author || "",
    isbn: book?.isbn || "",
    publicationYear: book?.publicationYear || new Date().getFullYear(),
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [isValid, setIsValid] = useState(false);

  const validateForm = async (data: NewBook) => {
    try {
      await bookSchema.validate(data, { abortEarly: false });
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    const updatedData = { ...formData, [name]: value };
    setFormData(updatedData);
    validateForm(updatedData);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (await validateForm(formData)) {
      onSubmit(formData);
    }
  };

  useEffect(() => {
    validateForm(formData);
  }, [formData]);

  return (
    <form onSubmit={handleSubmit}>
      <h2>Add new book</h2>
      <div>
        <input
          type="text"
          name="title"
          value={formData.title}
          onChange={handleChange}
          placeholder="Title"
        />
        {errors.title && <p className="error">{errors.title}</p>}
      </div>
      <div>
        <input
          type="text"
          name="author"
          value={formData.author}
          onChange={handleChange}
          placeholder="Author"
        />
        {errors.author && <p className="error">{errors.author}</p>}
      </div>
      <div>
        <input
          type="text"
          name="isbn"
          value={formData.isbn}
          onChange={handleChange}
          placeholder="ISBN"
        />
        {errors.isbn && <p className="error">{errors.isbn}</p>}
      </div>
      <div>
        <input
          type="number"
          name="publicationYear"
          value={formData.publicationYear}
          onChange={handleChange}
        />
        {errors.publicationYear && (
          <p className="error">{errors.publicationYear}</p>
        )}
      </div>
      <button type="submit" disabled={!isValid}>
        {book ? "Update Book" : "Add Book"}
      </button>
    </form>
  );
};

export default BookForm;
