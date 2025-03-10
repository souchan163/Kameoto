﻿using System;
using System.Drawing;
using Raylib_cs;
using Rectangle = Raylib_cs.Rectangle;
using Image = Raylib_cs.Image;
namespace Kameoto
{
    /// <summary>
    /// アトラスを生成します。
    /// 単一の画像から複数のテクスチャを切り出すことで、描画の高速化を実現します。
    /// </summary>
    public class AtlasTexture : IDisposable, ITextureReturnable
    {
        /// <summary>
        /// アトラスを生成します。
        /// 単一の画像から複数のテクスチャを切り出すことで、描画の高速化を実現します。
        /// </summary>
        /// <param name="fileName">アトラスのファイル名。</param>
        public AtlasTexture(string fileName)
        {
            Source = new Texture(fileName);
        }

        /// <summary>
        /// アトラスを生成します。
        /// 単一の画像から複数のテクスチャを切り出すことで、描画の高速化を実現します。
        /// </summary>
        /// <param name="texture">テクスチャ。</param>
        public AtlasTexture(Texture texture)
        {
            Source = texture;
        }

        ~AtlasTexture()
        {
            Dispose();
        }

        /// <summary>
        /// 読み込まれた画像の一部を切り出して、新たにテクスチャを生成します。
        /// </summary>
        /// <param name="rectangle">切り出す範囲。</param>
        /// <returns>テクスチャ。</returns>
        public Texture DerivateTexture(Rectangle rectangle)
        {
            Rectangle rect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            Image img = Raylib.LoadImageFromTexture(Source.texture);
            Raylib.ImageCrop(ref img, rect);
            Texture2D newTx = Raylib.LoadTextureFromImage(img);
            Raylib.UnloadImage(img);

            /*
            var t = DX.DerivationGraph(rectangle.X, rectangle.Y,
                rectangle.Width, rectangle.Height,
                Source.ID);

            return new Texture(t);*/
            return new Texture(newTx);
        }

        /// <summary>
        /// アトラスを破棄する。
        /// </summary>
        public void Dispose()
        {
            Source?.Dispose();
        }

        /// <summary>
        /// アトラス自体のテクスチャを取得する。
        /// </summary>
        /// <returns>アトラス画像本体のTexture。</returns>
        public Texture GetTexture()
        {
            return Source;
        }

        private protected Texture Source;
    }
}
