import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { JwtModule } from '@auth0/angular-jwt';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ChartsModule } from 'ng2-charts';
import { ColorPickerModule } from 'ngx-color-picker';
import { NgxBootstrapSliderModule } from 'ngx-bootstrap-slider';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgbModule  } from '@ng-bootstrap/ng-bootstrap';
import { NgxGaugeModule } from 'ngx-gauge';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './home/login/login.component';
import { RegisterComponent } from './home/register/register.component';
import { OverviewComponent } from './overview/overview.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { AdminComponent } from './admin/admin.component';
import { ClockComponent } from './overview/clock/clock.component';
import { WeatherComponent } from './overview/weather/weather.component';
import { SettingsUiComponent } from './user-settings/settings-ui/settings-ui.component';
import { SettingsOverviewComponent } from './user-settings/settings-overview/settings-overview.component';
import { SettingsUserComponent } from './user-settings/settings-user/settings-user.component';
import { QbittorrentComponent } from './overview/qbittorrent/qbittorrent.component';
import { ServerComponent } from './overview/server/server.component';
import { SpotifyComponent } from './overview/spotify/spotify.component';
import { GoogleComponent } from './overview/google/google.component';

import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { SettingsService } from './_services/settings.service';
import { ColorspacesService } from './_services/colorspaces.service';
import { KanbanfiltersService } from './_services/kanbanfilters.service';

import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { KanbanComponent } from './kanban/kanban.component';
import { BoardComponent } from './kanban/board/board.component';
import { ProjectsComponent } from './kanban/projects/projects.component';
import { TimelineComponent } from './kanban/timeline/timeline.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      OverviewComponent,
      UserSettingsComponent,
      SettingsUiComponent,
      SettingsOverviewComponent,
      SettingsUserComponent,
      LoginComponent,
      AdminComponent,
      ClockComponent,
      WeatherComponent,
      QbittorrentComponent,
      KanbanComponent,
      BoardComponent,
      ProjectsComponent,
      TimelineComponent,
      ServerComponent,
      SpotifyComponent,
      GoogleComponent
   ],
   imports: [
      RouterModule.forRoot(appRoutes),
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      }),
      BsDropdownModule.forRoot(),
      ProgressbarModule.forRoot(),
      TabsModule.forRoot(),
      ChartsModule,
      ColorPickerModule,
      NgxBootstrapSliderModule,
      BrowserAnimationsModule,
      ScrollingModule,
      DragDropModule,
      NgbModule,
      NgxGaugeModule
   ],
   providers: [
      AuthService,
      ErrorInterceptorProvider,
      AlertifyService,
      AuthGuard,
      SettingsService,
      ColorspacesService,
      KanbanfiltersService
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
