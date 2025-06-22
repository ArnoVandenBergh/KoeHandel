import { Component, inject, signal } from '@angular/core';
import { Button } from '../../components/button/button';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { GameService } from '../../services/game-service';
import { GameState, GameStateMessage } from '../../models/game-state';

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
  errorMessage = signal('');
  router = inject(Router);
  route = inject(ActivatedRoute);
  gameService = inject(GameService);

  constructor() {
    this.userName = this.route.snapshot.queryParamMap.get('name') || '';
  }

  joinGame() {
    this.errorMessage.set('');
    this.gameService.getGameById(this.gameId).subscribe({
      next: (game) => {
        if (game.gameState !== GameState.notStarted) {
          this.errorMessage.set(GameStateMessage(game.gameState));
        } else if (game.players.length >= 5) {
          this.errorMessage.set('Het spel is al vol (maximaal 5 spelers).');
        } else {
          this.router.navigate(['lobby'], { queryParams: { gameId: this.gameId, name: this.userName } });
        }
      },
      error: (err) => {
        if (err.status === 404) {
          this.errorMessage.set('Dit spel bestaat niet.');
        } else {
          this.errorMessage.set('Er is een fout opgetreden bij het zoeken van het spel.');
        }
      }
    });
  }
}
