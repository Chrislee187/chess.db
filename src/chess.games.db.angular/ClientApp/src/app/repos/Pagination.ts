export class Pagination {
  public currentPage: number;
  public pageSize: number;
  public totalPages: number;
  public nextPage: string;
  public previousPage: string;
  public sortFields: SortField[];

  static default: Pagination = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 0,
    nextPage: "",
    previousPage: "",
    sortFields: [{ fieldName: "White", ascending: true }]
  };
}

export class SortField {
  constructor(public fieldName: string, public ascending: boolean) {
  }
}
