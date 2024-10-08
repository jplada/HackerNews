import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NewsService } from './news.service';
import { environment } from 'src/environments/environment';
import { NewsResponse } from 'src/app/interfaces/newsResponse';

describe('NewsService', () => {
  let service: NewsService;
  let httpController: HttpTestingController;

  const mockedNews = {
    data: [{
        id: 1,
        title: "Title 1",
        url: "http://myurl.com/news"
      },
      {
        id: 2,
        title: "Title 2",
        url: "http://myurl.com/news"
      }],
    totalPages: 1
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [NewsService]
    });
    service = TestBed.inject(NewsService);
    httpController = TestBed.inject(HttpTestingController);
  });

  it('should create', () => {
    expect(service).toBeTruthy();
  });

  it('should search news', () => {
    service.getSearch("title", 0, 20).subscribe((response: NewsResponse) => {
        expect(response.data).toBeTruthy();
        expect(response.data.length).toBe(2);
        expect(response.data[0].title).toBe("Title 1");
    });
    const url = environment.baseUrl + 'Search?searchTerm=title&pageNumber=0&pageSize=20';
    const mockReq = httpController.expectOne(url);
    expect(mockReq.request.method).toEqual('GET');
    mockReq.flush(mockedNews);
  });

  it('should search news with empty search term', () => {
    service.getSearch("", 0, 20).subscribe((response: NewsResponse) => {
        expect(response.data).toBeTruthy();
        expect(response.data.length).toBe(2);
        expect(response.data[0].title).toBe("Title 1");
    });
    const url = environment.baseUrl + 'Search?pageNumber=0&pageSize=20';
    const mockReq = httpController.expectOne(url);
    expect(mockReq.request.method).toEqual('GET');
    mockReq.flush(mockedNews);
  });

  afterEach(() => {
    httpController.verify();
  });
});
