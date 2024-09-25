import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NewsComponent } from './news.component';
import { NewsService } from './news.service';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';

describe('NewsComponent', () => {
  let component: NewsComponent;
  let fixture: ComponentFixture<NewsComponent>;
  
  var newsService = new NewsService(null);

  beforeEach(() => {    
    newsService.getSearch = jasmine.createSpy().and.returnValue(of(mockedNews));
    TestBed.configureTestingModule({
      declarations: [NewsComponent],
      imports: [FormsModule],
      providers: [{
        provide: NewsService, useValue: newsService
      }],
    });
    fixture = TestBed.createComponent(NewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get latest on init', () => {
    expect(newsService.getSearch).toHaveBeenCalled();
    const compiled = fixture.nativeElement as HTMLElement;
    const tbody = compiled.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    expect(rows.length).toBe(10);
    const span = rows[0].querySelector('span');
    expect(span.textContent).toBe("New Version of Angular");
  });  

  it('should display pagination after load on init', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const pagination = compiled.querySelector('.pagination-section');
    expect(pagination).toBeTruthy();
  });    

  it('should request data after click on Next', () => {
    expect(newsService.getSearch).toHaveBeenCalledWith(null,0,10);
    expect(component.currentPage).toBe(0);
    newsService.getSearch = jasmine.createSpy().and.returnValue(of(mockedNewsPage2));
    const compiled = fixture.nativeElement as HTMLElement;
    const next = compiled.querySelector('#nextPage');    
    next.dispatchEvent(new Event('click'));
    fixture.detectChanges();
    expect(newsService.getSearch).toHaveBeenCalledWith(null,1,10);
    expect(component.currentPage).toBe(1);
    const tbody = compiled.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    expect(rows.length).toBe(2);
  });  

  it('should request data after click on Last', () => {
    expect(newsService.getSearch).toHaveBeenCalledWith(null,0,10);
    expect(component.currentPage).toBe(0);
    newsService.getSearch = jasmine.createSpy().and.returnValue(of(mockedNewsPage2));
    const compiled = fixture.nativeElement as HTMLElement;
    const next = compiled.querySelector('#lastPage');    
    next.dispatchEvent(new Event('click'));
    fixture.detectChanges();
    expect(newsService.getSearch).toHaveBeenCalledWith(null,1,10);
    expect(component.currentPage).toBe(1);
    const tbody = compiled.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    expect(rows.length).toBe(2);
  });  
  
  it('should search after Search Form submit', () => {
    component.searchTerm = 'Data';    
    const compiled = fixture.nativeElement as HTMLElement;
    const searchForm = compiled.querySelector('#searchForm');    
    searchForm.dispatchEvent(new Event('submit'));
    fixture.detectChanges();
    expect(newsService.getSearch).toHaveBeenCalledWith('Data',0,10);
    expect(component.currentPage).toBe(0);
    expect(component.searchActive).toBeTrue();
    const tbody = compiled.querySelector('tbody');
    const rows = tbody.querySelectorAll('tr');
    expect(rows.length).toBe(10);
  });

  it('should display "No data found" on empty Search', () => {
    newsService.getSearch = jasmine.createSpy().and.returnValue(of(mockedEmptySearch));
    component.searchTerm = 'Data';
    const compiled = fixture.nativeElement as HTMLElement;
    const searchForm = compiled.querySelector('#searchForm');    
    searchForm.dispatchEvent(new Event('submit'));
    fixture.detectChanges();
    expect(newsService.getSearch).toHaveBeenCalledWith('Data',0,10);
    const noData = compiled.querySelector('#noData');
    const tbody = compiled.querySelector('tbody');
    const pagination = compiled.querySelector('.pagination-section');    
    expect(noData).toBeTruthy();
    expect(tbody).toBeFalsy();
    expect(pagination).toBeFalsy();
  });   
});

const mockedNews = {
  data: [{
      id: 1,
      title: "New Version of Angular",
      url: "http://myurl.com/news"
    },
    {
      id: 2,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 3,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 4,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 5,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 6,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 7,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 8,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 9,
      title: "Title",
      url: "http://myurl.com/news"
    },
    {
      id: 10,
      title: "Title",
      url: "http://myurl.com/news"
    }],
  totalPages: 2
};

const mockedNewsPage2 = {
  data: [{
      id: 11,
      title: "Netcore Essentials",
      url: "http://myurl.com/news"
    },
    {
      id: 12,
      title: "What's new in Netcore",
      url: "http://myurl.com/news"
    }],
  totalPages: 2
};

const mockedEmptySearch = {
  data: [],
  totalPages: 0
};