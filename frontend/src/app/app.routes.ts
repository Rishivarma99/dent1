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
		path: '',
		pathMatch: 'full',
		redirectTo: 'login'
	},
	{
		path: '**',
		redirectTo: 'login'
	}
];
