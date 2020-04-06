"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var http_1 = require("@angular/common/http");
var SortField = /** @class */ (function () {
    function SortField(fieldName, ascending) {
        this.fieldName = fieldName;
        this.ascending = ascending;
    }
    SortField.fromUrl = function (url) {
        var i = url.indexOf("?");
        if (i === -1)
            return [];
        var urlParams = url.substr(i + 1);
        var params = new http_1.HttpParams({ fromString: urlParams });
        var sorting = params.get("order-by");
        if (!sorting)
            return [];
        var fields = new Array();
        for (var _i = 0, _a = sorting.split(","); _i < _a.length; _i++) {
            var clause = _a[_i];
            var parts = clause.split(" ");
            if (parts.length === 1) {
                fields.push(new SortField(parts[0], true));
            }
            else {
                fields.push(new SortField(parts[0], parts[1].toLocaleLowerCase() === "asc"));
            }
        }
        return fields;
    };
    Object.defineProperty(SortField.prototype, "matSortDirection", {
        get: function () {
            return this.ascending ? "asc" : "desc";
        },
        enumerable: true,
        configurable: true
    });
    return SortField;
}());
exports.SortField = SortField;
//# sourceMappingURL=SortField.js.map