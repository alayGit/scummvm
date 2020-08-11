const Width = 320;
const Height = 200;
import * as React from "react";
import ReactDOM = require("react-dom");
import { useState, useEffect } from "react";
import { isNullOrUndefined } from "util";
import * as WebServerSettings from "../../../../JsonResxConfigureStore/Resources/Dev/WebServerSettings.json"

export interface PictureUpdate {
    Buffer: string,
    X: number,
    Y: number,
    W: number,
    H: number,
    DrawingAction:string
}

export const GameFrame = (props: GameFrameProps) => {

    let [offScreenCanvasWorker, setOffScreenCanvasWorker] = useState<Worker>(undefined);
    useEffect(
        () => {
            let canvas = document.getElementById("canvas") as HTMLCanvasElement;
            let offScreenCanvasWorker = new Worker(`${WebServerSettings.ServerProtocol}://${WebServerSettings.ServerRoot}:${WebServerSettings.ServerPort}/Scripts/offScreenCanvasWorker.js`); //TODO: Get url don't hardcode
            let offScreenCanvas = canvas.transferControlToOffscreen();
            setOffScreenCanvasWorker(offScreenCanvasWorker);

            const transferable: unknown = offScreenCanvas; //To get around a known type script bug
            offScreenCanvasWorker.postMessage({ offScreenCanvas: offScreenCanvas }, [transferable as Transferable])

        }, []);

    useEffect(
        () => {
            if (props.frames != undefined && offScreenCanvasWorker != undefined) {
                offScreenCanvasWorker.postMessage({ frames: props.frames })
            }
        }
        , [props.frames, offScreenCanvasWorker]);


    var onKeyPress = (event: any) => {
        console.log(event.keyCode);

        if (props.proxy != undefined) {
            props.proxy.invoke('EnqueueString', String.fromCharCode(event.which)).done(function () {
                console.log('Invocation of EnqueueControlKey succeeded');
            }).fail(function (error: string) {
                console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
            });
        }
    }

    var onKeyDown = (event: any) => {
        var key = 0;

        if (props.controlKeys.hasOwnProperty(event.key)) {
            key = props.controlKeys[event.key];
        }

        if (key != 0) {
            props.proxy.invoke('EnqueueControlKey', key)
        }
    }

    var onMouseMove = (event: any) => {
        props.proxy.invoke('EnqueueMouseMove', event.nativeEvent.offsetX, event.nativeEvent.offsetY);
        console.log(event.nativeEvent.offsetX + " " + event.nativeEvent.offsetY);
    }

    var onClick = (event: any) => {
        props.proxy.invoke('EnqueueMouseClick', event.button);
    }

    return (<div id="gameFrame" onKeyPress={onKeyPress} onKeyDown={onKeyDown} onMouseMove={onMouseMove} onClick={onClick} tabIndex={0}>
        <canvas id="canvas" width={Width} height={Height} />
    </div>);
}

export interface GameFrameProps {
    proxy: any;
    frames: PictureUpdate[];
    controlKeys: any;
}