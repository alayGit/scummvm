﻿using ManagedCommon.Delegates;
using ManagedCommon.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
	public interface IWrapper
	{
		void EnqueueGameEvent(IGameEvent keyboardEvent);
		void Quit();
		void RunGame(AvailableGames game, byte[] gameData, string compressedAndEncodedGameSaveData, PlayAudio playSound, uint saveSlotToLoad);
		void ScheduleRedrawWholeScreen();
		SendScreenBuffers SendScreenBuffers { get; set; }
		SaveData OnSaveData { get; set; }
		void StartSound();
		void StopSound();
		Point GetCurrentMousePosition();
	};
}
