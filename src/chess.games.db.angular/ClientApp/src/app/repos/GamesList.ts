import { ChessGameItem } from "./ChessGameItem";

export class GamesList {
  constructor(
    public games: ChessGameItem[],
    public currentPage: number,
    public nextPage: string,
    public previousPage: string

  ) { }

}
