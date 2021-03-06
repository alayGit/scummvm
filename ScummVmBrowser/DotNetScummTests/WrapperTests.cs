﻿using CLIScumm;
using ConfigStore;
using ManagedCommon.Constants;
using ManagedCommon.Enums;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Implementations;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
			ILogger logger = new Mock<ILogger>().Object;
			Wrapper wrapper = new Wrapper(new JsonConfigStore(), new Mock<ISaveCache>().Object, new Base64ByteEncoder.Base64ByteEncoder(), new ScummTimer.ManagedScummTimer(new Mock<ILogger>().Object));

            Task runningGameTask = Task.Run(() =>
            { 
                wrapper.RunGame(AvailableGames.kq3, null, null, async (byte[] aud) => { await Task.CompletedTask; }, Constants.DoNotLoadSaveSlot);
            });
            wrapper.Quit();

            await runningGameTask;

            Assert.IsFalse(runningGameTask.IsFaulted);
        }

    }
}
