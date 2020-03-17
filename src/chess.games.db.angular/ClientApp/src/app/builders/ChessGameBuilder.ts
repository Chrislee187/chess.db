import { ChessGameItem } from "../models/ChessGameItem";
import { GamesList } from "../models/GamesList";

export class ChessGameBuilder {

  public build(): ChessGameItem {
    let game: ChessGameItem = {
      white: "white",
      black: "black",
      result: "1-0",
      event: "An event",
      round: "1",
      site: "At a site",
      date: "??.??.??",
      moves: "movelist..."
    };

    return game;
  }
}
