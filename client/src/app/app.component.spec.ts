import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { TodoService } from './todo.service';
import { of, throwError } from 'rxjs';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let todoService: jasmine.SpyObj<TodoService>;

  const mockTodos = [
    {
      id: '1',
      title: 'Test Todo 1',
      createdAt: '2025-01-01T00:00:00.000Z',
      completed: false
    },
    {
      id: '2',
      title: 'Test Todo 2',
      createdAt: '2025-01-01T00:00:00.000Z',
      completed: true,
      completedAt: '2025-01-01T01:00:00.000Z'
    }
  ];

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('TodoService', ['getTodos', 'addTodo', 'deleteTodo', 'toggleTodoComplete']);

    await TestBed.configureTestingModule({
      imports: [AppComponent, HttpClientTestingModule],
      providers: [
        { provide: TodoService, useValue: spy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    todoService = TestBed.inject(TodoService) as jasmine.SpyObj<TodoService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load todos on init', () => {
    // Arrange
    todoService.getTodos.and.returnValue(of(mockTodos));

    // Act
    component.ngOnInit();

    // Assert
    expect(todoService.getTodos).toHaveBeenCalled();
    expect(component.todos).toEqual(mockTodos);
  });

  it('should handle error when loading todos', () => {
    // Arrange
    const errorMessage = 'Failed to load todos';
    todoService.getTodos.and.returnValue(throwError(() => errorMessage));

    // Act
    component.ngOnInit();

    // Assert
    expect(component.errorMessage).toBe(errorMessage);
    expect(component.isLoading).toBeFalse();
  });

  it('should add a new todo', () => {
    // Arrange
    const newTodo = { ...mockTodos[0], title: 'New Todo' };
    component.newTodoTitle = 'New Todo';
    todoService.addTodo.and.returnValue(of(newTodo));

    // Act
    component.addTodo();

    // Assert
    expect(todoService.addTodo).toHaveBeenCalledWith('New Todo');
    expect(component.todos).toContain(newTodo);
    expect(component.newTodoTitle).toBe('');
  });

  it('should handle error when adding todo', () => {
    // Arrange
    const errorMessage = 'Failed to add todo';
    component.newTodoTitle = 'Test Todo';
    todoService.addTodo.and.returnValue(throwError(() => errorMessage));

    // Act
    component.addTodo();

    // Assert
    expect(component.errorMessage).toBe(errorMessage);
    expect(component.isLoading).toBeFalse();
  });

  it('should toggle todo completion', () => {
    // Arrange
    const todoId = '1';
    const updatedTodo = { ...mockTodos[0], completed: true };
    component.todos = [mockTodos[0]];
    todoService.toggleTodoComplete.and.returnValue(of(updatedTodo));

    // Act
    component.toggleTodoComplete(todoId);

    // Assert
    expect(todoService.toggleTodoComplete).toHaveBeenCalledWith(todoId);
    expect(component.todos[0]).toEqual(updatedTodo);
  });

  it('should delete a todo', () => {
    // Arrange
    const todoId = '1';
    component.todos = [mockTodos[0]];
    todoService.deleteTodo.and.returnValue(of(void 0));

    // Act
    component.deleteTodo(todoId);

    // Assert
    expect(todoService.deleteTodo).toHaveBeenCalledWith(todoId);
    expect(component.todos).toHaveLength(0);
  });

  it('should filter active todos', () => {
    // Arrange
    component.todos = mockTodos;

    // Act
    const activeTodos = component.activeTodos;

    // Assert
    expect(activeTodos).toHaveLength(1);
    expect(activeTodos[0].completed).toBeFalse();
  });

  it('should filter completed todos', () => {
    // Arrange
    component.todos = mockTodos;

    // Act
    const completedTodos = component.completedTodos;

    // Assert
    expect(completedTodos).toHaveLength(1);
    expect(completedTodos[0].completed).toBeTrue();
  });

  it('should toggle show completed', () => {
    // Arrange
    expect(component.showCompleted).toBeFalse();

    // Act
    component.toggleShowCompleted();

    // Assert
    expect(component.showCompleted).toBeTrue();
  });

  it('should format date correctly', () => {
    // Arrange
    const dateString = '2025-01-01T12:00:00.000Z';

    // Act
    const formattedDate = component.formatDate(dateString);

    // Assert
    expect(formattedDate).toContain('2025');
    expect(formattedDate).toContain('12:00');
  });
});
