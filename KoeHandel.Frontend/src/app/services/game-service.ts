import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { NewGame } from '../models/new-game';
import { BASE_URL } from './constants';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  constructor(private _http: HttpClient) { }

  public startNewGame(playerName: string): Observable<NewGame> {
    return this._http.post<NewGame>(`${BASE_URL}/games`, { playerName });
  }

  public getGameById(gameId: string): Observable<NewGame> {
    return this._http.get<NewGame>(`${BASE_URL}/games/${gameId}`);
  }
}
