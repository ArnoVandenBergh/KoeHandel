export enum GameState {
        notStarted,
        inProgress,
        finished
}

export function GameStateMessage(state: GameState): string {
    switch (state) {
        case GameState.notStarted:
            return "Het spel is nog niet begonnen.";
        case GameState.inProgress:
            return "Het spel is bezig.";
        case GameState.finished:
            return "Het spel is afgelopen.";
        default:
            return "Onbekende status.";
    }
}
