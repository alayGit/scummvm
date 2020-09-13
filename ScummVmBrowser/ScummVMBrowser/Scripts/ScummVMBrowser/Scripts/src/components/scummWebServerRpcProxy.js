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
exports.AddClient = exports.Quit = exports.RunGame = exports.Init = exports.InitProxy = void 0;
const ScummHubClientI_1 = require("./ScummHubClientI");
const WebServerSettings = require("../../../../JsonResxConfigureStore/Resources/Dev/WebServerSettings.json");
//const ScummVMWebServer = require("..\..\Proxies\ScummWebServer.js").ScummWebServer;
let _communicator;
let _base;
let _scummWebServer;
function InitProxy(hubName, port) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            _communicator = Ice.initialize();
            _base = _communicator.stringToProxy(`${hubName}${port}:ws -h ${WebServerSettings.ServerRoot} -p ${port}`); //ToDo: Not local host
            _scummWebServer = yield ScummWebsServerVMSlices.ScummWebServerPrx.checkedCast(_base);
            return _scummWebServer;
        }
        catch (ex) {
            if (_communicator) {
                return _communicator.destroy();
            }
            console.log(ex.toString());
        }
    });
}
exports.InitProxy = InitProxy;
function Init(gameId) {
    return __awaiter(this, void 0, void 0, function* () {
        yield _scummWebServer.Init(gameId);
    });
}
exports.Init = Init;
function RunGame(gameName, signalrConnectionId, saveStorage) {
    return __awaiter(this, void 0, void 0, function* () {
        yield _scummWebServer.RunGame(gameName, signalrConnectionId, JSON.stringify(saveStorage));
    });
}
exports.RunGame = RunGame;
function Quit(signalRConnectionId) {
    return __awaiter(this, void 0, void 0, function* () {
        yield _scummWebServer.Quit(signalRConnectionId);
    });
}
exports.Quit = Quit;
function AddClient(gameId, saveCallback) {
    return __awaiter(this, void 0, void 0, function* () {
        //const proxy = await ScummWebsServerVMSlices.ScummWebServerClientPrx.checkedCast(_communicator.stringToProxy(`${hubName}:ws -p ${port}`));
        const adapter = yield _communicator.createObjectAdapter("");
        const proxy = ScummWebsServerVMSlices.ScummWebServerClientPrx.uncheckedCast(adapter.addWithUUID(new ScummHubClientI_1.ScummWebServerClientI(saveCallback)));
        const connection = _scummWebServer.ice_getCachedConnection();
        connection.setAdapter(adapter);
        yield _scummWebServer.addClient(proxy, gameId);
    });
}
exports.AddClient = AddClient;
//# sourceMappingURL=scummWebServerRpcProxy.js.map