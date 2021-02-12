using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLIScumm;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using ManagedCommon.Enums;
using ConfigStore;
using ManagedCommon.Interfaces;
using ManagedCommon.Implementations;

namespace DotNetScummTests
{
    /// <summary>
    /// Summary description for WrapperTests
    /// </summary>
    /// 

    [TestClass]
    public class WrapperMultiStartTests
    {
        private bool _start;
        Wrapper _wrapper;
        public WrapperMultiStartTests(): base()
        {
            _start = false;
            _wrapper = new Wrapper(new JsonConfigStore());
            _wrapper.SendScreenBuffers+= (List<ScreenBuffer> screenBuffers) => { };
        }

      

        [TestMethod]
        public async Task DoesNotStartMultipleTimes()
        {
            List<Task> taskList = new List<Task>();
            const int NoThreads = 3;

            for (int i = 0; i < NoThreads; i++)
            {
               taskList.Add(Task.Run(() => RunWrapper()));
            }
            _start = true;

            await Task.Delay(2000);

            Assert.IsTrue(taskList.Where(t => t.IsCompleted).Count() == NoThreads - 1);
            
            Task runningTask = taskList.Single(t => !t.IsCompleted); 
            _wrapper.Quit();

            await runningTask;
        }

        private void RunWrapper()
        {
            while (!_start)
            {
                Thread.Sleep(15);
            }
            _wrapper.RunGame(AvailableGames.kq3, null, null, async (aud) => { await Task.CompletedTask; });
        }
    }
}
