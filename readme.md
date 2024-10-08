# HackerNews Latest News Application 

## Backend

Created with .Net 8.  
The App has one endpoint to search within latest news with paging:  
api/News/Search?searchTerm=term&pageNumber=0&pageSize=20  
The search term is searched in the items name.  
When the search term is empty all latest items are returned.  

All latest news items are loaded on start up and stored in cache, to speed up queries.  
On call of the Search endpoint the latest list of ids is retrieved from HackerNews API, and items are get from cache.  
Items that are not available in cache are retrieved from API.  

## FrontEnd

Created with Angular v16.  
On app start it loads the latest news from backend.  
The Search button allows to search for stories.  

## Run Backend

Must have .Net 8 installed.  
Open solution Backend\HackerNews.API\HackerNews.API.sln from visual studio.  
Start solution.  

## Run Frontend

Start backend.  
If Backend is not running in https://localhost:7111/ change file src\environments\environment.development.ts.  
To run Frontend app go to Frontend\HackerNewsUI, run commands npm install and then ng serve.  
Navigate in browser to http://localhost:4200/  

## Hours worked

Backend: 13 hours  
Frontend: 15 hours  