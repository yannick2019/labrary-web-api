export interface Book {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  imageUrl?: string;
  genreIds?: number[];
}

export interface NewBook {
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  imageFile?: File;
}
