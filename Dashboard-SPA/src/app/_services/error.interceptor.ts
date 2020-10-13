import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                // DEBUG
                // console.log(error);

                if (error instanceof HttpErrorResponse) {
                    if ((error.status >= 400) && (error.status < 500)) {
                        return throwError(
                            {
                                status: error.status,
                                message: error.error
                            }
                        );
                    }

                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        console.error(applicationError);
                        return throwError(applicationError);
                    }

                    let serverError = error.error;
                    let modelStateErrors = '';
                    if (serverError) {
                        // If there are multiple errors concat them
                        if (typeof serverError === 'object') {
                            serverError = serverError.errors;
                            // tslint:disable-next-line: forin
                            for (const key in serverError) {
                                modelStateErrors += serverError[key] + '\n';
                            }
                        }
                    }

                    return throwError(modelStateErrors || serverError || 'Server Error');
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
