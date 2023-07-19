"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var http_1 = require("@angular/common/http/http");
var operators_1 = require("rxjs/operators");
var GamesListRepo = /** @class */ (function () {
    function GamesListRepo(httpClient) {
        this.httpClient = httpClient;
        this.games = [
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
    }
    GamesListRepo.prototype.loadGames = function (url) {
        var _this = this;
        if (url === "")
            url = "http://localhost:5000/api/games";
        // TODO: Use an injected repository rather than HttpClient directly
        return this.httpClient
            .get(url, {
            observe: 'response',
            responseType: 'json'
        })
            .pipe(operators_1.map(function (response) {
            _this.currentPage = _this.getCurrentPageNumberFromUrl(url);
            _this.nextPage = _this.getLink('next-page', response.body._links);
            _this.previousPage = _this.getLink('prev-page', response.body._links);
            // TODO: Need access to the X-Pagination header for the rest
            // first/last, page number, total pages etc.
            // Can't seem to get around a CORS restriction, something in the webapi startup not quiet right
            _this.games = response.body.value.map(function (d) {
                return _this.mapToChessGameItem(d);
            });
            return true;
        }));
    };
    GamesListRepo.prototype.getCurrentPageNumberFromUrl = function (url) {
        if (url.indexOf('?') > -1) {
            var httpParams = new http_1.HttpParams({ fromString: url.split('?')[1] });
            return +httpParams.get('page');
        }
        else {
            return 1;
        }
    };
    GamesListRepo.prototype.getLink = function (name, links) {
        var t = links.find(function (l) { return l.rel === name; });
        if (t) {
            return t.href;
        }
        return "";
    };
    GamesListRepo.prototype.mapToChessGameItem = function (d) {
        return {
            white: d.White,
            black: d.Black,
            round: d.Round,
            result: d.Result,
            site: d.Site,
            event: d.Event,
            date: d.Date,
            moves: d.Moves
        };
    };
    return GamesListRepo;
}());
exports.GamesListRepo = GamesListRepo;
//# sourceMappingURL=GamesListRepo.js.map