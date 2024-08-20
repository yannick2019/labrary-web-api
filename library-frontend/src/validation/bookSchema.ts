import * as Yup from "yup";

export const bookSchema = Yup.object().shape({
  title: Yup.string()
    .required("Title is required")
    .min(3, "The title must contain at least 3 characters")
    .max(100, "The title cannot exceed 100 characters"),
  author: Yup.string()
    .required("Author is required")
    .min(3, "The author's name must contain at least 3 characters.")
    .max(50, "Author's name cannot exceed 50 characters"),
  isbn: Yup.string()
    .required("ISBN is required")
    .matches(/^(?:\d{10}|\d{13})$/, "ISBN must be 10 or 13 digits long"),
  publicationYear: Yup.number()
    .required("The year of publication is required")
    .min(1000, "Year of publication must be greater than 1000")
    .max(
      new Date().getFullYear(),
      "Year of publication cannot be in the future"
    ),
});
