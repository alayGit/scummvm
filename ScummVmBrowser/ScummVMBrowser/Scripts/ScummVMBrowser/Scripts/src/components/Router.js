"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MainRouter = void 0;
const react_router_dom_1 = require("react-router-dom");
const React = require("react");
const react_1 = require("react");
const testGameFrame_1 = require("./testGameFrame");
const gameScreen_1 = require("./gameScreen");
const utilities_1 = require("../utilities/utilities");
exports.MainRouter = () => {
    const [controlKeys, setControlKeys] = react_1.useState(undefined);
    react_1.useEffect(() => {
        utilities_1.GetResponseFromServer('Api/GetControlKeysJson', "json").then((result) => {
            if (result != undefined) {
                setControlKeys(result);
            }
        });
    }, []);
    if (controlKeys != undefined) {
        const gameScreenProps = {
            controlKeys
        };
        return (React.createElement(react_router_dom_1.BrowserRouter, null,
            React.createElement(react_router_dom_1.Route, { path: "/test", component: testGameFrame_1.TestGameFrame }),
            React.createElement(react_router_dom_1.Route, { path: "/gameScreen", render: () => React.createElement(gameScreen_1.GameScreen, Object.assign({}, gameScreenProps)) })));
    }
    else {
        return React.createElement("div", null, "\"Loading At Router\" ");
    }
};
//# sourceMappingURL=Router.js.map