"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GameFrame = void 0;
const Width = 320;
const Height = 200;
const React = require("react");
const react_1 = require("react");
const WebServerSettings = require("../../../../JsonResxConfigureStore/Resources/Dev/WebServerSettings.json");
exports.GameFrame = (props) => {
    let [offScreenCanvasWorker, setOffScreenCanvasWorker] = react_1.useState(undefined);
    react_1.useEffect(() => {
        let canvas = document.getElementById("canvas");
        let offScreenCanvasWorker = new Worker(`${WebServerSettings.ServerProtocol}://${WebServerSettings.ServerRoot}:${WebServerSettings.ServerPort}/Scripts/offScreenCanvasWorker.js`); //TODO: Get url don't hardcode
        let offScreenCanvas = canvas.transferControlToOffscreen();
        setOffScreenCanvasWorker(offScreenCanvasWorker);
        const transferable = offScreenCanvas; //To get around a known type script bug
        offScreenCanvasWorker.postMessage({ offScreenCanvas: offScreenCanvas }, [transferable]);
    }, []);
    react_1.useEffect(() => {
        if (props.frames != undefined && offScreenCanvasWorker != undefined) {
            offScreenCanvasWorker.postMessage({ frames: props.frames });
        }
    }, [props.frames, offScreenCanvasWorker]);
    var onKeyPress = (event) => {
        console.log(event.keyCode);
        if (props.proxy != undefined) {
            props.proxy.invoke('EnqueueString', String.fromCharCode(event.which)).done(function () {
                console.log('Invocation of EnqueueControlKey succeeded');
            }).fail(function (error) {
                console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
            });
        }
    };
    var onKeyDown = (event) => {
        var key = 0;
        if (props.controlKeys.hasOwnProperty(event.key)) {
            key = props.controlKeys[event.key];
        }
        if (key != 0) {
            props.proxy.invoke('EnqueueControlKey', key);
        }
    };
    var onMouseMove = (event) => {
        props.proxy.invoke('EnqueueMouseMove', event.nativeEvent.offsetX, event.nativeEvent.offsetY);
        console.log(event.nativeEvent.offsetX + " " + event.nativeEvent.offsetY);
    };
    var onClick = (event) => {
        props.proxy.invoke('EnqueueMouseClick', event.button);
    };
    return (React.createElement("div", { id: "gameFrame", onKeyPress: onKeyPress, onKeyDown: onKeyDown, onMouseMove: onMouseMove, onClick: onClick, tabIndex: 0 },
        React.createElement("canvas", { id: "canvas", width: Width, height: Height })));
};
//# sourceMappingURL=gameFrame.js.map