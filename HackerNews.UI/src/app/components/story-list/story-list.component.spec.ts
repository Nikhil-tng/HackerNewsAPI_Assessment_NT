import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { HackerNewsService } from '../../services/hacker-news.service';

describe('HackerNewsService', () => {
  let service: HackerNewsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [HackerNewsService],
    });
    service = TestBed.inject(HackerNewsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should retrieve new stories', () => {
    const mockStories = [
      {
        id: 1,
        title: 'Test Story',
        score: 100,
        by: 'testuser',
        time: 1234567890,
        type: 'story',  
        url: 'https://example.com',
        descendants: 0
      },
    ];

    service.getNewStories(1, 10).subscribe((stories) => {
      expect(stories.length).toBe(1);
      expect(stories).toEqual(mockStories);
    });

    const req = httpMock.expectOne(
      'http://localhost:5119/api/stories/new?page=1&pageSize=10'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);
  });

  afterEach(() => {
    httpMock.verify();
  });
});
