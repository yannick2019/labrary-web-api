import api from "./api";
import { LoginUserDto, RegisterUserDto, User } from "../types/User";

export const login = async (credentials: LoginUserDto): Promise<string> => {
  const response = await api.post<{ token: string }>(
    "/auth/login",
    credentials
  );
  const { token } = response.data;
  localStorage.setItem("token", token);
  setAuthToken(token);
  return token;
};

export const register = async (user: RegisterUserDto): Promise<User> => {
  const response = await api.post<User>("/auth/register", user);
  return response.data;
};

export const logout = (): void => {
  localStorage.removeItem("token");
  setAuthToken(null);
};

export const getCurrentUser = async (): Promise<User | null> => {
  try {
    const response = await api.get<User>("/auth/me");
    return response.data;
  } catch (error) {
    console.error("Error fetching current user:", error);
    return null;
  }
};

export const setAuthToken = (token: string | null): void => {
  if (token) {
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common["Authorization"];
  }
};

// Initialiser le token au démarrage de l'application
const token = localStorage.getItem("token");
if (token) {
  setAuthToken(token);
}
