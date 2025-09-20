import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TodoItem } from './models';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly apiUrl = 'http://localhost:5000/api/todo';

  constructor(private http: HttpClient) { }

  getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  addTodo(title: string): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.apiUrl, { title })
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteTodo(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  toggleTodoComplete(id: string): Observable<TodoItem> {
    return this.http.patch<TodoItem>(`${this.apiUrl}/${id}/toggle`, {})
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error(errorMessage);
    return throwError(() => errorMessage);
  }
}
