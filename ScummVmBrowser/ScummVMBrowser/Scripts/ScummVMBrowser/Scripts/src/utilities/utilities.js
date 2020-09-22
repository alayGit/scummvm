"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetResponseFromServer = void 0;
function GetResponseFromServer(urlPortion, responseType) {
    return __awaiter(this, void 0, void 0, function* () {
        const promise = (url) => {
            return fetch(url)
                .then(response => {
                if (!response.ok) {
                    throw new Error(response.statusText);
                }
                switch (responseType) {
                    case 'json':
                        return response.json();
                    case 'blob':
                        return response.blob();
                    case 'text':
                        return response.text();
                    case 'nothing':
                        return true;
                    default:
                        throw new Error(`Unknown response type ${responseType}`);
                }
            });
        };
        let returnResult = undefined;
        yield promise(`${window.location.origin}/${urlPortion}`).then(function (result) {
            returnResult = result;
        });
        return returnResult;
    });
}
exports.GetResponseFromServer = GetResponseFromServer;
;
//# sourceMappingURL=utilities.js.map