import { Component, inject } from '@angular/core';
import { Button } from "../../components/button/button";
import { Title } from "../../components/title/title";
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [Button, Title, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {
  router = inject(Router);
  userName: string = '';

  openLobby() {
    this.router.navigate(['lobby'], { queryParams: { name: this.userName } });
  }
}
