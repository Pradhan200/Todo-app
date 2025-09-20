import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from './todo.service';
import { TodoItem } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="notion-app">
      <!-- Sidebar -->
      <aside class="sidebar">
        <div class="sidebar-header">
          <div class="logo">
            <span class="logo-icon">✓</span>
            <span class="logo-text">Tasks</span>
          </div>
        </div>
        
        <nav class="sidebar-nav">
          <div class="nav-section">
            <div class="nav-item active">
              <span class="nav-icon">📋</span>
              <span class="nav-text">All Tasks</span>
              <span class="nav-count">{{ todos.length }}</span>
            </div>
          </div>
        </nav>
      </aside>

      <!-- Main Content -->
      <main class="main-content">
        <!-- Top Bar -->
        <header class="top-bar">
          <div class="breadcrumb">
            <span class="breadcrumb-item">All Tasks</span>
          </div>
        </header>

        <!-- Page Content -->
        <div class="page-content">
          <!-- Add Task Form -->
          <div class="add-task-section">
            <form (ngSubmit)="addTodo()" #todoForm="ngForm" class="add-task-form">
              <div class="input-container">
                <span class="input-icon">+</span>
                <input
                  type="text"
                  name="title"
                  [(ngModel)]="newTodoTitle"
                  #titleInput="ngModel"
                  class="task-input"
                  placeholder="Type a task and press Enter..."
                  required
                  [disabled]="isLoading">
                <button
                  type="submit"
                  class="add-button"
                  [disabled]="!todoForm.valid || isLoading || !newTodoTitle.trim()">
                  {{ isLoading ? 'Adding...' : 'Add' }}
                </button>
              </div>
            </form>
          </div>

          <!-- Error Display -->
          <div *ngIf="errorMessage" class="error-notification">
            <span class="error-icon">⚠️</span>
            <span>{{ errorMessage }}</span>
          </div>

          <!-- Loading State -->
          <div *ngIf="isLoading && todos.length === 0" class="loading-state">
            <div class="loading-spinner"></div>
            <span>Loading tasks...</span>
          </div>

          <!-- Tasks List -->
          <div *ngIf="!isLoading && todos.length > 0" class="tasks-section">
            <div class="tasks-header">
              <h1 class="page-title">All Tasks</h1>
              <div class="tasks-meta">
                <span class="tasks-count">{{ activeTodos.length }} active, {{ completedTodos.length }} completed</span>
                <button class="toggle-completed-btn" (click)="toggleShowCompleted()" [class.active]="showCompleted">
                  {{ showCompleted ? 'Hide' : 'Show' }} Completed
                </button>
              </div>
            </div>

            <!-- Active Tasks -->
            <div class="tasks-list">
              <div *ngFor="let todo of activeTodos; let i = index" class="task-item" [style.animation-delay]="i * 0.05 + 's'">
                <div class="task-checkbox">
                  <input type="checkbox" class="checkbox" (click)="toggleTodoComplete(todo.id)">
                </div>
                <div class="task-content">
                  <div class="task-title">{{ todo.title }}</div>
                  <div class="task-meta">
                    <span class="task-date">{{ formatDate(todo.createdAt) }}</span>
                  </div>
                </div>
                <div class="task-actions">
                  <button class="action-btn" (click)="deleteTodo(todo.id)" [disabled]="isLoading" title="Delete">
                    <span>🗑️</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Completed Tasks -->
            <div *ngIf="showCompleted && completedTodos.length > 0" class="completed-section">
              <h3 class="completed-title">Completed Tasks</h3>
              <div class="tasks-list completed-list">
                <div *ngFor="let todo of completedTodos; let i = index" class="task-item completed" [style.animation-delay]="i * 0.05 + 's'">
                  <div class="task-checkbox">
                    <input type="checkbox" class="checkbox" checked (click)="toggleTodoComplete(todo.id)">
                  </div>
                  <div class="task-content">
                    <div class="task-title completed-text">{{ todo.title }}</div>
                    <div class="task-meta">
                      <span class="task-date">{{ formatDate(todo.completedAt || todo.createdAt) }}</span>
                    </div>
                  </div>
                  <div class="task-actions">
                    <button class="action-btn" (click)="deleteTodo(todo.id)" [disabled]="isLoading" title="Delete">
                      <span>🗑️</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div *ngIf="!isLoading && todos.length === 0 && !errorMessage" class="empty-state">
            <div class="empty-illustration">
              <span class="empty-icon">📝</span>
            </div>
            <h2 class="empty-title">No tasks yet</h2>
            <p class="empty-description">Add your first task above to get started with your productivity journey.</p>
          </div>
        </div>
      </main>
    </div>
  `,
  styleUrls: []
})
export class AppComponent implements OnInit {
  todos: TodoItem[] = [];
  newTodoTitle = '';
  isLoading = false;
  errorMessage = '';
  showCompleted = false;

  constructor(private todoService: TodoService) {}

  get activeTodos(): TodoItem[] {
    return this.todos.filter(todo => !todo.completed);
  }

  get completedTodos(): TodoItem[] {
    return this.todos.filter(todo => todo.completed);
  }

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.todoService.getTodos().subscribe({
      next: (todos) => {
        this.todos = todos;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  addTodo(): void {
    if (!this.newTodoTitle.trim()) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.todoService.addTodo(this.newTodoTitle.trim()).subscribe({
      next: (newTodo) => {
        this.todos = [newTodo, ...this.todos]; // Prepend new todo
        this.newTodoTitle = '';
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  deleteTodo(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.todoService.deleteTodo(id).subscribe({
      next: () => {
        this.todos = this.todos.filter(todo => todo.id !== id);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  toggleTodoComplete(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.todoService.toggleTodoComplete(id).subscribe({
      next: (updatedTodo) => {
        const index = this.todos.findIndex(todo => todo.id === id);
        if (index !== -1) {
          this.todos[index] = updatedTodo;
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  toggleShowCompleted(): void {
    this.showCompleted = !this.showCompleted;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
  }
}
