module ScummVMSlices 
{
	interface ScummHubClient {
		["amd"] bool SaveGame(string saveData);
		["amd"] void BackEndQuit();
	}

	interface ScummHub
	{
	 void addClient(ScummHubClient* receiver);
	["amd"] void RunGame(string gameName, string compressedAndEncodedGameSaveData);
	 void Quit();
	}
}
