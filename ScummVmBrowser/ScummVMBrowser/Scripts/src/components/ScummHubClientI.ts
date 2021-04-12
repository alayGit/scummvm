declare var ScummWebsServerVMSlices: any;

export interface ISaveCallback {
    (saveData: string, fileName: string): boolean
}

export class ScummWebServerClientI extends ScummWebsServerVMSlices.ScummWebServerClient {

    saveCallback: ISaveCallback;

    constructor(saveCallback: ISaveCallback) {
        super();
        this._saveCallback = saveCallback;
    }

    SaveGame(saveData: string, fileName: string) {
       return this._saveCallback(saveData, fileName);
    }

}
