import { Component, OnInit, HostListener } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit {
   // Screen info
   screenHeight: any;
   screenWidth: any;

  constructor(private router: Router, private activeRoute: ActivatedRoute) { }

  ngOnInit() {
    // TODO: _ when settings is selected, and settings get selected again, it navigates to settings base, instead of settnings/ui
    if (this.activeRoute.children.length === 0) {
      this.router.navigate(['ui'], { relativeTo: this.activeRoute });
    }

    this.onResize();
  }

  // Help displaying
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
      this.screenHeight = window.innerHeight;
      this.screenWidth = window.innerWidth;
  }

}
