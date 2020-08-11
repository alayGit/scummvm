using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMocks
{
    public class ExpectedNextFrameResult
    {
       public byte[] PicBuff { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public override bool Equals(object obj)
        {
            ExpectedNextFrameResult expectedNextFrameResult = obj as ExpectedNextFrameResult;

            return expectedNextFrameResult != null
                && this.PicBuff.SequenceEqual(expectedNextFrameResult.PicBuff)
                && this.X == expectedNextFrameResult.X
                && this.Y == expectedNextFrameResult.Y
                && this.W == expectedNextFrameResult.W
                && this.H == expectedNextFrameResult.H;
        }
    }
}
