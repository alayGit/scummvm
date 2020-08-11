module ScummVMSlices 
{
	sequence<byte> GameData;
    dictionary<string, GameData> GameStorage;

	interface ScummHubClient {
		["amd"] bool SaveGame(GameData saveData, string fileName);
		["amd"] void BackEndQuit();
	}

	interface ScummHub
	{
	 void addClient(ScummHubClient* receiver);
	["amd"] void RunGame(string gameName, GameStorage gameStorage);
	 void Quit();
	}
}