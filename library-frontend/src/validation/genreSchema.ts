import * as Yup from "yup";

export const genreSchema = Yup.object().shape({
  name: Yup.string()
    .required("Name is required")
    .min(2, "The name must contain at least 2 characters")
    .max(50, "The name cannot exceed 50 characters"),
  description: Yup.string()
    .required("Description is required")
    .min(10, "The description must contain at least 10 characters")
    .max(500, "The description cannot exceed 500 characters"),
});
