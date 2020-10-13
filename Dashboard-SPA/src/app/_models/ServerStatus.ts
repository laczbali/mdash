import { Vpn } from './Vpn';
import { Drive } from './Drive';
import { Process } from './Process';
import { Resources } from './Resources';
import { error } from 'protractor';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ErrorObserver } from 'rxjs';

export class ServerStatus {
    vpn: Vpn;
    drives: Drive[];
    processes: Process[];
    resources: Resources;
    errors: string;
    querytime: number;
}
