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
  constructor(private newsService: NewsService) {}

  ngOnInit(): void {
    this.getLatest(0);
  }

  loadPage(pageNumber: number){    
    if(this.searchActive){
      this.getSearch(pageNumber);
    }
    else {
      this.getLatest(pageNumber);
    }
  }

  getLatest(pageNumber: number){
    this.isLoading = true;
    this.newsService.getLatest(pageNumber,10).subscribe((response: NewsResponse)=>{
      this.displayResults(pageNumber, response);      
    });
  }

  handleSearchSubmit(){
    if(!!this.searchTerm){
      this.currentSearchTerm = this.searchTerm;
      this.getSearch(0);
    }
  }

  onCancelSearchClick(){
    this.searchActive = false;
    this.currentSearchTerm = "";
    this.searchTerm = "";
    this.getLatest(0);
  }  

  getSearch(pageNumber: number){
    this.isLoading = true;
    this.newsService.getSearch(this.currentSearchTerm, pageNumber,10).subscribe((response: NewsResponse)=>{
      this.searchActive = true;
      this.searchingMessage = "Searching: " + this.currentSearchTerm;
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
