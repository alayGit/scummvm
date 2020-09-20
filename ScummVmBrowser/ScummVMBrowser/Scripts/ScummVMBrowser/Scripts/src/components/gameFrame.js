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
    let [pictureWorker, setPictureWorker] = react_1.useState(undefined);
    react_1.useEffect(() => {
        var channel = new MessageChannel();
        let canvas = document.getElementById("canvas");
        let pictureWorker = new Worker(`${WebServerSettings.ServerProtocol}://${WebServerSettings.ServerRoot}:${WebServerSettings.ServerPort}/Scripts/dataProcessorWorker.js`);
        let offScreenCanvasWorker = new Worker(`${WebServerSettings.ServerProtocol}://${WebServerSettings.ServerRoot}:${WebServerSettings.ServerPort}/Scripts/offScreenCanvasWorker.js`);
        pictureWorker.postMessage({ workerPort: channel.port2 }, [channel.port2]);
        let offScreenCanvas = canvas.transferControlToOffscreen();
        setPictureWorker(pictureWorker);
        setOffScreenCanvasWorker(offScreenCanvasWorker);
        const transferable = offScreenCanvas; //To get around a known type script bug
        offScreenCanvasWorker.postMessage({ offScreenCanvas: offScreenCanvas, port: channel.port1 }, [transferable, channel.port1]);
    }, []);
    react_1.useEffect(() => {
        if (props.frames != undefined && offScreenCanvasWorker != undefined && pictureWorker != undefined) {
            pictureWorker.postMessage({ frames: props.frames });
        }
    }, [props.frames, offScreenCanvasWorker, pictureWorker]);
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