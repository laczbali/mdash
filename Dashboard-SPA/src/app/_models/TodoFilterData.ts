import { TodoItem } from './TodoItem';

export class TodoFilterData {
  sort: {
    allowCustom: boolean,
    sortBy: TodoFilterData.SortType
  };

  hiddenPriorities: TodoItem.Priority[];

  hiddenProjects: string[];
}

export namespace TodoFilterData {
  export enum SortType {
    Date = 0,
    Name = 1,
    Priority = 2,
    Project = 3,
    Story = 4
  }
}
