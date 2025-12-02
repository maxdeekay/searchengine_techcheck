import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SearchRequest, EngineResult } from './search.models';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiUrl = 'https://localhost:7262/api/search';

  constructor(private http: HttpClient) { }

  search(query: string, engine: string): Observable<EngineResult> {
    const body: SearchRequest = { query, engine };
    return this.http.post<EngineResult>(this.apiUrl, body);
  }
}
