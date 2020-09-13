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
exports.TestGameFrame = void 0;
const React = require("react");
const react_1 = require("react");
var signalR = require('signalr');
exports.TestGameFrame = () => {
    const [frame, setFrame] = react_1.useState(undefined);
    const [ScummHub, setScummHub] = react_1.useState(undefined);
    var startConnection = () => __awaiter(void 0, void 0, void 0, function* () {
        var gameInfo = yield GetGameInfo();
        var connection = $.hubConnection(`http://localhost:${(8080)}`);
        var scummHub = connection.createHubProxy('ScummHub');
        setScummHub(scummHub);
        scummHub.on('NextFrame', function (frame) {
            setFrame(frame);
        });
        connection.start().done(function () {
            if (gameInfo.GameRequiresStarting) {
                scummHub.invoke('Init', 'kq3', '').done(function () {
                    console.log('Invocation of NewContosoChatMessage succeeded');
                }).fail(function (error) {
                    console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
                });
            }
        }).fail(function (error) {
            console.log('Invocation of start failed. Error:' + error);
        });
    });
    react_1.useEffect(() => {
        startConnection();
        //$('#gameFrame').bind('keydown', function (event: any) {
        //    onKeyDown(event);
        //});
    }, []);
    var onKeyPress = (event) => {
        console.log(event.keyCode);
        if (ScummHub != undefined) {
            ScummHub.invoke('EnqueueString', String.fromCharCode(event.which)).done(function () {
                console.log('Invocation of EnqueueControlKey succeeded');
            }).fail(function (error) {
                console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
            });
        }
    };
    var onKeyDown = (event) => {
        var key = 0;
        switch (event.key) {
            case "ArrowUp":
                key = 14;
                break;
            case "ArrowDown":
                key = 15;
                break;
            case "ArrowRight":
                key = 16;
                break;
            case "ArrowLeft":
                key = 17;
                break;
        }
        if (key != 0) {
            ScummHub.invoke('EnqueueControlKey', key);
        }
    };
    if (frame != undefined) {
        return React.createElement("div", { id: "gameFrame", onKeyPress: onKeyPress, onKeyDown: onKeyDown, tabIndex: 0 },
            React.createElement("img", { src: `data:image/bmp;base64, ${frame}`, alt: "Red dot" }));
    }
    else {
        return React.createElement("div", null, "Empty");
    }
};
const GetGameInfo = () => __awaiter(void 0, void 0, void 0, function* () {
    const promise = (url) => {
        return fetch(url)
            .then(response => {
            if (!response.ok) {
                throw new Error(response.statusText);
            }
            return response.json();
        });
    };
    let gameInfo = undefined;
    yield promise(`${window.location.origin}/TestApi/StartGame`).then(function (result) {
        gameInfo = result;
    });
    return gameInfo;
});
//# sourceMappingURL=testGameFrame.js.map