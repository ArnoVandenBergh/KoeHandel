import { Component } from '@angular/core';
import { Title } from "../../components/title/title";
import { Loading } from "../../components/loading/loading";
import { GameService } from '../../services/game-service';
import { NewGame } from '../../models/new-game';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Container } from '../../components/container/container';
import { ActivatedRoute } from '@angular/router';

// TODO: Add logic to distinct between people starting a new game and joining an existing one
@Component({
  selector: 'app-lobby',
  imports: [Title, Loading, AsyncPipe, Container],
  templateUrl: './lobby.html',
  styleUrl: './lobby.scss'
})
export class Lobby {
  gameData$: Observable<NewGame>;

  constructor(private gameService: GameService, private route: ActivatedRoute) {
    const playerName = this.route.snapshot.queryParamMap.get('name') || '';
    this.gameData$ = this.gameService.startNewGame(playerName);
  }
}
