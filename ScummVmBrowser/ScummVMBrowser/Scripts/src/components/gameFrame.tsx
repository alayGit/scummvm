const Width = 320;
const Height = 200;
import * as React from "react";
import ReactDOM = require("react-dom");
import { useState, useEffect } from "react";
import { isNullOrUndefined } from "util";
import { WebServerSettings, ClientSide } from "./configManager";
import useInterval from "./useInterval";

enum InputMessageType {
	TextString,
	MouseClick,
	MouseMove,
	ControlKey
}

interface KeyValuePair {
	Key: string,
	Value: string
}

export const GameFrame = (props: GameFrameProps) => {

	let [offScreenCanvasWorker, setOffScreenCanvasWorker] = useState<Worker>(undefined);
	let [pictureWorker, setPictureWorker] = useState<Worker>(undefined);
	let [eventQueue, setEventQueue] = useState<string>("[]");

	const getEventQueue = () => {
		return JSON.parse(eventQueue) as KeyValuePair[];
	}

	const updateEventQueue = (kvp: KeyValuePair) => {
		let eventQueueArray = getEventQueue();
		console.log("Length of event queue " + eventQueueArray.length);
		if (eventQueueArray.length <= ClientSide().MaxInputMessageQueueLength) {

			eventQueueArray.push(kvp);

			setEventQueue(JSON.stringify(eventQueueArray));
		}
	}

	const clearEventQueue = () => {
		setEventQueue("[]");
	}

	const isEventQueueEmpty = () => {
		return getEventQueue().length == 0;
	}

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

	useInterval(
		function () {
			if (!isEventQueueEmpty()) {
				props.proxy.invoke('EnqueueInputControls', getEventQueue()).done(function () {
					console.log('Invocation of EnqueueControlKey succeeded');
					clearEventQueue();
				}).fail(function (error: string) {
					console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
				});
			}
		}, ClientSide().InputMessageTimerMs);

	useEffect(
		() => {
			if (props.frameSets != undefined && offScreenCanvasWorker != undefined && pictureWorker != undefined) {
				pictureWorker.postMessage({ frameSets: props.frameSets })
			}
		}
		, [props.frameSets, offScreenCanvasWorker, pictureWorker]);


	var onKeyPress = (event: any) => {

		if (props.proxy != undefined) {

			updateEventQueue({ Key: InputMessageType.TextString.toString(), Value: String.fromCharCode(event.which) });

			//props.proxy.invoke('EnqueueString', String.fromCharCode(event.which)).done(function () {
			//    console.log('Invocation of EnqueueControlKey succeeded');
			//}).fail(function (error: string) {
			//    console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
			//});
		}
	}

	var onKeyDown = (event: any) => {
		var key = 0;

		if (props.controlKeys.hasOwnProperty(event.key)) {

			key = props.controlKeys[event.key];
		}

		if (key != 0) {
			updateEventQueue({ Key: InputMessageType.ControlKey.toString(), Value: key.toString() });
			//props.proxy.invoke('EnqueueControlKey', key)
		}
	}

	var onMouseMove = (event: any) => {
		updateEventQueue({ Key: InputMessageType.MouseMove.toString(), Value: `${event.nativeEvent.offsetX},${event.nativeEvent.offsetY}` });
		// props.proxy.invoke('EnqueueMouseMove', event.nativeEvent.offsetX, event.nativeEvent.offsetY);
	}

	var onClick = (event: any) => {
		updateEventQueue({ Key: InputMessageType.MouseClick.toString(), Value: event.button });
	}

	return (<div id="gameFrame" onKeyPress={onKeyPress} onKeyDown={onKeyDown} onMouseMove={onMouseMove} onClick={onClick} tabIndex={0}>
		<canvas id="canvas" width={Width} height={Height} style={{ cursor: "none" }} />
	</div>);
}

export interface GameFrameProps {
	proxy: any;
	frameSets: string;
	controlKeys: any;
}
