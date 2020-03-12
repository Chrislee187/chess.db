import { ChessGameItem } from "./ChessGameItem";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators"
import { DefaultUrlSerializer, UrlTree } from '@angular/router';

@Injectable()
export class ChessGameService {
  public games: ChessGameItem[] = [
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "1/2-1/2",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd711. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    },
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "1/2-1/2",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd711. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    },
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "1/2-1/2",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd711. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    },
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "1/2-1/2",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd711. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    },
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "0-1",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd7 11. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    },
    {
      white: "Gary Kasparov",
      black: "Nigel Short",
      result: "1-0",
      event: "An event",
      round: "1",
      site: "Somewhere over the rainbow",
      date: "??.??.??",
      moves: "1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O - O Be7 6. Re1 b5 7. Bb3 d6 8. c3 O- O 9. h3 Nb8 10. d4 Nbd7 11. c4 c6 12. cxb5 axb5 13. Nc3 Bb7 14. Bg5 b4 15. Nb1 h6 16. Bh4 c5 17. dxe5Nxe4 18. Bxe7 Qxe7 19. exd6 Qf6 20. Nbd2 Nxd6 21. Nc4 Nxc4 22. Bxc4 Nb623. Ne5 Rae8 24. Bxf7 + Rxf7 25. Nxf7 Rxe1 + 26. Qxe1 Kxf7 27. Qe3 Qg5 28. Qxg5hxg5 29. b3 Ke6 30. a3 Kd6 31. axb4 cxb4 32. Ra5 Nd5 33. f3 Bc8 34. Kf2 Bf535. Ra7 g6 36. Ra6 + Kc5 37. Ke1 Nf4 38. g3 Nxh3 39. Kd2 Kb5 40. Rd6 Kc5 41. Ra6Nf2 42. g4 Bd3 43. Re6"
    }

  ];

  public previousPage: string;
  public nextPage: string;
  public currentPage: number;
  constructor(private httpClient: HttpClient) { }

  loadGames(url: string) {

    if (url === "") url = "http://localhost:5000/api/games";

    // TODO: Use an injected repository rather than HttpClient directly

    return this.httpClient
      .get(url,
        {
          observe: 'response',
          responseType: 'json'
        })
      .pipe(
        map((response: any) => {
          this.currentPage = this.getCurrentPageNumberFromUrl(url);
          this.nextPage = this.getLink('next-page', response.body._links);
          this.previousPage = this.getLink('prev-page', response.body._links);

          // TODO: Need access to the X-Pagination header to the reset
          // first/last, page number, total pages etc.
          // Can't seem to get around a CORS restriction, something in the webapi startup not quiet right
          this.games = response.body.value.map(d => {
            return this.mapToChessGameItem(d);
          });
          return true;
        }));
  }

  getCurrentPageNumberFromUrl(url:string) {
    if (url.indexOf('?') > -1) {
      const httpParams = new HttpParams({ fromString: url.split('?')[1] });
      return +httpParams.get('page');
    } else {
      return 1;
    }
  }
  getLink(name: string, links: any) {
    let t = links.find(l => l.rel === name);

    if (t) {
      return t.href;
    }

    return "";
  }

  mapToChessGameItem(d: any) {
    const i = new ChessGameItem();
    i.white = d.White;
    i.black = d.Black;
    i.round = d.Round;
    i.result = d.Result;
    i.site = d.Site;
    i.event = d.Event;
    i.date = d.Date;
    i.moves = d.Moves;
    return i;
  }
}
