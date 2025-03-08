using Raylib_cs;

namespace Kameoto
{
    /// <summary>
    /// マウス操作を管理するクラス。
    /// </summary>
    public static class Mouse
    {
        /// <summary>
        /// マウスの入力を処理する。必ず毎フレーム呼ぶ必要があります。
        /// </summary>
        public static void Update()
        {
            Wheel = Raylib.GetMouseWheelMove();

          //  DX.GetMousePoint(out var x, out var y);
            var pos=Raylib.GetMousePosition();
            IsMouseMoving = false;
            if (X != pos.X || Y != pos.Y || Wheel != 0)
            {
                IsMouseMoving = true;
            }
            X = (int)pos.X;
            Y = (int)pos.Y;

       //     var mouse = DX.GetMouseInput();

            var isAnyButtonPushed = false;

            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)i))
                {
                    if (Buttons[i] <= 0)
                    {
                        // 前回未押下状態から新たに押下
                        Buttons[i] = 1;
                    }
                    else
                    {
                        // 連続して押下中
                        Buttons[i] = 2;
                    }
                    isAnyButtonPushed = true;
                }
                else
                {
                    // 押されていない場合、前回押下されていたなら「離された」として -1、連続未押下なら 0
                    if (Buttons[i] >= 1)
                    {
                        Buttons[i] = -1;
                    }
                    else
                    {
                        Buttons[i] = 0;
                    }
                }
            }

            IsAnyButtonPushing = isAnyButtonPushed;
        }

        /// <summary>
        /// マウスが押されたかどうかチェックします。。
        /// </summary>
        /// <param name="mouseButton">ボタン。</param>
        /// <returns>押されたかどうか。</returns>
        public static bool IsPushed(MouseButton mouseButton)
        {
            return Buttons[GetIndexFromMouseButton(mouseButton)] == 1;
        }

        /// <summary>
        /// マウスが押されているかどうかチェックします。
        /// </summary>
        /// <param name="mouseButton">ボタン。</param>
        /// <returns>押されているかどうか。</returns>
        public static bool IsPushing(MouseButton mouseButton)
        {
            return Buttons[GetIndexFromMouseButton(mouseButton)] > 0;
        }

        /// <summary>
        /// マウスのボタンが離されたかどうかチェックします。
        /// </summary>
        /// <param name="mouseButton">ボタン。</param>
        /// <returns>離されたかどうか。</returns>
        public static bool IsLeft(MouseButton mouseButton)
        {
            return Buttons[GetIndexFromMouseButton(mouseButton)] == -1;
        }

        private static int GetIndexFromMouseButton(MouseButton mouseButton)
        {
            // Raylib_cs.MouseButton の underlying 値を利用
            return (int)mouseButton;
        }

        private static MouseButton GetMouseButtonFromIndex(int index)
        {
            return (MouseButton)index;
        }

        private static readonly int[] Buttons = new int[5];

        /// <summary>
        /// マウスホイール回転量。
        /// 奥に回すと正の数になる。
        /// </summary>
        public static float Wheel { get; private set; }

        /// <summary>
        /// マウスX座標。
        /// </summary>
        public static int X { get; private set; }
        /// <summary>
        /// マウスY座標。
        /// </summary>
        public static int Y { get; private set; }

        /// <summary>
        /// マウスボタンのどれかが押下されているかどうか。
        /// フレームの更新のたびに false になります。
        /// </summary>
        public static bool IsAnyButtonPushing { get; private set; }

        /// <summary>
        /// マウスが動いているかどうか。
        /// フレームの更新のたびに false になります。
        /// </summary>
        public static bool IsMouseMoving { get; private set; }
    }


}