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
    Pagination.prototype.toUrlQueryParams = function () {
        var urlParams = "?page-size=" + this.pageSize;
        urlParams += "&page=" + this.currentPage;
        if (this.sortFields.length > 0) {
            var orderByParams = "";
            orderByParams = this.sortFields
                .map(function (x) { return x; })
                .reduce(function (g, field) {
                g += "" + field.fieldName + (field.ascending ? "" : " desc") + ",";
                return g;
            }, "");
            urlParams += "&order-by=" + orderByParams.substr(0, orderByParams.length - 1);
        }
        return urlParams;
    };
    Pagination.default = new Pagination({
        currentPage: 1,
        pageSize: 10,
        totalPages: 0,
        nextPageUrl: "",
        previousPageUrl: "",
        sortFields: [{ fieldName: "White", ascending: true }]
    });
    return Pagination;
}());
exports.Pagination = Pagination;
//# sourceMappingURL=Pagination.js.map