import { HttpParams } from "@angular/common/http";

export class SortField {

  static fromUrl(url: string): SortField[] {
    const i = url.indexOf("?");

    if (i === -1) return [];
    
    const urlParams = url.substr(i + 1);
    const params = new HttpParams({ fromString: urlParams });

    const sorting = params.get("order-by");

    if (!sorting) return [];

    const fields = new Array<SortField>();

    for (let clause of sorting.split(",")) {
      const parts = clause.split(" ");

      if (parts.length === 1) {
        fields.push(new SortField(parts[0], true ));
      } else {
        fields.push(new SortField(parts[0], parts[1].toLocaleLowerCase() === "asc" ));
      }
    }

    return fields;
  }

  constructor(public fieldName: string, public ascending: boolean) {
  }

  public get matSortDirection(): string {
    return this.ascending ? "asc" : "desc";
  }
}
