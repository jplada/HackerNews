# HackerNews Latest News Application 

## Backend

Created with .Net 8.
The App has 2 endpoints:
Latest: Gets the latest news with paging
Search: Searchs within the latest news with paging

All latest news items are loaded on start up and stored in cache, to speed up queries.
On client requests the latest list is retrieved from HackerNews API, and items get from cache.
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

Backend: 10 hours
Frontend: 14 hours