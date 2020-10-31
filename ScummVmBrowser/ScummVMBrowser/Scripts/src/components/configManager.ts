
import * as SoundSettingsDev from '../../../../JsonResxConfigureStore/Resources/Dev/SoundSettings.json';
import * as IceConfigFrontEndDev from '../../../../JsonResxConfigureStore/Resources/Dev/IceRemoteProcFrontEnd.json'
import * as WebServerSettingsDev from "../../../../JsonResxConfigureStore/Resources/Dev/WebServerSettings.json"


import * as SoundSettingsProd from '../../../../JsonResxConfigureStore/Resources/Prod/SoundSettings.json';
import * as IceConfigFrontEndDevProd from '../../../../JsonResxConfigureStore/Resources/Prod/IceRemoteProcFrontEnd.json'
import * as WebServerSettingsProd from "../../../../JsonResxConfigureStore/Resources/Prod/WebServerSettings.json"

import * as Mode from '../../../../JsonResxConfigureStore/Resources/Mode.json';


export const SoundSettings = () => {
	if (Mode.ProductionMode) {
		return SoundSettingsProd;
	}
	else {
		return SoundSettingsDev;
	}
}

export const IceConfigFrontEnd = () => {
	if (Mode.ProductionMode) {
		return IceConfigFrontEndDevProd;
	}
	else {
		return IceConfigFrontEndDev;
	}
}

export const WebServerSettings = () => {
	if (Mode.ProductionMode) {
		return WebServerSettingsProd;
	}
	else {
		return WebServerSettingsDev;
	}
}
