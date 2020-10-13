import { Injectable } from '@angular/core';
import { TodoFilterData } from '../_models/TodoFilterData';
import { BehaviorSubject } from 'rxjs';
import { TodoItem } from '../_models/TodoItem';

@Injectable({
  providedIn: 'root'
})
export class KanbanfiltersService {

  constructor() { }

  public tasks: BehaviorSubject<TodoItem[]> = new BehaviorSubject([]);

  public filterData: BehaviorSubject<TodoFilterData> = new BehaviorSubject(
    {
      sort: {
        allowCustom: false,
        sortBy: TodoFilterData.SortType.Date
      },
      hiddenPriorities: [],
      hiddenProjects: []
    }
  );

}
