"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var GamesList = /** @class */ (function () {
    function GamesList(games, pagination) {
        this.games = games;
        this.pagination = pagination;
        if (pagination) {
            this.currentPage = pagination.currentPage;
            this.nextPage = pagination.nextPage;
            this.previousPage = pagination.previousPage;
            this.pageSize = pagination.pageSize;
            this.totalPages = pagination.totalPages;
        }
    }
    return GamesList;
}());
exports.GamesList = GamesList;
//# sourceMappingURL=GamesList.js.map