import { NewsItem } from "./NewsItem";

export interface NewsResponse {
    data: NewsItem[];
    currentPage: number;
    totalPages: number;
}