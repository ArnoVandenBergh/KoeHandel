import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Lobby } from './pages/lobby/lobby';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },
    {
        path: 'lobby',
        component: Lobby
    }
];
