import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: 'login',
		loadComponent: () => import('./pages/login-page/login-page').then((m) => m.LoginPage)
	},
	{
		path: 'home',
		loadComponent: () => import('./pages/home-page/home-page').then((m) => m.HomePage)
	},
	{
		path: 'doctors',
		loadComponent: () => import('./pages/doctors-page/doctors-page').then((m) => m.DoctorsPage)
	},
	{
		path: 'patients',
		loadComponent: () => import('./pages/patients-page/patients-page').then((m) => m.PatientsPage)
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
