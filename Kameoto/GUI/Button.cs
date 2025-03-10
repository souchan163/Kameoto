﻿using System;
using System.Drawing;
using Rectangle = Raylib_cs.Rectangle;

namespace Kameoto.GUI
{
    /// <summary>
    /// ボタン。
    /// </summary>
    public class Button : DrawPart
    {
        /// <summary>
        /// GUIのボタンを描画する。
        /// </summary>
        /// <param name="background">ボタンの背景として使用する画像。正方形でなければならない。</param>
        /// <param name="content">ボタンの中身。</param>
        /// <param name="width">(オプション)横のサイズ。</param>
        /// <param name="height">(オプション)縦のサイズ。</param>
        public Button(ITextureReturnable background, ITextureReturnable content, int? width = null, int? height = null)
            : base(width ?? content.GetTexture().TextureSize.Width, height ?? content.GetTexture().TextureSize.Height)
        {
            OnMouseDown += Button_OnMouseDown;
            OnMouseUp += Button_OnMouseUp;

            Background = background;
            Content = content;

            DownAnimation = new Animation.EaseOut(100, 95, 1000 * 250);
            UpAnimation = new Animation.EaseOut(95, 100, 1000 * 250);
        }

        public override void Update(bool canHandle, int? pointX = null, int? pointY = null, int parentAbsoluteX = 0, int parentAbsoluteY= 0)
        {
            DownAnimation?.Tick();
            UpAnimation?.Tick();

            base.Update(canHandle, pointX, pointY, parentAbsoluteX, parentAbsoluteY);

            var v = VirtualScreen.GetTexture();
            if (DownAnimation.Counter.State == TimerState.Started)
            {
                v.ScaleX = v.ScaleY = (float)(DownAnimation.GetAnimation() / 100f);
            }
            else if (UpAnimation.Counter.State == TimerState.Started)
            {
                v.ScaleX = v.ScaleY = (float)(UpAnimation.GetAnimation() / 100f);
            }
            else
            {
                if (LongClickCounter.State == TimerState.Started)
                {
                    v.ScaleX = v.ScaleY = 0.95f;
                }
                else
                {
                    v.ScaleX = v.ScaleY = 1.0f;
                }
            }
        }

        /// <summary>
        /// GUI部品を描画する。
        /// </summary>
        public override void Draw()
        {
            if (ShouldBuild)
            {
                Build();
            }

            VirtualScreen.ClearScreen();

            VirtualScreen.Draw(() =>
            {
                Texture.Draw(Width / 2.0, Height / 2.0);

                foreach (var item in Child)
                {
                    item.Draw();
                    item.Screen.GetTexture().Draw(item.X, item.Y);
                }
            });

            if (!Enabled)
            {
                VirtualScreen.GetTexture().Brightness(-128);
            }

            Screen.ClearScreen();

            Screen.Draw(() => VirtualScreen.GetTexture().Draw(Width / 2.0, Height / 2.0));
        }

        public override void Build()
        {
            SetTexture(Background, Content);
            base.Build();
        }

        /// <summary>
        /// ボタンの中身を変更する。
        /// </summary>
        /// <param name="content">ボタンの中身。</param>
        public void ChangeContent(ITextureReturnable content)
        {
            Content = content;
            // 再生成
            ShouldBuild = true;
        }

        /// <summary>
        /// ボタンの背景を変更する。
        /// </summary>
        /// <param name="background">ボタンの背景。</param>
        public void ChangeBackground(ITextureReturnable background)
        {
            Background = background;
            // 再生成
            ShouldBuild = true;
        }

        /// <summary>
        /// ボタンのサイズを変更する。
        /// </summary>
        /// <param name="width">横幅。</param>
        /// <param name="height">縦幅。</param>
        public void ChangeSize(int? width = null, int? height = null)
        {
            Width = width ?? Screen.ScreenSize.Width;
            Height = height ?? Screen.ScreenSize.Height;
            // 再生成
            ShouldBuild = true;
        }

