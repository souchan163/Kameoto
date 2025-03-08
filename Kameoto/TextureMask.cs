using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kameoto
{
    /// <summary>
    /// テクスチャを使用したマスクを作成し、描画範囲を切り取ります。
    /// </summary>
    public class TextureMask : IDisposable
    {
        /// <summary>
        /// テクスチャを使用したマスクを作成します。
        /// </summary>
        /// <param name="width">マスクの横解像度。</param>
        /// <param name="height">マスクの縦解像度。</param>
        public TextureMask(int width, int height)
        {
            Screen = new VirtualScreen(width, height);
        }

        /// <summary>
        /// マスクを作る。
        /// </summary>
        /// <param name="mask">マスクにする内容。</param>
        /// <returns>TextureMask。</returns>
        public TextureMask CreateMask(Action mask)
        {
            Screen.ClearScreen();
            Screen.Draw(mask);
            return this;
        }

        /// <summary>
        /// マスクを使用して描画する。
        /// </summary>
        /// <param name="masking">マスクする内容。</param>
        /// <param name="reverse">マスクを反転するかどうか。</param>
        /// <returns>TextureMask。</returns>
        public TextureMask Masking(Action masking, bool reverse = false)
        {
            int width = Screen.GetTexture().TextureSize.Width;
            int height = Screen.GetTexture().TextureSize.Height;
            RenderTexture2D contentRT = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(contentRT);
            masking?.Invoke();
            Raylib.EndTextureMode();

            RenderTexture2D finalRT = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(finalRT);
            Raylib.DrawTexture(contentRT.Texture, 0, 0, Color.White);

            Raylib.BeginBlendMode(Raylib_cs.BlendMode.Multiplied);
            Raylib.DrawTexture(Screen.GetTexture().texture, 0, 0, Color.White);
            Raylib.EndBlendMode();
            Raylib.EndTextureMode();
            Screen.UpdateTexture(finalRT.Texture);
            Raylib.UnloadRenderTexture(contentRT);
            Raylib.UnloadRenderTexture(finalRT);


            
            return this;
        }

        /// <summary>
        /// マスク画像を破棄する。
        /// </summary>
        public void Dispose()
        {
            Screen?.Dispose();
        }

        private readonly VirtualScreen Screen;
    }
}
