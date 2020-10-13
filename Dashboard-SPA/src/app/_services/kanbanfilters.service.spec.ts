/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { KanbanfiltersService } from './kanbanfilters.service';

describe('Service: Kanbanfilters', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [KanbanfiltersService]
    });
  });

  it('should ...', inject([KanbanfiltersService], (service: KanbanfiltersService) => {
    expect(service).toBeTruthy();
  }));
});
