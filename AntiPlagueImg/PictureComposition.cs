using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AntiPlagueImg
{
    public abstract class Composition
    {
        protected string CodeComposition;
        protected int Height;
        protected int Width;
        protected Tuple<byte, byte, byte, byte> NormColor;
    }

    public class PictureComposition : Composition
    {
        public string GetCodeComposition { get { return CodeComposition; } }

        public bool SaveCodeCompositionPicture(string path)
        {
            try
            {
                short count = 15;
                uint h = (uint)Height;
                uint w = (uint)Width;
                byte[] hb = new byte[4];
                byte[] wb = new byte[4];

                for (; count >= 0;)
                {
                    if (h - (int)Math.Pow(2, count) >= 0)
                    {
                        h -= (uint)Math.Pow(2, count);
                        hb[count / 8] += (byte)(Math.Pow(2, count) / Math.Pow(2, 8 * (count / 8)));
                    }
                    if (w - (int)Math.Pow(2, count) >= 0)
                    {
                        w -= (uint)Math.Pow(2, count);
                        wb[count / 8] += (byte)(Math.Pow(2, count) / Math.Pow(2, 8 * (count / 8)));
                    }
                    count--;
                }

                hb = hb.Concat(wb).ToArray();
                hb = hb.Concat(Encoding.Default.GetBytes(CodeComposition)).ToArray();

                using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] buffer = hb;
                    // запись массива байтов в файл
                    fstream.Write(buffer, 0, buffer.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public void CreateCompositionPicture(string path)
        {
            var img = new Bitmap(this.Width, this.Height);
            for (int i = CodeComposition.Length - 1; i >= 0; i--)
            {
                byte b1 = (byte)CodeComposition[i];
                img.SetPixel(i % this.Width, i / this.Height, Color.FromArgb(b1, b1, b1));
            }
            img.Save(path);
        }

        public PictureComposition(string composition, Tuple<byte, byte, byte, byte> normColor, int h, int w)
        {
            this.CodeComposition = composition;
            this.NormColor = normColor;
            this.Height = h;
            this.Width = w;
        }

        public PictureComposition(string path, bool IsFileComposition, int h, int w)
        {
            try
            {
                this.Height = h;
                this.Width = w;
                if (!IsFileComposition)
                {
                    WebClient client = new WebClient();
                    Console.WriteLine("Файл скачивается...");
                    Stream stream = client.OpenRead(path);
                    Console.WriteLine("Файл скачался, начинается первичная обработка");
                    var img = Image.FromStream(stream);
                    var img1 = new Bitmap(img, new Size(w, h));
                    NormColor = Average(ref img1);

                    for (int i = 0; i < img1.Height; i++)
                    {
                        for (int j = 0; j < img1.Width; j++)
                        {
                            byte R = img1.GetPixel(j, i).R;
                            byte G = img1.GetPixel(j, i).G;
                            byte B = img1.GetPixel(j, i).B;
                            byte SredRGB = (byte)((R + G + B) / 3);
                            CodeComposition += (char)SredRGB;
                        }
                    }
                    Height = img1.Height;
                    Width = img1.Width;
                   Console.WriteLine("Композиция изображения создана");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Tuple<byte, byte, byte, byte> Average(ref Bitmap img1)
        {
            var sumColors = (0.0, 0.0, 0.0, 0.0);
            for (int i = 0; i < img1.Height; i++)
                for (int j = 0; j < img1.Width; j++)
                {
                    sumColors.Item1 += (UInt32)img1.GetPixel(j, i).A;
                    sumColors.Item2 += (UInt32)img1.GetPixel(j, i).R;
                    sumColors.Item3 += (UInt32)img1.GetPixel(j, i).G;
                    sumColors.Item4 += (UInt32)img1.GetPixel(j, i).B;
                }
            var c0 = ((byte)(sumColors.Item1 / (img1.Width * img1.Height)), (byte)(sumColors.Item2 / (img1.Width * img1.Height)), (byte)(sumColors.Item3 / (img1.Width * img1.Height)), (byte)(sumColors.Item4 / (img1.Width * img1.Height)));
            return c0.ToTuple();
        }

        private double EqualDjaroVinkler(string s, string es, bool procent = false)
        {
            string s1 = s;
            string s2 = es;

            if (s1 == s2) return procent == true ? 100 : 1;

            var a = s1.Length;
            var b = s2.Length;
            // var d = Math.Truncate(Convert.ToDouble(Math.Max(a, b) / 2)) - 1;
            var m = 0d;
            var tr = 0d;
            var L = 0d;
            var p = 0.1;
            var bt = 0.8; // порог усиления
            var dw = 0d;
            var bf = new Dictionary<int, char>();
            var modR = 1;
            for (int i = 0; i < a; i++)
            {
                var y0 = (i + 1) % Height == 0 ? (i + 1) / Height : (i + 1) / Height + 1;
                var x0 = (i + 1) - Width * (y0 - 1);

                if ((i + 1) % (int)(a * 0.1) == 0)
                    Console.WriteLine("Сравнение изображений завершено на " + (int)(((float)i / (float)a) * 100) + "%");
                for (int j = 0; j < b; j++)
                {

                    var y = (j + 1) % Height == 0 ? (j + 1) / Height : (j + 1) / Height + 1;
                    var x = (j + 1) - Width * (y - 1);
                    if ((Height * Width) / modR < x * x + y * y - x0 * x0 - y0 * y0)
                        if (y > (Height + y0) / modR) break;
                        else continue;

                    if (s1[i] == s2[j])
                    {
                        if (bf.ContainsKey(j)) { if (bf[j] == s1[i]) continue; }
                        bf.Add(j, s1[i]);
                        m += 1;
                        if (i != j) tr += 1;
                        if (i == j)
                        {
                            if (i == 0) L = 1;
                            else if ((L <= 3) && (i == L + 1)) L += 1;
                        }
                        break;
                    }
                }
            }
            if (m > 0)
            {
                var t = Math.Truncate(tr / 2);
                var dj = (m / a + m / b + (m - t) / m) / 3;
                if (dj >= bt) dw = dj + (L * p * (1 - dj));
                else dw = dj;
            }
            Console.WriteLine("Сравнение изображений завершено на 100%");
            return procent == true ? dw * 100 : dw;
        }

        public double EqualsF(PictureComposition another)
        {
            return EqualDjaroVinkler(CodeComposition, another.GetCodeComposition, true);
        }
    }
}
