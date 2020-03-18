import { ChessGameBuilder } from "./ChessGameBuilder";
import { ChessGameItem } from "../models/ChessGameItem";
import { GamesList } from "../models/GamesList";

export class GamesListBuilder {

  private defaultGame = new ChessGameBuilder().build();

  build(): GamesList {
    return new GamesList([this.defaultGame], "", null);
  }
}
