"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var rxjs_1 = require("rxjs");
var operators_1 = require("rxjs/operators");
var BaseRepo = /** @class */ (function () {
    function BaseRepo(httpClient, rootUrl) {
        this.httpClient = httpClient;
        this.rootUrl = rootUrl;
    }
    BaseRepo.prototype.count = function () {
        return this.httpClient
            .get(this.rootUrl + "/count", {
            observe: "response",
            responseType: "json"
        })
            .pipe(operators_1.catchError(function (errorResponse) {
            console.error("API Error: ", errorResponse);
            return rxjs_1.throwError(errorResponse);
            ;
        }), operators_1.map(function (response) {
            return +response.body;
        }));
    };
    return BaseRepo;
}());
exports.BaseRepo = BaseRepo;
//# sourceMappingURL=BaseRepo.js.map