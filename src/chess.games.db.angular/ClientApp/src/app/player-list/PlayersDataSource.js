"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var rxjs_1 = require("rxjs");
var PlayersDataSource = /** @class */ (function () {
    function PlayersDataSource(playerRepo) {
        this.playerRepo = playerRepo;
        this.playersSubject = new rxjs_1.BehaviorSubject([]);
    }
    PlayersDataSource.prototype.connect = function (collectionViewer) {
        return this.playersSubject.asObservable();
    };
    PlayersDataSource.prototype.disconnect = function (collectionViewer) {
        this.playersSubject.complete();
    };
    PlayersDataSource.prototype.loadPlayers = function (sortFields) {
        var _this = this;
        this.playerRepo.load2()
            .subscribe(function (data) { return _this.playersSubject.next(data); });
    };
    return PlayersDataSource;
}());
exports.PlayersDataSource = PlayersDataSource;
//# sourceMappingURL=PlayersDataSource.js.map