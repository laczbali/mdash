export class TodoItem {
    taskName: string;
    projectName: string;
    storyName: string;
    priority: TodoItem.Priority;
    createdDate: Date;
    dueDate: Date;
}

export namespace TodoItem {
    export enum Priority {
        Low = -1,
        Normal = 0,
        High = 1
    }
}
