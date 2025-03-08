using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using Rectangle = Raylib_cs.Rectangle;
using Color = Raylib_cs.Color;

namespace Kameoto
{
    /// <summary>
    /// リサイズ可能なウィンドウ上のテクスチャの描画機能を提供するクラス。
    /// 一般的に 9 スライスなどと呼ばれています。
    /// </summary>
    public class ResizeableBoxTexture : IDisposable
    {
        public ResizeableBoxTexture(int cornerWidth, int cornerHeight, Texture texture)
        {
            var atlas = new AtlasTexture(texture);

            TopLeft = atlas.DerivateTexture(new Rectangle(0, 0, cornerWidth, cornerHeight));
            Top = atlas.DerivateTexture(new Rectangle(cornerWidth, 0, texture.TextureSize.Width - (cornerWidth * 2), cornerHeight));
            TopRight = atlas.DerivateTexture(new Rectangle(texture.TextureSize.Width - cornerWidth, 0, cornerWidth, cornerHeight));

            Left = atlas.DerivateTexture(new Rectangle(0, cornerHeight, cornerWidth, texture.TextureSize.Height - (cornerHeight * 2)));
            Center = atlas.DerivateTexture(new Rectangle(cornerWidth, cornerHeight, texture.TextureSize.Width - (cornerWidth * 2), texture.TextureSize.Height - (cornerHeight * 2)));
            Right = atlas.DerivateTexture(new Rectangle(texture.TextureSize.Width - cornerWidth, cornerHeight, cornerWidth, texture.TextureSize.Height - (cornerHeight * 2)));

            BottomLeft = atlas.DerivateTexture(new Rectangle(0, texture.TextureSize.Height - cornerHeight, cornerWidth, cornerHeight));
            Bottom = atlas.DerivateTexture(new Rectangle(cornerWidth, texture.TextureSize.Height - cornerHeight, texture.TextureSize.Width - (cornerWidth * 2), cornerHeight));
            BottomRight = atlas.DerivateTexture(new Rectangle(texture.TextureSize.Width - cornerWidth, texture.TextureSize.Height - cornerHeight, cornerWidth, cornerHeight));

            CornerWidth = cornerWidth;
            CornerHeight = cornerHeight;
        }

        ~ResizeableBoxTexture()
        {
            Dispose();
        }

        public void Draw(Rectangle r)
        {
            TopLeft.Draw(r.X, r.Y);
            Extend(Top, r.X + CornerWidth, r.Y, r.Width - (CornerWidth * 2), CornerHeight);
            TopRight.Draw((r.X + r.Width) - CornerWidth, r.Y);

            Extend(Left, r.X, r.Y + CornerHeight, CornerWidth, r.Height - (CornerHeight * 2));
            Extend(Center, r.X + CornerWidth, r.Y + CornerHeight, r.Width - (CornerWidth * 2), r.Height - (CornerHeight * 2));
            Extend(Right, (r.X + r.Width) - CornerWidth, r.Y + CornerHeight, CornerWidth, r.Height - (CornerHeight * 2));

            BottomLeft.Draw(r.X, (r.Y + r.Height) - CornerHeight);
            Extend(Bottom, r.X + CornerWidth, (r.Y + r.Height) - CornerHeight, r.Width - (CornerWidth * 2), CornerHeight);
            BottomRight.Draw((r.X + r.Width) - CornerWidth, (r.Y + r.Height) - CornerHeight);


        }

        private void Extend(Texture t, float x, float y,float width, float height)
        {    // テクスチャ全体を描画するためのソース矩形
            Rectangle source = new Rectangle(0, 0, t.TextureSize.Width, t.TextureSize.Height);
            // 描画先の矩形（拡大・縮小）
            Rectangle dest = new Rectangle(x, y, width, height);
            // 原点は左上、回転角は 0、Color.WHITE で描画
            Raylib.DrawTexturePro(t.texture, source, dest, new Vector2(0, 0), 0, Color.White);

            //            DX.DrawExtendGraph(x, y, x + width, y + height, t.ID, DX.TRUE);
        }

        public void Dispose()
        {
            TopLeft?.Dispose();
            Top?.Dispose();
            TopRight?.Dispose();
            Left?.Dispose();
            Center?.Dispose();
            Right?.Dispose();
            BottomLeft?.Dispose();
            Bottom?.Dispose();
            BottomRight?.Dispose();
        }

        private readonly Texture TopLeft;
        private readonly Texture Top;
        private readonly Texture TopRight;
        private readonly Texture Left;
        private readonly Texture Center;
        private readonly Texture Right;
        private readonly Texture BottomLeft;
        private readonly Texture Bottom;
        private readonly Texture BottomRight;
        private readonly int CornerWidth;
        private readonly int CornerHeight;
    }
}
