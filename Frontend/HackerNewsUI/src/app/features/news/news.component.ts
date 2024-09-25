import { Component, OnInit } from '@angular/core';
import { NewsService } from './news.service';
import { NewsResponse } from 'src/app/interfaces/newsResponse';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.css']
})
export class NewsComponent implements OnInit{
  newsList: any;
  currentPage: number;
  totalPages: number;
  paginationLegend: string;
  searchTerm: string;
  searchActive: boolean;
  currentSearchTerm: string;
  searchingMessage: string;
  noDataFound: boolean;
  isLoading: boolean;
  showFormErrors: boolean;
  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.getSearch(0);
  }

  loadPage(pageNumber: number){    
    this.getSearch(pageNumber);
  }

  handleSearchSubmit(){
    if(!!this.searchTerm){
      this.currentSearchTerm = this.searchTerm;
      this.getSearch(0);
    }
    else{
      this.showFormErrors = true;
    }
  }

  onCancelSearchClick(){
    this.searchActive = false;
    this.currentSearchTerm = "";
    this.searchTerm = "";
    this.showFormErrors = false;
    this.getSearch(0);
  }  

  getSearch(pageNumber: number){
    this.showFormErrors = false;
    this.isLoading = true;
    const searchTerm = !!this.currentSearchTerm?this.currentSearchTerm:null;
    this.newsService.getSearch(searchTerm, pageNumber,10).subscribe((response: NewsResponse)=>{
      if(!!this.currentSearchTerm){
        this.searchActive = true;
        this.searchingMessage = "Searching: " + this.currentSearchTerm;  
      }
      this.displayResults(pageNumber, response);
    });
  }

  displayResults(pageNumber: number, response: NewsResponse){
    this.currentPage = pageNumber;
    if(response.data && response.data.length){
      this.newsList = response.data;
      this.noDataFound = false;
    }
    else{
      this.newsList = null;
      this.noDataFound = true;
    }      
    this.totalPages = response.totalPages;
    this.paginationLegend = `Showing page ${this.currentPage+1} of ${this.totalPages}`;
    this.isLoading = false;
  }

  openLink(url: string){
    if(!!url){
      window.open(url, "_blank");
    }    
  }  
}
