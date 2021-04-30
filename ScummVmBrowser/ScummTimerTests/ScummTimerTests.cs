using ManagedCommon.Delegates;
using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ScummTimer;
using System;
using System.Threading.Tasks;

namespace ScummTimerTests
{
    [TestClass]
    public class ScummTimerTests
    {
		ManagedScummTimer _managedScummTimer;
		Mock<ILogger> _logger;

		public ScummTimerTests()
		{
			_logger = new Mock<ILogger>();
			_managedScummTimer = new ManagedScummTimer(_logger.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_managedScummTimer.Dispose();
		}

        [TestMethod]
        public async Task CanScheduleTimedEvent()
		{
			IntPtr expectedIntPtr;
			Mock<ScummTimerCallback> callback;
			ScheduleTimedEvent(out expectedIntPtr, out callback);

			await Task.Delay(1000);

			callback.Verify(e => e.Invoke(It.Is<IntPtr>(x => x == expectedIntPtr)), Times.AtLeast(50));
		}

		[TestMethod]
		public async Task CanRemoveScheduledEvent()
		{
			IntPtr expectedIntPtr;
			Mock<ScummTimerCallback> callback;
			ScheduleTimedEvent(out expectedIntPtr, out callback);
			_managedScummTimer.RemoveTimerProc(callback.Object);

			await Task.Delay(1000);
			int invocationsAfterStop = callback.Invocations.Count;
			await Task.Delay(1000);

			Assert.AreEqual(invocationsAfterStop, callback.Invocations.Count);
		}

		[TestMethod]
		public async Task EventsStopAfterDispose()
		{
			IntPtr expectedIntPtr;
			Mock<ScummTimerCallback> callback;
			ScheduleTimedEvent(out expectedIntPtr, out callback);
			_managedScummTimer.Dispose();

			await Task.Delay(1000);
			int invocationsAfterStop = callback.Invocations.Count;
			await Task.Delay(1000);

			Assert.AreEqual(invocationsAfterStop, callback.Invocations.Count);
		}

		[TestMethod]
		public void CannotAddEventsAfterDispose()
		{
			bool exceptionThrown = false;
			try
			{
				IntPtr expectedIntPtr;
				Mock<ScummTimerCallback> callback;

				_managedScummTimer.Dispose();
				ScheduleTimedEvent(out expectedIntPtr, out callback);
			}
			catch(ObjectDisposedException)
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
		}

		[TestMethod]
		public void CannotRemoveEventsAfterDispose()
		{
			bool exceptionThrown = false;
			try
			{
				Mock<ScummTimerCallback> callback = new Mock<ScummTimerCallback>();

				_managedScummTimer.Dispose();
				_managedScummTimer.RemoveTimerProc(callback.Object);
			}
			catch (ObjectDisposedException)
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
		}

		[TestMethod]
		public void CannotScheduleSameEventWithADifferentId()
		{
			const string IdFirstAdded = "33";
			bool exceptionThrown = false;
			try
			{
				Mock<ScummTimerCallback> testCallBack = new Mock<ScummTimerCallback>();

				_managedScummTimer.InstallTimerProc(testCallBack.Object, 100000, new IntPtr(5), IdFirstAdded);
				_managedScummTimer.InstallTimerProc(testCallBack.Object, 100000, new IntPtr(5), "43");
			}
			catch
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
			_logger.Verify(e => e.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, It.Is<string[]>(a => a.Length == 1 && a[0] == string.Format(ManagedScummTimer.AlreadyExistsIdError, IdFirstAdded))), Times.Once);
		}

		[TestMethod]
		public void CannotScheduleSameEventTwice()
		{
			const string IdFirstAdded = "33";
			bool exceptionThrown = false;
			try
			{
				Mock<ScummTimerCallback> testCallBack = new Mock<ScummTimerCallback>();

				_managedScummTimer.InstallTimerProc(testCallBack.Object, 100000, new IntPtr(5), IdFirstAdded);
				_managedScummTimer.InstallTimerProc(testCallBack.Object, 100000, new IntPtr(5), IdFirstAdded);
			}
			catch
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
			_logger.Verify(e => e.LogMessage(LoggingLevel.Error, LoggingCategory.CliScummSelfHost, ErrorMessage.GeneralErrorCliScumm, It.Is<string[]>(a => a.Length == 1 && a[0] == string.Format(ManagedScummTimer.AlreadyExistsTimerError, IdFirstAdded))), Times.Once);
		}

		private void ScheduleTimedEvent(out IntPtr expectedIntPtr, out Mock<ScummTimerCallback> callback)
		{
			expectedIntPtr = new IntPtr(10);
			callback = new Mock<ScummTimerCallback>();
			_managedScummTimer.InstallTimerProc(callback.Object, 10000, expectedIntPtr, "blah");
		}
	}
}
