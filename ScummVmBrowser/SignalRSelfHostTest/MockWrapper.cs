using CLIScumm;
using CliScummEvents;
using ManagedCommon.Enums;
using ManagedCommon.Interfaces;
using ManagedCommon.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SignalRSelfHostTest
{
    public class MockWrapper : IWrapper
    {
        public List<ControlKeys> ReceivedControlKeys { get; private set; }
        public List<string> ReceivedStrings { get; private set; }

        bool hasQuit;

        public MockWrapper()
        {
            hasQuit = false;
            ReceivedControlKeys = new List<ControlKeys>();
            ReceivedStrings = new List<string>();
        }

        public CopyRectToScreen OnCopyRectToScreen { get; set; }
        public SaveData OnSaveData { get; set; }
        public bool RedrawWholeScreenOnNextFrame { get; set; }

        public void EnqueueGameEvent(IGameEvent keyboardEvent)
        {
            Type keyboardEventType = keyboardEvent.GetType();

            if (keyboardEvent.GetType() == typeof(SendControlCharacters))
            {
                ReceivedControlKeys.Add(((SendControlCharacters)keyboardEvent).ControlKey);
            }
            else if (keyboardEvent.GetType() == typeof(SendString))
            {
                ReceivedStrings.Add(((SendString)keyboardEvent).StringToSend);
            }
        }

        public void Quit()
        {
            hasQuit = true;
        }

        public void RunGame(AvailableGames game, byte[] gameData, Dictionary<string, byte[]> gameStorage, PlayAudio playAudio)
        {
            while (!hasQuit)
            {
                Task.Delay(100);
            }
        }

        public byte[] GetWholeScreen()
        {
            throw new NotImplementedException();
        }

        public byte[] GetWholeScreen(ref int width, ref int height)
        {
            throw new NotImplementedException();
        }

        public byte[] GetNextSound()
        {
            throw new NotImplementedException();
        }

        public void StartSound()
        {
            throw new NotImplementedException();
        }

        public void StopSound()
        {
            throw new NotImplementedException();
        }

        public Point GetCurrentMousePosition()
        {
            throw new NotImplementedException();
        }
    }
}
