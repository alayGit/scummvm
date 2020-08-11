using ManagedCommon.Delegates;
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
		void RunGame(AvailableGames game, byte[] gameData, Dictionary<string, byte[]> saveData, PlayAudio playSound);
		byte[] GetWholeScreen(ref int width, ref int height);
		CopyRectToScreen OnCopyRectToScreen { get; set; }
		SaveData OnSaveData { get; set; }
		void StartSound();
		void StopSound();
		Point GetCurrentMousePosition();
	};
}
