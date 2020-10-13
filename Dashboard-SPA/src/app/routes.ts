import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { OverviewComponent } from './overview/overview.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdminComponent } from './admin/admin.component';
import { SettingsUiComponent } from './user-settings/settings-ui/settings-ui.component';
import { SettingsUserComponent } from './user-settings/settings-user/settings-user.component';
import { SettingsOverviewComponent } from './user-settings/settings-overview/settings-overview.component';
import { KanbanComponent } from './kanban/kanban.component';
import { BoardComponent } from './kanban/board/board.component';
import { ProjectsComponent } from './kanban/projects/projects.component';
import { TimelineComponent } from './kanban/timeline/timeline.component';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent, data: {mainDepth: 0} },
    { path: 'overview', component: OverviewComponent, canActivate: [AuthGuard], data: {mainDepth: 1}  },
    {
        path: 'kanban', component: KanbanComponent, canActivate: [AuthGuard], data: {mainDepth: 2}, children: [
            { path: 'board', component: BoardComponent, canActivate: [AuthGuard] },
            { path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard] },
            { path: 'timeline', component: TimelineComponent, canActivate: [AuthGuard] }
        ]
    },
    {
        path: 'settings', component: UserSettingsComponent, canActivate: [AuthGuard], data: {mainDepth: 5}, children: [
            { path: 'ui', component: SettingsUiComponent, canActivate: [AuthGuard] },
            { path: 'overview', component: SettingsOverviewComponent, canActivate: [AuthGuard] },
            { path: 'user', component: SettingsUserComponent, canActivate: [AuthGuard] }
        ]
    },
    { path: 'admin', component: AdminComponent, canActivate: [AuthGuard], data: {mainDepth: 6}  },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
