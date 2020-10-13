import { Component, OnInit, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl } from '@angular/forms';
import { TodoFilterData } from '../_models/TodoFilterData';
import { TodoItem } from '../_models/TodoItem';
import { AlertifyService } from '../_services/alertify.service';
import { KanbanfiltersService } from '../_services/kanbanfilters.service';

@Component({
  selector: 'app-kanban',
  templateUrl: './kanban.component.html',
  styleUrls: ['./kanban.component.css']
})

// TODO: ! Populate filter projects dropdown from db
// TODO: ! Show/Hide all for Project/Priority filters
// TODO: ! Save/Load filter settings
// TODO: ! Implement Projects tab
// TODO: ^ Implement Timeline tab
// TODO: ! Rename kanbanfilters.service to kanban.service

export class KanbanComponent implements OnInit {
  sortRadio = new FormControl();
  customSort = new FormControl();

  projects: string[] = [
    'General Stuff',
    'Important Project',
    'Different Things'
  ];

  // Screen info
  screenHeight: any;
  screenWidth: any;

  constructor(
    private router: Router,
    private activeRoute: ActivatedRoute,
    private alertify: AlertifyService,
    private filter: KanbanfiltersService
  ) { }

  ngOnInit() {
    // TODO: _ when kanban is selected, and kanban get selected again, it navigates to base
    if (this.activeRoute.children.length === 0) {
      this.router.navigate(['board'], { relativeTo: this.activeRoute });
    }
    this.onResize();
    this.sortRadio.setValue('0');

    // Query projects
  }

  sortByChanged() {
    this.filter.filterData.value.sort.sortBy = this.sortRadio.value;
    this.filter.filterData.next(this.filter.filterData.value);
  }

  customSortChanged() {
    this.filter.filterData.value.sort.allowCustom = this.customSort.value;
    this.filter.filterData.next(this.filter.filterData.value);
  }

  filterPriorityChanged(priority: string) {
    const value = (<HTMLInputElement>document.getElementsByName('priority_' + priority)[0]).checked;
    const field = TodoItem.Priority[priority];
    if (value) {
      const index = this.filter.filterData.value.hiddenPriorities.indexOf(field);
      this.filter.filterData.value.hiddenPriorities.splice(index, 1);
    } else {
      this.filter.filterData.value.hiddenPriorities.push(field);
    }
    this.filter.filterData.next(this.filter.filterData.value);
  }

  filterProjectChanged(project) {
    const value = (<HTMLInputElement>document.getElementsByName('project_' + project)[0]).checked;
    if (value) {
      const index = this.filter.filterData.value.hiddenProjects.indexOf(project);
      this.filter.filterData.value.hiddenProjects.splice(index, 1);
    } else {
      this.filter.filterData.value.hiddenProjects.push(project);
    }
    this.filter.filterData.next(this.filter.filterData.value);
  }

  closeDropdown(dropDown) {
    // dropDown.close();
  }

  filterSaved() {
    this.alertify.success('Implement Filter Save/Load');
  }

  // Help displaying
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
    this.screenHeight = window.innerHeight;
    this.screenWidth = window.innerWidth;
  }

}
