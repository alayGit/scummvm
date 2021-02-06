const Width = 320;
const Height = 200;
import * as React from "react";
import ReactDOM = require("react-dom");
import { useState, useEffect } from "react";
import { isNullOrUndefined } from "util";
import { WebServerSettings } from "./configManager";

export interface PictureUpdate {
	CompressedBuffer: string,
	CompressedPaletteBuffer: string,
	paletteHash: number,
	ignoreColor: number,
    X: number,
    Y: number,
    W: number,
    H: number,
}

export const GameFrame = (props: GameFrameProps) => {

	let [offScreenCanvasWorker, setOffScreenCanvasWorker] = useState<Worker>(undefined);
	let [pictureWorker, setPictureWorker] = useState<Worker>(undefined);
    useEffect(
		() => {
			var channel = new MessageChannel();
			let canvas = document.getElementById("canvas") as HTMLCanvasElement;
			let pictureWorker = new Worker(`${WebServerSettings().ServerProtocol}://${WebServerSettings().ServerRoot}:${WebServerSettings().ServerPort}/Scripts/dataProcessorWorker.js`);
			let offScreenCanvasWorker = new Worker(`${WebServerSettings().ServerProtocol}://${WebServerSettings().ServerRoot}:${WebServerSettings().ServerPort}/Scripts/offScreenCanvasWorker.js`);
			pictureWorker.postMessage({ workerPort: channel.port2 }, [channel.port2]);
            let offScreenCanvas = canvas.transferControlToOffscreen();
			setPictureWorker(pictureWorker);
			setOffScreenCanvasWorker(offScreenCanvasWorker);

            const transferable: unknown = offScreenCanvas; //To get around a known type script bug
            offScreenCanvasWorker.postMessage({ offScreenCanvas: offScreenCanvas, port: channel.port1 }, [transferable as Transferable, channel.port1])

        }, []);

    useEffect(
        () => {
            if (props.frames != undefined && offScreenCanvasWorker != undefined && pictureWorker != undefined) {
                pictureWorker.postMessage({ frames: props.frames })
            }
        }
        , [props.frames, offScreenCanvasWorker, pictureWorker]);


    var onKeyPress = (event: any) => {

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
    }

    var onClick = (event: any) => {
        props.proxy.invoke('EnqueueMouseClick', event.button);
    }

    return (<div id="gameFrame" onKeyPress={onKeyPress} onKeyDown={onKeyDown} onMouseMove={onMouseMove} onClick={onClick} tabIndex={0}>
		<canvas id="canvas" width={Width} height={Height} style={{cursor:"none" } } />
    </div>);
}

export interface GameFrameProps {
    proxy: any;
    frames: PictureUpdate[][];
    controlKeys: any;
}
