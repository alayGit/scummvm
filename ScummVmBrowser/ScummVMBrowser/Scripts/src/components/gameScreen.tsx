import * as React from "react";
import { useEffect, useState } from "react";
import { Redirect } from "react-router-dom";
import { GameFrame, GameFrameProps } from "./gameFrame";
import { WebAudioStreamer } from "./webAudioStreamer";
import { Init, InitProxy, Quit, RunGame, AddClient } from "./scummWebServerRpcProxy"
import { IceConfigFrontEnd, WebServerSettings } from "./configManager";



const RegexGuid = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

export const GameScreen = (props: GameScreenProps) => {

    const [proxy, setProxy] = useState<any>();

	type typeGameState = 'retrieveId' | 'connecting' | 'running' | 'error';

    const [gameState, setGameState] = useState<typeGameState>('retrieveId');
    const [gameId, setGameId] = useState<string>(undefined);
    const [availableGame, setAvailableGame] = useState<string>("");
	let [soundWorker, setSoundWorker] = useState<Worker>(undefined);
    const [webAudioStreamer, setWebAudioStreamer] = useState<WebAudioStreamer>(undefined);
	const [nextAudioSample, setNextAudioSample] = useState<number[]>(undefined);
	let [gameMessageWorker, setGameMessageWorker] = useState<Worker>(undefined);
	let [toGameWorkerChannel, setToGameWorkerChannel] = useState<MessageChannel>(undefined);
	let [fromGameMessageMessageWorkerToSoundWorker, setFromGameMessageMessageWorkerToSoundWorker] = useState<MessageChannel>(undefined);

    const GetSaveStorage = (gameName: string) => {
        return localStorage.getItem(gameName);
    }

    useEffect(
        () => {
			if (gameState == 'connecting') {
				var fromGameMessageMessageWorkerToSoundWorker = new MessageChannel();
				setFromGameMessageMessageWorkerToSoundWorker(toGameWorkerChannel);

				gameMessageWorker = new Worker(`${WebServerSettings().ServerProtocol}://${WebServerSettings().ServerRoot}:${WebServerSettings().ServerPort}/Scripts/gameMessageWorker.js`);

				toGameWorkerChannel = new MessageChannel();
				setToGameWorkerChannel(toGameWorkerChannel);
				gameMessageWorker.postMessage({ toGameWorkerChannel: toGameWorkerChannel.port2, fromGameMessageMessageWorkerToSoundWorker: fromGameMessageMessageWorkerToSoundWorker.port1 }, [toGameWorkerChannel.port2, fromGameMessageMessageWorkerToSoundWorker.port1]);

				setGameMessageWorker(gameMessageWorker);

                var connection = ($ as any).hubConnection(`${window.location.protocol}//${window.location.host}/`);
                var hubServer = connection.createHubProxy('HubServer');

                const saveFunc = function (saveData: string, saveName: string) {
                    try {
                        localStorage.setItem(availableGame, saveData);

                        return true;
                    }
                    catch (Exception) {
                        console.log(Exception);

                        return false;
                    }

                }

				hubServer.on('SendGameMessages',
					function (messages: string) {
						toGameWorkerChannel.port1.postMessage(messages);
                    }
				);

				soundWorker = new Worker(`${WebServerSettings().ServerProtocol}://${WebServerSettings().ServerRoot}:${WebServerSettings().ServerPort}/Scripts/soundProcessorWorker.js`);
				soundWorker.postMessage({ fromGameMessageMessageWorkerToSoundWorker: fromGameMessageMessageWorkerToSoundWorker.port2 }, [fromGameMessageMessageWorkerToSoundWorker.port2]);

				soundWorker.onmessage = function(e) {
					setNextAudioSample(e.data);
                 }

                async function ConnectionAndStart() {
                    await connection.start();
                    await InitProxy(IceConfigFrontEnd().HubName, IceConfigFrontEnd().Port);
                    await AddClient(gameId, saveFunc)
                    await Init(gameId);
                    setProxy(hubServer);
                    await hubServer.invoke('Init', gameId);
                    await RunGame(availableGame, hubServer.connection.id, GetSaveStorage(availableGame));
                    setGameState('running'); //Important otherwise when we turn on sound, we will get a 'nextAudioSample' and connect again

                    setWebAudioStreamer(new WebAudioStreamer(
                        () => {
                            hubServer.invoke('StartSound');
                        },
                        () => {
                            hubServer.invoke('StopSound');
                        }))
                }

                ConnectionAndStart();
            }
        }
        , [gameState, nextAudioSample, gameId, availableGame]);

    useEffect(
        () => {
            if (nextAudioSample != undefined && webAudioStreamer != undefined) {
                webAudioStreamer.pushOntoAudioBuffer(nextAudioSample);
            }
        }, [nextAudioSample, webAudioStreamer]
    )

    switch (gameState) {
        case 'retrieveId':
            const paramOfUnknown = GetParamFromPath(1);
            if (IsGuid(paramOfUnknown)) {
                const guidParam = paramOfUnknown;
                const availableGame = GetParamFromPath(ParamPosition.AvailableGame);
                setAvailableGame(availableGame);
                setGameState('connecting');
                setGameId(paramOfUnknown);
                return (<div>Loading</div>);
            }
            else { //TODO: Check that it is a valid game
                const paramOfAvailableGame = paramOfUnknown;
                const uuidv1 = require('uuid/v1');
                const redirectTo = `/gameScreen/${paramOfAvailableGame}/${uuidv1()}`;
                return (<Redirect to={redirectTo} />);
            }
        case 'connecting':
            return (<div>Connecting</div>);
        case 'running':
            const gameFrameProps: GameFrameProps = {
                proxy,
				controlKeys: props.controlKeys, 
				gameMessageWorker,
            }
            return <GameFrame {...gameFrameProps} />;
        case 'error':
            return (<div>Error</div>);
    }
}

const IsGuid = (potentialGuid: string) => {
    return RegexGuid.test(potentialGuid);
}

const GetParamFromPath = (paramId: ParamPosition) => {
    let path = window.location.pathname;

    const pathPortions: string[] = window.location.pathname.split('/').filter((s) => s != '');

    return pathPortions[pathPortions.length - paramId];
}

export interface GameScreenProps {
    controlKeys: any
}

//From the right to left
enum ParamPosition {
    Guid = 1,
    AvailableGame = 2,
}


