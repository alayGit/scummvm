module ScummWebsServerVMSlices
{
    dictionary<string, string> GameStorage;

	interface ScummWebServerClient 
	{
		["amd"] bool SaveGame(string saveData, string fileName);
	}

	interface ScummWebServer
	{
	  void addClient(ScummWebServerClient* receiver, string gameId);
    ["amd"]  void Init(string gameId); //Init and StartProcess
	["amd"] void RunGame (string gameName, string signalrConnectionId, string compressedAndEncodedGameSaveData);
    ["amd"]  void Quit(string signalrConnectionId);
	  string GetControlKeys();
	}
}
