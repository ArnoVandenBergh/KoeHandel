import { Component, inject } from '@angular/core';
import { Button } from "../../components/button/button";
import { Title } from "../../components/title/title";
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [Button, FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class Home {

  constructor() { 
    this.userName = localStorage.getItem('userName')?.length as number > 0 ? localStorage.getItem('userName')! : '';
  }
  router = inject(Router);
  userName: string = '';

  openLobby() {
    this.saveUserName();
    this.router.navigate(['lobby'], { queryParams: { name: this.userName } });
  }

  private saveUserName() {
    localStorage.setItem('userName', this.userName);
  }
}
