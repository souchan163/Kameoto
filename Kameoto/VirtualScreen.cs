using System;
using System.Drawing;
using System.Numerics;
using Raylib_cs;
using Rectangle = Raylib_cs.Rectangle;
using Color = Raylib_cs.Color;

namespace Kameoto
{
    /// <summary>
    /// 仮想スクリーン（レンダーターゲット）。
    /// </summary>
    public class VirtualScreen : IDisposable, ITextureReturnable
    {
        /// <summary>
        /// 仮想スクリーンを作成します。
        /// </summary>
        /// <param name="width">横幅。</param>
        /// <param name="height">縦幅。</param>
        public VirtualScreen(int width, int height)
        {
            renderTexture = Raylib.LoadRenderTexture(width, height);
            RRTexture = new Texture();
        }

        /// <summary>
        /// 仮想スクリーンに描画する。
        /// </summary>
        /// <param name="drawing">テクスチャに描画するためのラムダ式。</param>
        /// <returns>VirtualScreen インスタンス自身。</returns>
        public VirtualScreen Draw(Action drawing)
        {
            Raylib.BeginTextureMode(renderTexture);
            drawing?.Invoke();
            Raylib.EndTextureMode();
            return this;
        }

        /// <summary>
        /// 仮想スクリーンにテクスチャを描画します。
        /// </summary>
        /// <param name="texture">描画するテクスチャ。</param>
        /// <param name="x">X 座標。</param>
        /// <param name="y">Y 座標。</param>
        /// <param name="sourceRect">描画範囲（null の場合はテクスチャ全体）。</param>
        /// <returns>VirtualScreen インスタンス自身。</returns>
        [Obsolete("ラムダ式による Draw(Action drawing) を使用してください")]
        public VirtualScreen Draw(Texture2D texture, float x, float y, Rectangle? sourceRect = null)
        {
            if (texture.Id == 0) // 無効なテクスチャの場合は何もしない
            {
                return this;
            }

            Raylib.BeginTextureMode(renderTexture);
            if (sourceRect.HasValue)
            {
                Rectangle src = sourceRect.Value;
                Raylib.DrawTextureRec(texture, src, new Vector2(x, y), Color.White);
            }
            else
            {
                Raylib.DrawTexture(texture, (int)x, (int)y, Color.White);
            }

            Raylib.EndTextureMode();
            RRTexture.texture = renderTexture.Texture;

            return this;
        }

        /// <summary>
        /// 仮想スクリーンの内容をクリアします。
        /// </summary>
        public void ClearScreen()
        {
            Raylib.BeginTextureMode(renderTexture);
            //真っ黒に
            Raylib.ClearBackground(Color.Black);
            Raylib.EndTextureMode();
        }

        /// <summary>
        /// 仮想スクリーンを破棄します。
        /// </summary>
        public void Dispose()
        {
            Raylib.UnloadRenderTexture(renderTexture);
        }

        /// <summary>
        /// 仮想スクリーンのテクスチャ（描画結果）を取得します。
        /// </summary>
        /// <returns>描画結果のテクスチャ。</returns>
        public Texture GetTexture()
        {
            return  RRTexture;
        }

        Texture ITextureReturnable.GetTexture()
        {
            throw new NotImplementedException();
        }

        public void UpdateTexture(Texture2D uTx)
        {
            //  this.renderTexture.Texture = new Texture2D();
            RRTexture.texture=uTx;
        }

        /// <summary>
        /// 仮想スクリーンのサイズ。
        /// </summary>
        public Size ScreenSize
        {
            get
            {
                return new Size(renderTexture.Texture.Width, renderTexture.Texture.Height);
            }
        }

        /// <summary>
        /// 内部で使用するレンダーターゲット。
        /// </summary>
        public RenderTexture2D renderTexture { get; set; }

        /// <summary>
        /// RaylibのRenderTextureをKameotoのTexture型にしたやつ
        /// newするよりマシだと信じたい
        /// </summary>
        public Texture RRTexture;
    }
}
