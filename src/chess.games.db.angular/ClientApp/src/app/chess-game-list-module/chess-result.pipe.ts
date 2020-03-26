import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'chessResult'
})
export class ChessResultPipe implements PipeTransform {

  transform(value: string): string {
    return value === "1-0" ? "w" : value === "0-1" ? "b" : "½";
  }

}
