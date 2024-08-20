import api from "./api";
import { Book, NewBook } from "../types/User";

export const getBooks = async (): Promise<Book[]> => {
  const response = await api.get<Book[]>("/books");
  return response.data;
};

export const getBook = async (id: number): Promise<Book> => {
  const response = await api.get<Book>(`/books/${id}`);
  return response.data;
};

export const createBook = async (newBook: NewBook): Promise<Book> => {
  const formData = new FormData();
  formData.append("title", newBook.title);
  formData.append("author", newBook.author);
  formData.append("isbn", newBook.isbn);
  formData.append("publicationYear", newBook.publicationYear.toString());
  if (newBook.imageFile) {
    formData.append("image", newBook.imageFile);
  }

  const response = await api.post<Book>("/books", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

export const updateBook = async (
  id: number,
  updatedBook: NewBook
): Promise<Book> => {
  const formData = new FormData();
  formData.append("title", updatedBook.title);
  formData.append("author", updatedBook.author);
  formData.append("isbn", updatedBook.isbn);
  formData.append("publicationYear", updatedBook.publicationYear.toString());
  if (updatedBook.imageFile) {
    formData.append("image", updatedBook.imageFile);
  }

  const response = await api.put<Book>(`/books/${id}`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

export const deleteBook = async (id: number): Promise<void> => {
  await api.delete(`/books/${id}`);
};
