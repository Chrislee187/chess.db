import { ChessGameItem } from "./ChessGameItem";
import { Pagination } from "./Pagination";

export class GamesList {

  static empty = new GamesList([], "", null);

  pageSize: number;
  totalPages: number;
  currentPage: number;
  nextPageUrl: string;
  previousPageUrl: string;
  firstPageUrl: string;
  lastPageUrl: string;

  constructor(
    public games: Array<ChessGameItem>,
    public currentPageUrl: string,
    public pagination: Pagination
  ) {

    if (pagination) {
      this.currentPage = pagination.currentPage;
      this.nextPageUrl = pagination.nextPageUrl;
      this.previousPageUrl = pagination.previousPageUrl;
      this.pageSize = pagination.pageSize;
      this.totalPages = pagination.totalPages;

      if (this.currentPage !== 1) {
        this.firstPageUrl = this.buildUrlToAnotherPage(this.currentPageUrl, 1);
      }
      if (this.currentPage !== this.totalPages) {
        this.lastPageUrl = this.buildUrlToAnotherPage(this.currentPageUrl, this.totalPages);
      }
    }
  }
  buildUrlToAnotherPage(currentPageUrl: string, newPage: number): string {

    return currentPageUrl
      ? currentPageUrl.replace(`page=${this.currentPage}`, `page=${newPage}`)
      : "";
  }
}
