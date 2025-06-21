import { Component, inject } from '@angular/core';
import { Button } from '../../components/button/button';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-join',
  standalone: true,
  imports: [Button, FormsModule],
  templateUrl: './join.html',
  styleUrl: './join.scss'
})
export class Join {
  gameId: string = '';
  userName: string = '';
  router = inject(Router);
  route = inject(ActivatedRoute);

  constructor() {
    this.userName = this.route.snapshot.queryParamMap.get('name') || '';
  }

  joinGame() {
    // TODO: Add logic to validate gameId exists
    this.router.navigate(['lobby'], { queryParams: { gameId: this.gameId, name: this.userName } });
  }
}
