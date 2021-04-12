using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveFileCompressorTool
{
    public partial class Form1 : Form
    {
		Saving.SaveDataEncoderAndCompressor _compressor;
        public Form1()
        {
			_compressor = new Saving.SaveDataEncoderAndCompressor(new ManagedYEncoder.ManagedYEncoder(new Mock<ILogger>().Object, ManagedCommon.Enums.Logging.LoggingCategory.CliScummSelfHost), new SevenZCompression.SevenZCompressor());
            InitializeComponent();
        }

		private void button2_Click(object sender, EventArgs e)
		{
			textBox1.Text = _compressor.CompressAndEncode(JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(textBox1.Text));
		}

		private void button1_Click(object sender, EventArgs e)
		{
			textBox1.Text = JsonConvert.SerializeObject(_compressor.DecompressAndDecode(textBox1.Text));
		}
	}
}
