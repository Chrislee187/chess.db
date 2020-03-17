export class Pagination {
  public page: number;
  public pageSize: number;

  public sortFields: SortField[];
}

export class SortField {
  constructor(public fieldName: string, public direction: string) {
  }
}
