import * as React from "react";
import { useEffect, useState } from "react";
import * as jQuery from "jquery";
var signalR = require('signalr');

export const TestGameFrame = () => {



    const [frame, setFrame] = useState(undefined);

    const [ScummHub, setScummHub] = useState(undefined);

    var startConnection = async () => {
        var gameInfo = await GetGameInfo();
        var connection = ($ as any).hubConnection(`http://localhost:${(8080)}`);

        var scummHub = connection.createHubProxy('ScummHub');
        setScummHub(scummHub);

        scummHub.on('NextFrame',
            function (frame: string) {
                setFrame(frame);
            }
        );

        connection.start().done(function () {
            if (gameInfo.GameRequiresStarting) {
                scummHub.invoke('Init', 'kq3', '').done(function () {
                    console.log('Invocation of NewContosoChatMessage succeeded');
                }).fail(function (error: string) {
                    console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
                });
            }

        }).fail(function (error: string) {
            console.log('Invocation of start failed. Error:' + error)
        });
    }

    useEffect(
        () => {
            startConnection();

            //$('#gameFrame').bind('keydown', function (event: any) {
            //    onKeyDown(event);
            //});
        },

        []
    );


    var onKeyPress = (event: any) => {
        if (ScummHub != undefined) {
            ScummHub.invoke('EnqueueString', String.fromCharCode(event.which)).done(function () {
                console.log('Invocation of EnqueueControlKey succeeded');
            }).fail(function (error: string) {
                console.log('Invocation of EnqueueControlKey failed. Error: ' + error);
            });
        }
    }

    var onKeyDown = (event: any) => {
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
            ScummHub.invoke('EnqueueControlKey', key)
        }
    }

    if (frame != undefined) {
        return <div id="gameFrame" onKeyPress={onKeyPress} onKeyDown={onKeyDown} tabIndex={0}>
            <img src={`data:image/bmp;base64, ${frame}`} alt="Red dot" />
        </div>
    }
    else {
        return <div>Empty</div>
    }
}

interface GameInfo {
    Port: number;
    GameRequiresStarting: boolean;
}

const GetGameInfo = async (): Promise<GameInfo> => {
    const promise = (url: string): Promise<GameInfo> => {
        return fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error(response.statusText)
                }
                return response.json()
            });
    }
    let gameInfo: GameInfo = undefined;
    await promise(`${window.location.origin}/TestApi/StartGame`).then(function (result) {
        gameInfo = result;
    });

    return gameInfo;
};
