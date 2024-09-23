import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { NewsResponse } from "src/app/interfaces/newsResponse";
import { environment } from "src/environments/environment";

@Injectable()
export class NewsService{        
        private baseUrl = environment.baseUrl;
        constructor(private http: HttpClient){}

        getLatest(pageNumber: number, pageSize: number): Observable<NewsResponse> {
                const url = this.baseUrl + `Latest?pageNumber=${pageNumber}&pageSize=${pageSize}`;
                return this.http.get<NewsResponse>(url);
	}

	getSearch(searchTerm: string, pageNumber: number, pageSize: number): Observable<NewsResponse> {
                const url = this.baseUrl + `Search?searchTerm=${searchTerm}&pageNumber=${pageNumber}&pageSize=${pageSize}`;
                return this.http.get<NewsResponse>(url);
	}    
    
}