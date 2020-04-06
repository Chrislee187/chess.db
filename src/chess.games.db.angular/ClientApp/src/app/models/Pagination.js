"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Pagination = /** @class */ (function () {
    function Pagination(pagination) {
        this.currentPage = pagination.currentPage;
        this.pageSize = pagination.pageSize;
        this.totalPages = pagination.totalPages;
        this.nextPageUrl = pagination.nextPageUrl;
        this.previousPageUrl = pagination.previousPageUrl;
        this.sortFields = pagination.sortFields;
    }
    Pagination.parseJson = function (json) {
        return new Pagination(JSON.parse(json));
    };
    Pagination.default = function (sortFields) {
        return new Pagination({
            currentPage: 1,
            pageSize: 10,
            totalPages: 0,
            nextPageUrl: "",
            previousPageUrl: "",
            sortFields: sortFields
        });
    };
    Pagination.prototype.toUrlQueryParams = function () {
        var urlParams = "?page-size=" + this.pageSize;
        urlParams += "&page=" + this.currentPage;
        if (this.sortFields.length > 0) {
            var orderByParams = this.sortFields
                .reduce(function (g, field) {
                g += "" + field.fieldName + (field.ascending ? "" : " desc") + ",";
                return g;
            }, "");
            urlParams += "&order-by=" + orderByParams.substr(0, orderByParams.length - 1);
        }
        return urlParams;
    };
    return Pagination;
}());
exports.Pagination = Pagination;
//# sourceMappingURL=Pagination.js.map