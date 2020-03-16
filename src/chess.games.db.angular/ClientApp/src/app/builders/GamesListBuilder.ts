import { ChessGameBuilder } from "./ChessGameBuilder";
import { ChessGameItem } from "../repos/ChessGameItem";
import { GamesList } from "../repos/GamesList";

export class GamesListBuilder {

  private defaultGame: ChessGameItem = new ChessGameBuilder().build();

  public build(): GamesList {
    return new GamesList([this.defaultGame], 1, "", "");
  }
}
