import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Lobby } from './pages/lobby/lobby';
import { Join } from './pages/join/join';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },
    {
        path: 'lobby',
        component: Lobby
    },
    {
        path: 'join',
        component: Join
    }
];
