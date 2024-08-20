import api from "./api";
import { Genre, NewGenre } from "../types/Genre";

export const getGenres = async (): Promise<Genre[]> => {
  const response = await api.get<Genre[]>("/genres");
  return response.data;
};

export const getGenre = async (id: number): Promise<Genre> => {
  const response = await api.get<Genre>(`/genres/${id}`);
  return response.data;
};

export const createGenre = async (newGenre: NewGenre): Promise<Genre> => {
  const response = await api.post<Genre>("/genres", newGenre);
  return response.data;
};

export const updateGenre = async (
  id: number,
  updatedGenre: NewGenre
): Promise<Genre> => {
  const response = await api.put<Genre>(`/genres/${id}`, updatedGenre);
  return response.data;
};

export const deleteGenre = async (id: number): Promise<void> => {
  await api.delete(`/genres/${id}`);
};
