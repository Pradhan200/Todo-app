import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TodoService } from './todo.service';
import { TodoItem } from './models';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TodoService]
    }).compileComponents();
    
    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get todos', () => {
    const mockTodos: TodoItem[] = [
      {
        id: '1',
        title: 'Test Todo',
        createdAt: '2025-01-01T00:00:00.000Z',
        completed: false
      }
    ];

    service.getTodos().subscribe((todos: TodoItem[]) => {
      expect(todos).toEqual(mockTodos);
    });

    const req = httpMock.expectOne('http://localhost:5000/api/todo');
    expect(req.request.method).toBe('GET');
    req.flush(mockTodos);
  });

  it('should add a todo', () => {
    const newTodo: TodoItem = {
      id: '2',
      title: 'New Todo',
      createdAt: '2025-01-01T00:00:00.000Z',
      completed: false
    };

    service.addTodo('New Todo').subscribe((todo: TodoItem) => {
      expect(todo).toEqual(newTodo);
    });

    const req = httpMock.expectOne('http://localhost:5000/api/todo');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'New Todo' });
    req.flush(newTodo);
  });

  it('should delete a todo', () => {
    service.deleteTodo('1').subscribe((response: void) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne('http://localhost:5000/api/todo/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should handle HTTP errors', () => {
    service.getTodos().subscribe({
      next: () => {
        fail('should have failed');
      },
      error: (error: string) => {
        expect(error).toContain('Error Code: 500');
      }
    });

    const req = httpMock.expectOne('http://localhost:5000/api/todo');
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
  });
});
