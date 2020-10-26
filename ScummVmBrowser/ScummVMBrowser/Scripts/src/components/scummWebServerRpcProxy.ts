import { ScummWebServerClientI, ISaveCallback } from "./ScummHubClientI";
import { WebServerSettings } from "./configManager";


declare var Ice: any;
declare var ScummWebsServerVMSlices: any;
//const ScummVMWebServer = require("..\..\Proxies\ScummWebServer.js").ScummWebServer;
let _communicator: any;
let _base: any;
let _scummWebServer: any;
export async function InitProxy(hubName:string, port:number) {
    try {
        _communicator = Ice.initialize();
        _base = _communicator.stringToProxy(`${hubName}${port}:ws -h ${WebServerSettings().ServerRoot} -p ${port}`); //ToDo: Not local host
        _scummWebServer = await ScummWebsServerVMSlices.ScummWebServerPrx.checkedCast(_base);
        return _scummWebServer;
    }
    catch (ex) {
        if (_communicator) {
            return _communicator.destroy();
        }
        console.log(ex.toString());       
    }
}

export async function Init(gameId: string) {
    await _scummWebServer.Init(gameId);
}

export async function RunGame(gameName: string, signalrConnectionId: string, saveStorage: object) {
    await _scummWebServer.RunGame(gameName, signalrConnectionId, JSON.stringify(saveStorage));
}

export async function Quit(signalRConnectionId: string) {
    await _scummWebServer.Quit(signalRConnectionId);
}

export async function AddClient(gameId:string, saveCallback: ISaveCallback) {
    //const proxy = await ScummWebsServerVMSlices.ScummWebServerClientPrx.checkedCast(_communicator.stringToProxy(`${hubName}:ws -p ${port}`));
    const adapter = await _communicator.createObjectAdapter("");
    const proxy = ScummWebsServerVMSlices.ScummWebServerClientPrx.uncheckedCast(adapter.addWithUUID(new ScummWebServerClientI(saveCallback)));
    const connection = _scummWebServer.ice_getCachedConnection();
    connection.setAdapter(adapter);
    await _scummWebServer.addClient(proxy, gameId);
}


