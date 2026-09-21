import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, QueryParams } from '../models/common.model';

/**
 * Thin generic REST client. Each feature service extends this with its own
 * resource path, so the paging/filtering query-string logic lives in one place.
 */
@Injectable({ providedIn: 'root' })
export abstract class ApiService<T, TCreate = Partial<T>> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly resource: string; // e.g. "contacts"

  private get baseUrl(): string {
    return `${environment.apiBaseUrl}/${this.resource}`;
  }

  /** Build HttpParams from an arbitrary query object, skipping null/undefined. */
  protected toParams(query: object = {}): HttpParams {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(query)) {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    }
    return params;
  }

  getPaged<Q extends QueryParams>(query: Q = {} as Q): Observable<PagedResult<T>> {
    return this.http.get<PagedResult<T>>(this.baseUrl, { params: this.toParams(query) });
  }

  getById(id: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${id}`);
  }

  create(dto: TCreate): Observable<T> {
    return this.http.post<T>(this.baseUrl, dto);
  }

  update(id: string, dto: Partial<T>): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
