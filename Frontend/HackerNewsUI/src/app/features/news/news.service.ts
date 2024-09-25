import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { NewsResponse } from "src/app/interfaces/newsResponse";
import { environment } from "src/environments/environment";

@Injectable()
export class NewsService{        
        private baseUrl = environment.baseUrl;
        constructor(private http: HttpClient){}

	getSearch(searchTerm: string, pageNumber: number, pageSize: number): Observable<NewsResponse> {
                const searchTermParam = !!searchTerm?`searchTerm=${searchTerm}&`:"";                
                const url = this.baseUrl + `Search?${searchTermParam}pageNumber=${pageNumber}&pageSize=${pageSize}`;
                return this.http.get<NewsResponse>(url);
	}    
    
}