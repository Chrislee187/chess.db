import { ChessGameItem } from "./ChessGameItem";
import { Pagination } from "./Pagination";

export class GamesList {
  public pageSize: number;
  public totalPages: number;
  public currentPage: number;
  public nextPage: string;
  public previousPage: string;

  constructor(
    public games?: Array<ChessGameItem>,
    private pagination?: Pagination
  ) {

    if (pagination) {
      this.currentPage = pagination.currentPage;
      this.nextPage = pagination.nextPage;
      this.previousPage = pagination.previousPage;
      this.pageSize = pagination.pageSize;
      this.totalPages = pagination.totalPages;

    }
  }

}
