import { SortField } from "./SortField";

export class Pagination implements IPagination {
  public currentPage: number;
  public pageSize: number;
  public totalPages: number;
  public nextPageUrl?: string;
  public previousPageUrl?: string;
  public sortFields: SortField[];

  static default: Pagination = new Pagination({
    currentPage: 1,
    pageSize: 10,
    totalPages: 0,
    nextPageUrl: "",
    previousPageUrl: "",
    sortFields: [{ fieldName: "White", ascending: true }]
  });

  static parseJson(json: string): Pagination {
    return new Pagination(JSON.parse(json));
  }

  constructor(pagination: Pagination) {
    this.currentPage = pagination.currentPage;
    this.pageSize = pagination.pageSize;
    this.totalPages = pagination.totalPages;
    this.nextPageUrl = pagination.nextPageUrl;
    this.previousPageUrl = pagination.previousPageUrl;
    this.sortFields = pagination.sortFields;
  }

  toUrlQueryParams?(): string {

    let urlParams = `?page-size=${this.pageSize}`;
    urlParams += `&page=${this.currentPage}`;


    if (this.sortFields.length > 0) {
      let orderByParams = "";
      orderByParams = this.sortFields
        .map(x => x)
        .reduce((g: string, field: SortField) => {
          g += `${field.fieldName}${field.ascending ? "" : " desc"},`;
          return g;
        }, "");

      urlParams += `&order-by=${orderByParams.substr(0, orderByParams.length - 1)}`;
    }

    return urlParams;
  }
}
export interface IPagination {
  currentPage: number;
  pageSize: number;
  totalPages: number;
  nextPageUrl?: string;
  previousPageUrl?: string;
  sortFields: SortField[];

  toUrlQueryParams?(): string;
}