        private void SetTexture(ITextureReturnable background, ITextureReturnable content)
        {
            var contentTex = content.GetTexture();
            var backTex = background.GetTexture();
            // ボタンの土台の描画。
            var buttonTextureSize = backTex.TextureSize;
            var oneSize = ((int)Math.Ceiling(1.0 * buttonTextureSize.Width / 3), (int)Math.Ceiling(1.0 * buttonTextureSize.Height / 3));

            var screen = new VirtualScreen(Width, Height);
            VirtualScreen = new VirtualScreen(Width, Height);

            screen.Draw(() =>
            {
                // 左上
                backTex.ReferencePoint = ReferencePoint.TopLeft;
                backTex.ScaleX = 1.0f;
                backTex.ScaleX = 1.0f;
                backTex.Draw(0, 0, new Rectangle(0, 0, oneSize.Item1, oneSize.Item2));

                // 中央上
                backTex.ReferencePoint = ReferencePoint.TopCenter;
                backTex.ScaleX = (float)(1.0 * (Width - (oneSize.Item1 * 2)) / oneSize.Item1);
                backTex.Draw(Width / 2.0, 0, new Rectangle(oneSize.Item1, 0, oneSize.Item1, oneSize.Item2));

                // 右上
                backTex.ReferencePoint = ReferencePoint.TopRight;
                backTex.ScaleX = 1.0f;
                backTex.Draw(Width, 0, new Rectangle(oneSize.Item1 * 2, 0, oneSize.Item1, oneSize.Item2));

                // 左中央
                backTex.ReferencePoint = ReferencePoint.CenterLeft;
                backTex.ScaleY = (float)(1.0 * (Height - (oneSize.Item2 * 2)) / oneSize.Item2);
                backTex.Draw(0, Height / 2.0, new Rectangle(0, oneSize.Item2, oneSize.Item1, oneSize.Item2));

                // 中央
                backTex.ReferencePoint = ReferencePoint.Center;
                backTex.ScaleX = (float)(1.0 * (Width - (oneSize.Item1 * 2)) / oneSize.Item1);
                backTex.Draw(Width / 2.0, Height / 2.0, new Rectangle(oneSize.Item1, oneSize.Item2, oneSize.Item1, oneSize.Item2));

                // 右中央
                backTex.ReferencePoint = ReferencePoint.CenterRight;
                backTex.ScaleX = 1.0f;
                backTex.Draw(Width, Height / 2.0, new Rectangle(oneSize.Item1 * 2, oneSize.Item2, oneSize.Item1, oneSize.Item2));

                // 左下
                backTex.ReferencePoint = ReferencePoint.BottomLeft;
                backTex.ScaleY = 1.0f;
                backTex.Draw(0, Height, new Rectangle(0, oneSize.Item2 * 2, oneSize.Item1, oneSize.Item2));

                // 中央下
                backTex.ReferencePoint = ReferencePoint.BottomCenter;
                backTex.ScaleX = (float)(1.0 * (Width - (oneSize.Item1 * 2)) / oneSize.Item1);
                backTex.Draw(Width / 2.0, Height, new Rectangle(oneSize.Item1, oneSize.Item2 * 2, oneSize.Item1, oneSize.Item2));

                // 右下
                backTex.ReferencePoint = ReferencePoint.BottomRight;
                backTex.ScaleX = 1.0f;
                backTex.Draw(Width, Height, new Rectangle(oneSize.Item1 * 2, oneSize.Item2 * 2, oneSize.Item1, oneSize.Item2));

                // 文字の描画
                contentTex.ReferencePoint = ReferencePoint.Center;
                contentTex.Draw(Width / 2.0, Height / 2.0);
            });

            Texture = screen.GetTexture();
            Texture.ReferencePoint = ReferencePoint.Center;
            VirtualScreen.GetTexture().ReferencePoint = ReferencePoint.Center;
        }

        private void Button_OnMouseDown(object sender, EventArgs e)
        {
            UpAnimation.Stop();
            UpAnimation.Reset();
            DownAnimation.Reset();
            DownAnimation.Start();
        }

        private void Button_OnMouseUp(object sender, EventArgs e)
        {
            DownAnimation.Stop();
            DownAnimation.Reset();
            UpAnimation.Reset();
            UpAnimation.Start();
        }

        private readonly Animation.EaseOut DownAnimation;
        private readonly Animation.EaseOut UpAnimation;

        private VirtualScreen VirtualScreen;
        private ITextureReturnable Background;
        private ITextureReturnable Content;
    }
}