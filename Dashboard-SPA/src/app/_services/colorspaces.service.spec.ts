/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ColorspacesService } from './colorspaces.service';

describe('Service: Colorspaces', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ColorspacesService]
    });
  });

  it('should ...', inject([ColorspacesService], (service: ColorspacesService) => {
    expect(service).toBeTruthy();
  }));
});
