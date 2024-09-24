import { NewsItem } from "./newsItem";

export interface NewsResponse {
    data: NewsItem[];
    currentPage: number;
    totalPages: number;
}