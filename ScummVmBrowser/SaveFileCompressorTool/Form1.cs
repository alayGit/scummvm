using ConfigStore;
using ManagedCommon.Interfaces;
using ManagedCommon.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveFileCompressorTool
{
    public partial class Form1 : Form
    {
		Saving.SaveDataEncoderAndCompressor _compressor;
		IConfigurationStore<System.Enum> _configStore;
		public Form1()
        {
			_compressor = new Saving.SaveDataEncoderAndCompressor(new Base64ByteEncoder.Base64ByteEncoder(), new SevenZCompression.SevenZCompressor(), new JsonConfigStore());
            InitializeComponent();
        }

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				textBox1.Text = _compressor.CompressAndEncode(JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(textBox1.Text));
			}
			catch(Exception ex)
			{

			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				textBox1.Text = JsonConvert.SerializeObject(_compressor.DecompressAndDecode(textBox1.Text.Trim()));
			}
			catch(Exception ex)
			{

			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				var toFill = _compressor.DecompressAndDecode(textBox1.Text);
				int number = int.Parse(toFill.First().Key.Split('.')[1]);
				string prefix = toFill.First().Key.Split('.')[0];
				for (int i = number + 1; i < int.Parse(txtFillAmount.Text); i++)
				{
					toFill[prefix + "." + (i.ToString()).PadLeft(3, '0')] = toFill[prefix + ".001"];
				}

				string x = JsonConvert.SerializeObject(toFill);
				string compressed = _compressor.CompressAndEncode(JsonConvert.DeserializeObject<Dictionary<string, GameSave>>(x));
				File.WriteAllText("c:\\temp\\compressedSaved.txt", compressed);
			}
			catch(Exception ex)
			{

			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			GameSave save = new GameSave() { Data = File.ReadAllBytes(textBox1.Text) };
			Dictionary<string, GameSave> dict = new Dictionary<string, GameSave>();
			dict.Add("kq6.001", save);

			string compressed = _compressor.CompressAndEncode(dict);

			textBox1.Text = compressed;
		}
	}
}
