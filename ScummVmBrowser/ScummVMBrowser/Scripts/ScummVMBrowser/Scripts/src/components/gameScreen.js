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
exports.GameScreen = void 0;
const React = require("react");
const react_1 = require("react");
const react_router_dom_1 = require("react-router-dom");
const gameFrame_1 = require("./gameFrame");
const webAudioStreamer_1 = require("./webAudioStreamer");
const scummWebServerRpcProxy_1 = require("./scummWebServerRpcProxy");
const IceConfig = require("../../../../JsonResxConfigureStore/Resources/Dev/IceRemoteProcFrontEnd.json");
const WebServerSettings = require("../../../../JsonResxConfigureStore/Resources/Dev/WebServerSettings.json");
const RegexGuid = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
exports.GameScreen = (props) => {
    const [proxy, setProxy] = react_1.useState();
    const [gameState, setGameState] = react_1.useState('retrieveId');
    const [gameId, setGameId] = react_1.useState(undefined);
    const [frames, setFrame] = react_1.useState(undefined);
    const [availableGame, setAvailableGame] = react_1.useState("");
    //const [saveStorage, setSaveStorage] = useState<object>(undefined);
    let [soundWorker, setSoundWorker] = react_1.useState(undefined);
    const [webAudioStreamer, setWebAudioStreamer] = react_1.useState(undefined);
    const [nextAudioSample, setNextAudioSample] = react_1.useState(undefined);
    const GetSaveStorage = (gameName) => {
        const gameSavesJson = localStorage.getItem(gameName);
        var result = new Object();
        if (gameSavesJson != null) {
            result = JSON.parse(gameSavesJson);
        }
        return result;
    };
    react_1.useEffect(() => {
        if (gameState == 'connecting') {
            var connection = $.hubConnection(`${window.location.protocol}//${window.location.host}/`);
            connection.logging = true;
            var hubServer = connection.createHubProxy('HubServer');
            const saveFunc = function (saveData, saveName) {
                try {
                    const saveStorage = GetSaveStorage(availableGame);
                    saveStorage[saveName] = saveData;
                    localStorage.setItem(availableGame, JSON.stringify(saveStorage));
                    return true;
                }
                catch (Exception) {
                    console.log(Exception);
                    return false;
                }
            };
            hubServer.on('NextFrame', function (pictureUpdates) {
                setFrame(pictureUpdates);
            });
            soundWorker = new Worker(`${WebServerSettings.ServerProtocol}://${WebServerSettings.ServerRoot}:${WebServerSettings.ServerPort}/Scripts/soundProcessorWorker.js`);
            hubServer.on('PlaySound', function (yEncodedData) {
                soundWorker.postMessage(yEncodedData);
                //setNextAudioSample(DecodeYEncode(yEncodedData));
            });
            soundWorker.onmessage = function (e) {
                setNextAudioSample(e.data);
            };
            //connection.start()
            //    .then(function () {
            //        InitProxy("ScummWebServerHub", 5632);
            //    }).then(function () {
            //        Init(gameId);
            //    }).then(function () {
            //        setProxy(hubServer);
            //    }).then(function () {
            //        hubServer.invoke('Init', gameId);
            //    }).then(function () {
            //        RunGame(availableGame, hubServer.connectionId, GetSaveStorage(availableGame));
            //    }).then(function () {
            //        setGameState('running');
            //    }).fail(function (error: string) {
            //        setGameState('error');
            //    });
            function ConnectionAndStart() {
                return __awaiter(this, void 0, void 0, function* () {
                    yield connection.start();
                    yield scummWebServerRpcProxy_1.InitProxy(IceConfig.HubName, IceConfig.Port);
                    yield scummWebServerRpcProxy_1.AddClient(gameId, saveFunc);
                    yield scummWebServerRpcProxy_1.Init(gameId);
                    setProxy(hubServer);
                    yield hubServer.invoke('Init', gameId);
                    yield scummWebServerRpcProxy_1.RunGame(availableGame, hubServer.connection.id, GetSaveStorage(availableGame));
                    setGameState('running'); //Important otherwise when we turn on sound, we will get a 'nextAudioSample' and connect again
                    setWebAudioStreamer(new webAudioStreamer_1.WebAudioStreamer(() => {
                        hubServer.invoke('StartSound');
                    }, () => {
                        hubServer.invoke('StopSound');
                    }));
                });
            }
            ConnectionAndStart();
        }
    }, [gameState, nextAudioSample, gameId, availableGame]);
    react_1.useEffect(() => {
        if (nextAudioSample != undefined && webAudioStreamer != undefined) {
            webAudioStreamer.pushOntoAudioBuffer(nextAudioSample);
        }
    }, [nextAudioSample, webAudioStreamer]);
    switch (gameState) {
        case 'retrieveId':
            const paramOfUnknown = GetParamFromPath(1);
            if (IsGuid(paramOfUnknown)) {
                const guidParam = paramOfUnknown;
                const availableGame = GetParamFromPath(ParamPosition.AvailableGame);
                setAvailableGame(availableGame);
                setGameState('connecting');
                setGameId(paramOfUnknown);
                return (React.createElement("div", null, "Loading"));
            }
            else { //TODO: Check that it is a valid game
                const paramOfAvailableGame = paramOfUnknown;
                const uuidv1 = require('uuid/v1');
                const redirectTo = `/gameScreen/${paramOfAvailableGame}/${uuidv1()}`;
                return (React.createElement(react_router_dom_1.Redirect, { to: redirectTo }));
            }
        case 'connecting':
            return (React.createElement("div", null, "Connecting"));
        case 'running':
            const gameFrameProps = {
                proxy,
                frames,
                controlKeys: props.controlKeys
            };
            return React.createElement(gameFrame_1.GameFrame, Object.assign({}, gameFrameProps));
        case 'error':
            return (React.createElement("div", null, "Error"));
    }
};
const IsGuid = (potentialGuid) => {
    return RegexGuid.test(potentialGuid);
};
const GetParamFromPath = (paramId) => {
    let path = window.location.pathname;
    const pathPortions = window.location.pathname.split('/').filter((s) => s != '');
    return pathPortions[pathPortions.length - paramId];
};
//From the right to left
var ParamPosition;
(function (ParamPosition) {
    ParamPosition[ParamPosition["Guid"] = 1] = "Guid";
    ParamPosition[ParamPosition["AvailableGame"] = 2] = "AvailableGame";
})(ParamPosition || (ParamPosition = {}));
//# sourceMappingURL=gameScreen.js.map