import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: 'login',
		loadComponent: () => import('./features/auth/pages/login-page/login-page').then((m) => m.LoginPage)
	},
	{
		path: 'doctors',
		loadComponent: () => import('./features/doctors/pages/doctors-page/doctors-page').then((m) => m.DoctorsPage)
	},
	{
		path: 'patients',
		loadComponent: () => import('./features/patients/pages/patients-page/patients-page').then((m) => m.PatientsPage)
	},
	{
		path: '',
		pathMatch: 'full',
		redirectTo: 'login'
	},
	{
		path: '**',
		redirectTo: 'login'
	}
];
