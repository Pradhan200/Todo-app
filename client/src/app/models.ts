export interface TodoItem {
  id: string;
  title: string;
  createdAt: string; // ISO-8601
  completed: boolean;
  completedAt?: string; // ISO-8601
}
