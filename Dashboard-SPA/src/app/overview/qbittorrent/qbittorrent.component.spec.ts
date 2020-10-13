/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { QbittorrentComponent } from './qbittorrent.component';

describe('QbittorrentComponent', () => {
  let component: QbittorrentComponent;
  let fixture: ComponentFixture<QbittorrentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QbittorrentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QbittorrentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
