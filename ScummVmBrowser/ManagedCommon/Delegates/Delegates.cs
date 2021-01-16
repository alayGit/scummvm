using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ManagedCommon.Enums;

namespace ManagedCommon.Delegates
{
    public delegate void CopyRectToScreen(List<ScreenBuffer> screenBuffers);
    public delegate Task CopyRectToScreenAsync(List<ScreenBuffer> screenBuffers);
    public delegate bool SaveData(byte[] data, string saveName);
    public delegate Task<bool> SaveDataAsync(byte[] data, string saveName);
    public delegate void PlayAudio(byte[] data);
    public delegate Task PlayAudioAsync(byte[] data);
    public delegate Task Quit();
    public delegate Point GetCurrentMousePosition();
    public delegate void EnqueueString(string toSend);
    public delegate void EnqueueControlKey(ControlKeys toSend);
    public delegate void EnqueueMouseMove(int x, int y);
    public delegate void EnqueueMouseClick(MouseClick mouseClick);
    public delegate void StartSound();
    public delegate void StopSound();
    public delegate void ScheduleRedrawWholeScreen();
    public delegate bool StartConnection(int port);
    public class Delegates
    {
    }
}
