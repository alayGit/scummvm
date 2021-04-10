module ScummVMSlices 
{
	sequence<byte> GameData;
  
	interface ScummHubClient {
		["amd"] bool SaveGame(GameData saveData, string fileName);
		["amd"] void BackEndQuit();
	}

	interface ScummHub
	{
	 void addClient(ScummHubClient* receiver);
	["amd"] void RunGame(string gameName, string compressedAndEncodedGameSaveData);
	 void Quit();
	}
}
