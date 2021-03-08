using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Actions;
using ManagedCommon.Models;

namespace ManagedCommon.Delegates
{
    public delegate void SendScreenBuffers(List<ScreenBuffer> screenBuffers);
    public delegate Task SendGameMessagesAsync(string gameMessages);
    public delegate bool SaveData(byte[] data, string saveName);
    public delegate Task<bool> SaveDataAsync(byte[] data, string saveName);
	public delegate void PlayAudio(byte[] data);
	public delegate Task Quit();
    public delegate Point GetCurrentMousePosition();
	public delegate void EnqueueInputMessages(InputMessage[] inputMessages);
	public delegate Task MessagesProcessed(string gameMessages);
	public delegate void StartSound();
    public delegate void StopSound();
    public delegate void ScheduleRedrawWholeScreen();
    public delegate bool StartConnection(int port);
    public class Delegates
    {
    }
}
