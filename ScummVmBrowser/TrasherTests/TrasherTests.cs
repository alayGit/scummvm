using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trasher;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
namespace TrasherTests
{
    [TestClass]
    public class TrasherTests
    {
        [TestMethod]
        public async Task TrasherCallsTrashSubscribersAndThenTrashRoutineWhenTimerExpires()
        {
            using (TrasherState trasherState = new TrasherState())
            {
                await Task.Delay(65000);

                SubscribersCalledInOrder(trasherState);
                Assert.IsTrue(trasherState.Trasher.IsTrashed);
            }
        }

        [TestMethod]
        public async Task TrasherCanPauseAndResume()
        {
            using (TrasherState trasherState = new TrasherState())
            {
                await trasherState.Trasher.PauseTrashCountDown();
                await Task.Delay(65000);
                Assert.IsFalse(trasherState.Trasher.IsTrashed);
                await trasherState.Trasher.ResumeTrashCountDown();
                await Task.Delay(65000);
                Assert.IsTrue(trasherState.Trasher.IsTrashed);

                SubscribersCalledInOrder(trasherState);
            }
        }

        public void SubscribersCalledInOrder(TrasherState trasherState)
        {
            Assert.IsTrue(trasherState.TrashResults.Select(t => t.TimeWhenCalled).Min() == trasherState.TrashResults[trasherState.TrashResults.Count - 1].TimeWhenCalled);
            Assert.IsFalse(trasherState.TrashResults.Any(r => r.TimeWhenCalled == DateTime.MinValue));
        }

    }
}
