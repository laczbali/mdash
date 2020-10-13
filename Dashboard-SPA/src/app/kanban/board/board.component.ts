import { Component, OnInit, Input } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TodoItem } from 'src/app/_models/TodoItem';
import { TodoFilterData } from 'src/app/_models/TodoFilterData';
import { KanbanfiltersService } from 'src/app/_services/kanbanfilters.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css']
})
export class BoardComponent implements OnInit {
  // TODO: ! Implement task query from backend
  // TODO: ? Add New/+ button to New task category
  // TODO: ? Have some edit functionality on double click, or similar

  newItems: TodoItem[] = [
    {
      taskName: 'A Please do this upcoming to do thing',
      projectName: 'Important Project',
      storyName: 'Whatever',
      priority: TodoItem.Priority.Low,
      createdDate: new Date(Date.now()),
      dueDate: new Date(Date.now() + (1000 * 60 * 60 * 24 * 40))
    }
  ];

  inProgressItems: TodoItem[] = [
    {
      taskName: 'I have a task task that is important',
      projectName: 'General Stuff',
      storyName: 'Placeholder',
      priority: TodoItem.Priority.High,
      createdDate: new Date(Date.now()),
      dueDate: new Date(Date.now() + (1000 * 60 * 60 * 24 * 20))
    }
  ];

  doneItems: TodoItem[] = [
    {
      taskName: 'Done Task To Be Done In the Past',
      projectName: 'General Stuff',
      storyName: 'Placeholder',
      priority: TodoItem.Priority.Normal,
      createdDate: new Date(Date.now()),
      dueDate: new Date(Date.now() + (1000 * 60 * 60 * 24))
    }
  ];

  constructor(public filter: KanbanfiltersService) { }

  ngOnInit() {
    // Sort/filter arrays on settings change
    this.filter.filterData.subscribe(
      next => { this.organizeLists(); }
    );
  }

  drop(event: CdkDragDrop<string[]>) {
    // Place element in array
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
    }

    // Sort/filter arrays
    this.organizeLists();
  }

  organizeLists() {
    // Sort lists
    if (this.filter.filterData.value.sort.allowCustom === false) {
      const lists = [this.newItems, this.inProgressItems, this.doneItems];
      lists.forEach(list =>
        list.sort(
          (a, b) => {
            const selectedSort: TodoFilterData.SortType = <TodoFilterData.SortType>+this.filter.filterData.value.sort.sortBy;

            switch (selectedSort) {
              case TodoFilterData.SortType.Date:
                if (a.dueDate < b.dueDate) { return -1; } else { return 1; }

              case TodoFilterData.SortType.Name:
                return a.taskName.localeCompare(b.taskName);

              case TodoFilterData.SortType.Priority:
                if (a.priority > b.priority) { return -1; } else { return 1; }

              case TodoFilterData.SortType.Project:
                return a.projectName.localeCompare(b.projectName);

              case TodoFilterData.SortType.Story:
                return a.storyName.localeCompare(b.storyName);

              default:
                return 0;
            }
          }
        )
      );
    }
  }

  isHidden(item: TodoItem) {
    if (this.filter.filterData.value.hiddenPriorities.indexOf(item.priority) !== -1) {
      return true;
    }

    if (this.filter.filterData.value.hiddenProjects.indexOf(item.projectName) !== -1) {
      return true;
    }

    return false;
  }

  lookupMonth(monthNum) {
    const month = new Array(12);
    month[0] = 'jan';
    month[1] = 'feb';
    month[2] = 'mar';
    month[3] = 'apr';
    month[4] = 'may';
    month[5] = 'jun';
    month[6] = 'jul';
    month[7] = 'aug';
    month[8] = 'sep';
    month[9] = 'oct';
    month[10] = 'nov';
    month[11] = 'dec';

    return month[monthNum];
  }

}
