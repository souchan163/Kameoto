using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Raylib_cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rectangle = Raylib_cs.Rectangle;
using Color = Raylib_cs.Color;
using System.Numerics;

namespace Kameoto
{
    /// <summary>
    /// テクスチャ。
    /// </summary>
    public class Texture : IDisposable, ITextureReturnable
    {
        /// <summary>
        /// テクスチャを生成します。
        /// </summary>
        public Texture()
        {

            fRotation = 0;
            ScaleX = 1.0f;
            ScaleY = 1.0f;
            Opacity = 1.0;
        }

        /// <summary>
        /// 画像ファイルからテクスチャを生成します。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        public Texture(string fileName)
            : this()
        {
            texture = Raylib.LoadTexture(fileName);
           // if(texture.Id)
            {
                IsEnable = true;
            }
            FileName = fileName;
            rec = new Rectangle(0, 0, texture.Width, texture.Height);
            RotaOrigin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);

        }



        /// <summary>
        /// ビットマップからテクスチャを生成します。
        /// ない
        /// </summary>
        /// <param name="bitmap">ビットマップ。</param>
        public Texture(int naiyon)
            : this()
        {

        }
        /// <summary>
        /// RaylibのTexture2Dから生成
        /// </summary>
        /// <param name="newTx"></param>
        public Texture(Texture2D newTx):this()
        {
            this.texture = newTx;

        }


        public void Dispose()
        {
            if(IsEnable)
            {
                Raylib.UnloadTexture(texture);

                IsEnable = false;
            }
        }

        public void ChangeColor()
        {

        }
        public void Draw(double _x, double _y)
        {
            Draw(_x, _y, rec);
        }
        /// <summary>
        /// 描画します。
        /// </summary>

        public void Draw(double _x, double _y, Rectangle sourceRect)
        {
            // 座標を丸める
            float x = (float)Math.Round(_x);
            float y = (float)Math.Round(_y);





            // ソース矩形のサイズに、ユーザー定義のスケール(ScaleX, ScaleY)とウィンドウスケールを適用
            float scaledWidth = sourceRect.Width * ScaleX * Kameoto.fWindowScaleX;
            float scaledHeight = sourceRect.Height * ScaleY * Kameoto.fWindowScaleY;

            // 描画先の矩形を計算（オフセットとスケールを反映）
            // ※ RotaOrigin は元画像内で回転の基準点（ピボット）として使用される
            Rectangle destRect = new Rectangle(
                Kameoto.nWindowOffsetX + (x + RotaOrigin.X) * Kameoto.fWindowScaleX,
                Kameoto.nWindowOffsetY + (y + RotaOrigin.Y) * Kameoto.fWindowScaleY,
                scaledWidth,
                scaledHeight
            );
            if (this.bFlipVertical) sourceRect.Height *= -1;
            if (this.bFlipHorizontal) sourceRect.Width *= -1;

            // BlendModeの設定（必要に応じて）
            Raylib.BeginBlendMode(this.BlendMode);

            if (Opacity > 255) Opacity = 255;
            // 色にアルファ値（Opacity）を反映
            Color tmpColor = new Color(color.R, color.G, color.B, (byte)Opacity);

            // DrawTextureProで、ソース矩形の内容を destRect に描画
            // RotaOrigin を回転の基準点として使用し、fRotation はラジアン値を度に変換して渡す
            Raylib.DrawTexturePro(
                texture,                  // 元のテクスチャ
                sourceRect,          // ソース矩形
                destRect,            // 描画先の矩形
                new Vector2(RotaOrigin.X * Kameoto.fWindowScaleX, RotaOrigin.Y * Kameoto.fWindowScaleY),          // 回転の基準点
                (float)(fRotation * 180.0 / MathF.PI), // ラジアン→度変換
                tmpColor             // 描画色（アルファ値付き）
            );

            Raylib.EndBlendMode();
        }

        /// <summary>
        /// テクスチャをPNGファイルに出力します。
        /// </summary>
        /// <param name="path">保存先。</param>
        public void SaveAsPNG(string path)
        {
            throw new Exception("未実装");
        }

        /// <summary>
        /// テクスチャを取得する。
        /// </summary>
        /// <returns>テクスチャ。</returns>
        public Texture GetTexture()
        {
            return this;
        }

        public bool bFlipVertical { get; set; }
        public bool bFlipHorizontal { get; set; }

        /// <summary>
        /// DXLibだとGraphFilterでテクスチャの色変えてるっぽいのでその値を保存してraylib描画時に使う用
        /// </summary>
        public Color color { get; set; }
        
        /// <summary>
        /// 有効かどうか。
        /// </summary>
        public bool IsEnable { get; private set; }

        /// <summary>
        /// 合成モード。
        /// </summary>
        public BlendMode BlendMode { get; set; }

        /// <summary>
        /// ファイル名。
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 不透明度。
        /// </summary>
        public double Opacity { get; set; }


        public Texture2D texture { get; set; }
        public Vector2 RotaOrigin = new Vector2(0, 0);

        /// <summary>
        /// 角度(弧度法)。
        /// </summary>
        public float fRotation { get; set; }

        /// <summary>
        /// 描画基準点。
        /// </summary>
        public ReferencePoint ReferencePoint { get; set; }

        /// <summary>
        /// 拡大率X。
        /// </summary>
        public float ScaleX { get; set; }

        /// <summary>
        /// 拡大率Y。
        /// </summary>
        public float ScaleY { get; set; }

        /// <summary>
        /// テクスチャのサイズを返します。
        /// </summary>
        public Size TextureSize
        {
            get
            {
                return new Size(this.texture.Width, this.texture.Height);
            }
        }

        /// <summary>
        /// 拡大率を考慮した、描画されるときのサイズ。
        /// </summary>
        public Size ActualSize
        {
            get
            {
                var s = TextureSize;
                return new Size((int)(ScaleX * s.Width), (int)(ScaleY * s.Height));
            }
        }

        public Rectangle rec;
    }



    /// <summary>
    /// 描画基準点。
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReferencePoint
    {
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft,

        /// <summary>
        /// 中央上
        /// </summary>
        TopCenter,

        /// <summary>
        /// 右上
        /// </summary>
        TopRight,

        /// <summary>
        /// 左中央
        /// </summary>
        CenterLeft,

        /// <summary>
        /// 中央
        /// </summary>
        Center,

        /// <summary>
        /// 右中央
        /// </summary>
        CenterRight,

        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 中央下
        /// </summary>
        BottomCenter,

        /// <summary>
        /// 右下
        /// </summary>
        BottomRight
    }
}