import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, finalize, Observable, of } from 'rxjs';
import { Story } from '../models/story.model';
import { LoaderService } from './loader.service';

@Injectable({
  providedIn: 'root',
})
export class HackerNewsService {
  private apiUrl = 'http://localhost:5119/api/stories';

  constructor(private http: HttpClient, private loaderService: LoaderService) {}

  getNewStories(
    page: number,
    pageSize: number,
    searchTerm: string = ''
  ): Observable<Story[]> {
    this.loaderService.show();

    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('search', searchTerm);
    }

    return this.http.get<Story[]>(`${this.apiUrl}/new`, { params }).pipe(
      finalize(() => this.loaderService.hide()), // Hide loader regardless of success/error
      catchError((err) => {
        console.error('Error fetching stories', err);
        return of([]); // Optional: fallback to empty list
      })
    );
  }
}
