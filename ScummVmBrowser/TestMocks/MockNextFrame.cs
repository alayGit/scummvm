using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMocks
{
   public class MockNextFrame
    {
        Queue<ExpectedNextFrameResult> _resultsQueue;

        public bool NextFrameCalledCorrectNumberOfTimes { get; set; }
        public bool EveryFrameMatched { get; set; }

        public MockNextFrame(IEnumerable<ExpectedNextFrameResult> expectedResults)
        {
            _resultsQueue = new Queue<ExpectedNextFrameResult>(expectedResults);

            NextFrameCalledCorrectNumberOfTimes = false;
            EveryFrameMatched = true;
        }


        public void NextFrame (List<ScreenBuffer> results)
        {
            ExpectedNextFrameResult expectedNextFrame = _resultsQueue.Dequeue();
            ExpectedNextFrameResult actualResult = new ExpectedNextFrameResult() { PicBuff = results[0].Buffer, H = results[0].H, W = results[0].W, X = results[0].X, Y = results[0].Y };

            EveryFrameMatched = EveryFrameMatched && expectedNextFrame.Equals(actualResult);
            NextFrameCalledCorrectNumberOfTimes = _resultsQueue.Count == 0;
        }
    }
}
