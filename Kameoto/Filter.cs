using System;
using System.Runtime.InteropServices;
using Raylib_cs;



namespace Kameoto
{
    /// <summary>
    /// テクスチャにエフェクトを掛けるメソッドを集めた静的クラス。
    /// </summary>
    public static class Filter
    {
        private static void RGBToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            v = max;
            float delta = max - min;
            s = (max == 0) ? 0 : delta / max;
            if (delta == 0)
            {
                h = 0;
            }
            else
            {
                if (max == r)
                    h = 60 * (((g - b) / delta) % 6);
                else if (max == g)
                    h = 60 * (((b - r) / delta) + 2);
                else // max == b
                    h = 60 * (((r - g) / delta) + 4);
                if (h < 0) h += 360;
            }
        }
        private static Color HSVToRGB(float h, float s, float v, byte a)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;
            float r, g, b;
            if (h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }
            byte R = (byte)Clamp((int)((r + m) * 255), 0, 255);
            byte G = (byte)Clamp((int)((g + m) * 255), 0, 255);
            byte B = (byte)Clamp((int)((b + m) * 255), 0, 255);
            return new Color(R, G, B, a);
        }
        unsafe private static Image CreateImageFromColors(Color[] pixels, int width, int height)
        {
            Image img = new Image();
            img.Width = width;
            img.Height = height;
            img.Mipmaps = 1;
            img.Format = PixelFormat.UncompressedR8G8B8A8 ;
            int byteCount = width * height * 4;
            byte[] pixelBytes = new byte[byteCount];
            for (int i = 0; i < pixels.Length; i++)
            {
                int offset = i * 4;
                pixelBytes[offset] = pixels[i].R;
                pixelBytes[offset + 1] = pixels[i].G;
                pixelBytes[offset + 2] = pixels[i].B;
                pixelBytes[offset + 3] = pixels[i].A;
            }

            IntPtr ptr = Marshal.AllocHGlobal(byteCount);
            Marshal.Copy(pixelBytes, 0, ptr, byteCount);
            img.Data = (void*)ptr;
            return img;
        }
        unsafe private static Color[] GetImageData(Image img)
        {
            int pixelCount = img.Width * img.Height;
            int byteCount = pixelCount * 4;
            byte[] pixelBytes = new byte[byteCount];
            Marshal.Copy((IntPtr)img.Data, pixelBytes, 0, byteCount);
            Color[] pixels = new Color[pixelCount];
            for (int i = 0; i < pixelCount; i++)
            {
                int offset = i * 4;
                byte r = pixelBytes[offset];
                byte g = pixelBytes[offset + 1];
                byte b = pixelBytes[offset + 2];
                byte a = pixelBytes[offset + 3];
                pixels[i] = new Color(r, g, b, a);
            }
            return pixels;
        }
        private static byte AdjustLevel(byte channel, int inputMin, int inputMax, int gamma, int outputMin, int outputMax)
        {
            int value = channel;
            if (value < inputMin)
                return (byte)outputMin;
            if (value > inputMax)
                return (byte)outputMax;
            float normalized = (value - inputMin) / (float)(inputMax - inputMin);
            float gammaFactor = gamma / 100f; // 100 → 1.0
            normalized = (float)Math.Pow(normalized, gammaFactor);
            int result = outputMin + (int)(normalized * (outputMax - outputMin));
            return (byte)Clamp(result, 0, 255);
        }
        private static void ApplyFilter(ITextureReturnable g, Func<Color, Color> pixelFunc)
        {
            var tex = g.GetTexture();
            Image img = Raylib.LoadImageFromTexture(tex.texture);
            Color[] pixels= GetImageData(img);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = pixelFunc(pixels[i]);
            }
            Image newImg = CreateImageFromColors(pixels, img.Width, img.Height);
            Texture2D newTex = Raylib.LoadTextureFromImage(newImg);
            tex.texture = newTex;
            Raylib.UnloadImage(newImg);
            Raylib.UnloadImage(img);

        }
        /// <summary>
        /// モノトーンフィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="cB">青色差。-255～255の範囲。</param>
        /// <param name="cR">赤色差。-255～255の範囲。</param>
        public static void Monotone(this ITextureReturnable g, int cB, int cR)
        {
            ApplyFilter(g, (col) =>
            {
                int gray = (col.R + col.G + col.B) / 3;
                int newR = Clamp(gray + cR, 0, 255);
                int newB = Clamp(gray + cB, 0, 255);
                return new Color((byte)newR, (byte)gray, (byte)newB, col.A);
            });
            //  Raylib.SetTextureFilter(g.GetTexture().texture, TextureFil);
           // DX.GraphFilter(g.GetTexture().ID, DX.DX_GRAPH_FILTER_MONO, Clamp(cB, -255, 255), Clamp(cR, -255, 255));
        }

        /// <summary>
        /// ガウスフィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="width">ピクセル幅。8, 16, 32のいずれか。</param>
        /// <param name="strength">ぼかしの強さ。100で約1ピクセル分。</param>
        public static void Gauss(this ITextureReturnable g, int width, int strength)
        {
            throw new Exception("未実装です、ごめんなさ");
            if (width != 8 && width != 16 && width != 32)
            {
            //    throw new ArgumentOutOfRangeException();
            }

    //        DX.GraphFilter(g.GetTexture().ID, DX.DX_GRAPH_FILTER_GAUSS, width, strength);
        }

        /// <summary>
        /// 色相フィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="hue">ピクセルの色相からどのくらい変えるか。-180～180の範囲。</param>
        public static void HueRel(this ITextureReturnable g, int hue)
        {
            ApplyFilter(g, (col) =>
            {
                float h, s, v;
                RGBToHSV(col, out h, out s, out v);
                h = (h + hue) % 360;
                if (h < 0) h += 360;
                return HSVToRGB(h, s, v, col.A);
            });
        }

        /// <summary>
        /// 色相フィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="hue">色相。0～360の範囲。</param>
        public static void HueAbs(this ITextureReturnable g, int hue)
        {
            ApplyFilter(g, (col) =>
            {
                float h, s, v;
                RGBToHSV(col, out h, out s, out v);
                h = Clamp(hue, 0, 360);
                return HSVToRGB(h, s, v, col.A);
            });
        }

        /// <summary>
        /// 彩度フィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="saturation">彩度。-255～の範囲。</param>
        public static void Saturation(this ITextureReturnable g, int saturation)
        {
            ApplyFilter(g, (col) =>
            {
                float h, s, v;
                RGBToHSV(col, out h, out s, out v);
                s += saturation / 255f;
                s = s < 0 ? 0 : s > 1 ? 1 : s;
                return HSVToRGB(h, s, v, col.A);
            });
        }

        /// <summary>
        /// 輝度フィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="brightness">輝度。-255～255の範囲。</param>
        public static void Brightness(this ITextureReturnable g, int brightness)
        {
            ApplyFilter(g, (col) =>
            {
                float h, s, v;
                RGBToHSV(col, out h, out s, out v);
                v += brightness / 255f;
                v = v < 0 ? 0 : v > 1 ? 1 : v;
                return HSVToRGB(h, s, v, col.A);
            });
        }

        /// <summary>
        /// HSBフィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="absHue">色相を統一(絶対値を用いる)するかどうか。</param>
        /// <param name="hue">色相。</param>
        /// <param name="saturation">彩度。</param>
        /// <param name="brightness">輝度。</param>
        public static void HSB(this ITextureReturnable g, bool absHue, int hue, int saturation, int brightness)
        {
            ApplyFilter(g, (col) =>
            {
                float h, s, v;
                RGBToHSV(col, out h, out s, out v);
                if (absHue)
                {
                    h = Clamp(hue, 0, 360);
                }
                else
                {
                    h = (h + hue) % 360;
                    if (h < 0) h += 360;
                }
                s += saturation / 255f;
                s = s < 0 ? 0 : s > 1 ? 1 : s;
                v += brightness / 255f;
                v = v < 0 ? 0 : v > 1 ? 1 : v;
                return HSVToRGB(h, s, v, col.A);
            });
        }

        /// <summary>
        /// レベル補正フィルターを適用する。
        /// </summary>
        /// <param name="g">テクスチャ。</param>
        /// <param name="inputMin">入力レベルの最小値。0～255の範囲。</param>
        /// <param name="inputMax">入力レベルの最大値。0～255の範囲。</param>
        /// <param name="gamma">ガンマ値。100が1.0を表し、1～の範囲。</param>
        /// <param name="outputMin">出力レベルの最小値。0～255の範囲。</param>
        /// <param name="outputMax">出力レベルの最大値。0～255の範囲。</param>
        public static void Level(this ITextureReturnable g, int inputMin, int inputMax, int gamma, int outputMin, int outputMax)
        {
            ApplyFilter(g, (col) =>
            {
                byte r = AdjustLevel(col.R, inputMin, inputMax, gamma, outputMin, outputMax);
                byte g_ = AdjustLevel(col.G, inputMin, inputMax, gamma, outputMin, outputMax);
                byte b = AdjustLevel(col.B, inputMin, inputMax, gamma, outputMin, outputMax);
                return new Color(r, g_, b, col.A);
            });
        }

        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            else if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else
            {
                return value;
            }
        }
    }
}
