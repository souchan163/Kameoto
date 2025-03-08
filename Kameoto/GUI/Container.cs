using Raylib_cs;

namespace Kameoto.GUI
{
    /// <summary>
    /// 決まったサイズを持つコンテナを生成する。
    /// </summary>
    public class Container : DrawPart
    {
        /// <summary>
        /// 決まったサイズを持つコンテナを生成する。
        /// </summary>
        public Container(int width, int height, Color backgroundColor)
            : base(width, height)
        {
            BackgroundColor = backgroundColor;
            VirtualScreen = new VirtualScreen(Width, Height);
        }

        public override void Draw()
        {
            VirtualScreen.ClearScreen();
            VirtualScreen.Draw(() =>
            {
                Raylib_cs.Color bgColor = new Raylib_cs.Color(
                                   BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
                // コンテナ全体を背景色で塗りつぶす
                Raylib.DrawRectangle(0, 0, Width, Height, bgColor);
            });

            Texture = VirtualScreen.GetTexture();

            base.Draw();
        }

        private Color BackgroundColor;
        private VirtualScreen VirtualScreen;
    }
}
