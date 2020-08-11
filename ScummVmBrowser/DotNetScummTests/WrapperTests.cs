using CLIScumm;
using ConfigStore;
using ManagedCommon.Enums;
using ManagedCommon.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetScummTests
{
    [TestClass]
    public class WrapperTests
    {
        [TestMethod]
        public async Task NoSubscribersToOnCopyRectToScreenDoesNotCauseException()
        {
           Wrapper wrapper = new Wrapper(new JsonConfigStore());

            Task runningGameTask = Task.Run(() =>
            { 
                wrapper.RunGame(AvailableGames.kq3, null, null, async (byte[] aud) => { await Task.CompletedTask; });
            });
            wrapper.Quit();

            await runningGameTask;

            Assert.IsFalse(runningGameTask.IsFaulted);
        }

    }
}
