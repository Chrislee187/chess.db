"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Pagination = /** @class */ (function () {
    function Pagination() {
    }
    Pagination.default = {
        currentPage: 1,
        pageSize: 10,
        totalPages: 0,
        nextPage: "",
        previousPage: "",
        sortFields: [{ fieldName: "White", ascending: true }]
    };
    return Pagination;
}());
exports.Pagination = Pagination;
var SortField = /** @class */ (function () {
    function SortField(fieldName, ascending) {
        this.fieldName = fieldName;
        this.ascending = ascending;
    }
    return SortField;
}());
exports.SortField = SortField;
//# sourceMappingURL=Pagination.js.map