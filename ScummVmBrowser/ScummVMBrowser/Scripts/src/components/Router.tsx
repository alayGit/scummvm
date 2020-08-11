import { BrowserRouter, Route, Link } from "react-router-dom";
import * as React from "react";
import { useState, useEffect } from "react";
import { TestGameFrame } from "./testGameFrame";
import { GameScreen, GameScreenProps } from "./gameScreen";
import { GetResponseFromServer } from "../utilities/utilities";
export const MainRouter = () => {
    const [controlKeys, setControlKeys] = useState(undefined);

    useEffect(() => {
        GetResponseFromServer<any>('Api/GetControlKeysJson', "json").then((result) => {
            if (result != undefined) {
                setControlKeys(result);
            }
        });
    }, []);

    if (controlKeys != undefined) {
        const gameScreenProps: GameScreenProps = {
            controlKeys
        }

        return (
            <BrowserRouter>
                <Route path="/test" component={TestGameFrame} />
                <Route path="/gameScreen" render={() => <GameScreen {...gameScreenProps} />} />
            </BrowserRouter>
        );
    }
    else {
        return <div>"Loading At Router" </div>;
    }
}

