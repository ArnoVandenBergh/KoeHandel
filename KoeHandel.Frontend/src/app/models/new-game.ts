import { GameState } from "./game-state";
import { NewPlayer } from "./new-player";

export interface NewGame {
  gameId: number;
  players: NewPlayer[];
  gameState: GameState;
}
