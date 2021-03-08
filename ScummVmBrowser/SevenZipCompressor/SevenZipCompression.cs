using ManagedCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SevenZip
{
	public class SevenZipCompression : IMessageCompression
	{
		SevenZipCompressor _sevenZipCompressor;
		public SevenZipCompression()
		{
			string assemblyFullPath = Assembly.GetAssembly(typeof(SevenZipCompression)).Location;
			string assemblyPath = Path.GetDirectoryName(assemblyFullPath);

			SevenZipBase.SetLibraryPath(Path.Combine(assemblyPath, "7za.dll"));
			_sevenZipCompressor = new SevenZipCompressor();
			_sevenZipCompressor.CompressionLevel = CompressionLevel.Ultra;
			_sevenZipCompressor.CompressionMethod = CompressionMethod.Lzma2;
		}

		public byte[] Compress(byte[] input)
		{
			using(MemoryStream mi = new MemoryStream(input))
			{
				using (MemoryStream mo = new MemoryStream())
				{
					_sevenZipCompressor.CompressStream(mi, mo);
					mo.Flush();

					return mo.ToArray();
				}
			}
		}

		public byte[] Decompress(byte[] input)
		{
			using (MemoryStream mi = new MemoryStream(input))
			{
				using(SevenZipExtractor extractor = new SevenZipExtractor(mi))
				{
					using (MemoryStream mo = new MemoryStream())
					{
						extractor.ExtractFile(0, mo);
						mo.Flush();

						return mo.ToArray();
					}
				}
			}
		}
	}
}
