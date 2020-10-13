import { Component, OnInit } from '@angular/core';
import { interval } from 'rxjs';

@Component({
  selector: 'app-clock',
  templateUrl: './clock.component.html',
  styleUrls: ['./clock.component.css']
})
export class ClockComponent implements OnInit {
  hours = 0;
  minutes = 0;
  seconds = 0;
  day = 'Monday';
  date = '1970. January 1.';

  constructor() { }

  ngOnInit() {
    this.updateClock();
    const tick = interval(1000);
    tick.subscribe(
      next => {
        this.updateClock();
      }
    );
  }

  updateClock() {
    const now = new Date();
    this.hours = now.getHours();
    this.minutes = now.getMinutes();
    this.seconds = now.getSeconds();
    this.day = this.lookupDay(now.getDay());
    this.date = now.getFullYear().toString() + '. ' + this.lookupMonth(now.getMonth()) + ' ' + now.getUTCDate().toString() + '.';
  }

  padNumber(num) {
    let numString: string;
    numString = String(num);
    if (numString.length < 2) {
      numString = '0' + numString;
    }

    return numString;
  }

  lookupDay(dayNum) {
    const weekday = new Array(7);
    weekday[0] = 'Sunday';
    weekday[1] = 'Monday';
    weekday[2] = 'Tuesday';
    weekday[3] = 'Wednesday';
    weekday[4] = 'Thursday';
    weekday[5] = 'Friday';
    weekday[6] = 'Saturday';

    return weekday[dayNum];
  }

  lookupMonth(monthNum) {
    const month = new Array(12);
    month[0] = 'January';
    month[1] = 'February';
    month[2] = 'March';
    month[3] = 'April';
    month[4] = 'May';
    month[5] = 'June';
    month[6] = 'July';
    month[7] = 'August';
    month[8] = 'September';
    month[9] = 'October';
    month[10] = 'November';
    month[11] = 'December';

    return month[monthNum];
  }

}
