using CLIScumm;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Drawing;

namespace CliScummSocketWrapper
{
    public class SocketWrapper : Wrapper
    {
        WrapperHub wrapperHub;
        private SocketWrapper(string gameFileLocation, CopyRectToScreen copyRectToScreen) : base(gameFileLocation, copyRectToScreen)
        {
            wrapperHub = new WrapperHub();
         }

        public void SendImageDownSocket(Bitmap bitmap)
        {
        }

        public static void CreateSocketWrapperSocketWrapper(string gameName)
        {
            CopyRectToScreen copyRectToScreen = (Bitmap bitmap) => { };

            string gameFileLocation = "C:\\ScummVMNew\\ScummVMWeb\\Games\\KQ3"; //To be replaced with game name loading code

            SocketWrapper socketWrapper = new SocketWrapper(gameFileLocation, copyRectToScreen);

            copyRectToScreen = socketWrapper.SendImageDownSocket;
        }

    }
}