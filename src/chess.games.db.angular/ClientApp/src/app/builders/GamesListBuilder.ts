import { ChessGameBuilder } from "./ChessGameBuilder";
import { GamesList } from "../models/GamesList";

export class GamesListBuilder {

  private defaultGame = new ChessGameBuilder().build();

  build(): GamesList {
    return new GamesList([this.defaultGame], "", null);
  }
}
