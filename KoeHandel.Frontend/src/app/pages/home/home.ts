import { Component, inject } from '@angular/core';
import { Button } from "../../components/button/button";
import { Title } from "../../components/title/title";
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [Button, Title],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {
  router = inject(Router);

  openLobby() {
    this.router.navigate(['lobby']);
  }
}
