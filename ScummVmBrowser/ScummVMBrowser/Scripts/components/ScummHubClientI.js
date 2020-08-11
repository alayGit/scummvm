"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class ScummWebServerClientI extends ScummWebsServerVMSlices.ScummWebServerClient {
    constructor(saveCallback) {
        super();
        this._saveCallback = saveCallback;
    }
    SaveGame(saveData, fileName) {
        return this._saveCallback(saveData, fileName);
    }
}
exports.ScummWebServerClientI = ScummWebServerClientI;
//# sourceMappingURL=ScummHubClientI.js.map