"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var GamesList = /** @class */ (function () {
    function GamesList(games, currentPageUrl, pagination) {
        this.games = games;
        this.currentPageUrl = currentPageUrl;
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
    GamesList.prototype.buildUrlToAnotherPage = function (currentPageUrl, newPage) {
        return currentPageUrl
            ? currentPageUrl.replace("page=" + this.currentPage, "page=" + newPage)
            : "";
    };
    return GamesList;
}());
exports.GamesList = GamesList;
//# sourceMappingURL=GamesList.js.map