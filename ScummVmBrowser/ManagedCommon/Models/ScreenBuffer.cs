using ManagedCommon.Enums.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCommon.Interfaces
{
    [Serializable]
    public class ScreenBuffer
    {
        public ScreenBuffer()
        {
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public byte[] PictureBuffer { get; set; }
		public byte[] PaletteBuffer { get; set; }
		public uint PaletteHash { get; set; }
		public int IgnoreColour { get; set; }
    }
}
