using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalaag {
    public static class DataReader {
        /// <summary>
        /// leest binary data van een file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/18331349/net-4-5-file-read-performance-sync-vs-async"/>
        public static async Task<MemoryStream> ReadAllFileAsync(string filename) {
            MemoryStream ms = new MemoryStream();
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
                file.Position = 0;
                file.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// verwerking "raw data" in mnist files
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <see cref="http://yann.lecun.com/exdb/mnist/"/>
        public static async Task<List<double[]>> ReadMnistAsync(string filename) {
            MemoryStream ms = await ReadAllFileAsync(filename);
            byte[] buff = new byte[ms.Length];
            ms.Read(buff, 0, buff.Length);
            int magicnum = (buff[0] << 24) + (buff[1] << 16) + (buff[2] << 8) + (buff[3]);
            int count = (buff[4] << 24) + (buff[5] << 16) + (buff[6] << 8) + (buff[7]);
            switch (magicnum) {
                case 2051: // images; row & col altijd 28
                    //.Skip(16).Take(maxcount)...
                    return buff.Skip(16).AsParallel().Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / (28*28))
                        .Select(x => x.Select(v => (double)v.Value).ToArray())
                        .ToList();
                case 2049: // labels
                    //return buff.Skip(8).AsParallel().Select(x => new double[] { x }).ToList();
                    return (from x in buff select new double[] { x }).Skip(8).ToList();
            }
            return null;
        }

        public static async Task<List<double[]>> ReadMnistFractionedAsync(string filename, int skippedImages, int images) {
            MemoryStream ms = await ReadAllFileAsync(filename);
            byte[] buff = new byte[ms.Length];
            ms.Read(buff, 0, buff.Length);
            //byte[] buff = await ReadAllFileAsync(filename);
            int magicnum = (buff[0] << 24) + (buff[1] << 16) + (buff[2] << 8) + (buff[3]);
            int count = (buff[4] << 24) + (buff[5] << 16) + (buff[6] << 8) + (buff[7]);
            switch (magicnum) {
                case 2051: // images; row & col altijd 28
                    //.Skip(16).Take(maxcount)...
                    return buff.Skip(16).Skip(28*28*skippedImages).Take(28 * 28 * images).AsParallel().Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / (28 * 28))
                        .Select(x => x.Select(v => (double)v.Value).ToArray())
                        .ToList();
                case 2049: // labels
                    //return buff.Skip(8).AsParallel().Select(x => new double[] { x }).ToList();
                    return (from x in buff select new double[] { x }).Skip(8).Skip(skippedImages).Take(images).ToList();
            }
            return null;
        }
    }
}
