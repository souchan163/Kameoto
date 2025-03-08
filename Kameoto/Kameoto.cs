using Raylib_cs;
using System;
using System.Runtime.CompilerServices;

namespace Kameoto
{
    /// <summary>
    /// Amaoto クラス。
    /// </summary>
    public static class Kameoto
    {
        /// <summary>
        /// Amaoto と DXライブラリの初期化をする。必ず Amaoto の使用前に呼び出す必要がある。
        /// </summary>
        /// <param name="beforeInit">DxLib_Initの前に設定するメソッド。</param>
        /// <param name="afterInit">DxLib_Initの後に設定するメソッド。</param>
        public static void Init(Action beforeInit, Action afterInit)
        {
            beforeInit?.Invoke();
            //サイズは後から変えちゃって
            Raylib.InitWindow(windowWidth, windowHeight, "window");


         //   DX.SetUsePremulAlphaConvertLoad(DX.TRUE);
           // DX.CreateMaskScreen();

            afterInit?.Invoke();
        }

        /// <summary>
        /// Amaoto と DXライブラリの終了処理をする。
        /// </summary>
        public static void End()
        {
           // if (DX.DxLib_End() == -1)
            {
              //  throw new Exception("DXLib ending failed.");
            }
            Raylib.CloseWindow();
        }

        /// <summary>
        /// ループ直後に呼び出すメソッド。
        /// </summary>
        public static void Loop()
        {
            SetWindowOffsetAndScale();
            Key.Update();
            Mouse.Update();

            MouseHandled = false;
        }

        /// <summary>
        /// GUI で使われる長押し時間を変更する。
        /// </summary>
        /// <param name="ms">ミリ秒。</param>
        public static void SetLongClickNs(int ms)
        {
            LongClickMs = ms;
        }


        /// <summary>
        /// FontRenderのデバッグを行うかどうか設定する。
        /// </summary>
        /// <param name="debug">デバッグを行う。</param>
        public static void SetFontRenderDebug(bool debug)
        {
            FontRenderDebug = debug;
        }

        /// <summary>
        /// 現在のフレームでマウス操作したと言うことにする。
        /// </summary>
        public static void HandleMouse()
        {
            MouseHandled = true;
        }

        /// <summary>
        /// 長押し時間。
        /// </summary>
        public static int LongClickMs { get; private set; } = 400;

        /// <summary>
        /// FontRenderのデバッグを行うかどうか。
        /// </summary>
        public static bool FontRenderDebug { get; private set; } = false;

        /// <summary>
        /// 現在のフレームでマウス操作が行われたかどうか。
        /// </summary>
        public static bool MouseHandled { get; private set; } = false;


        public static int windowWidth = 1920;
        public static int windowHeight = 1080;

        private static void SetWindowOffsetAndScale()
        {
            // 現在のウィンドウサイズを取得
            int windowWidth = Raylib.GetScreenWidth();
            int windowHeight = Raylib.GetScreenHeight();

            // 16:9 アスペクト比のビューポートを計算
            float targetAspect = 16f / 9f;
            int viewportWidth, viewportHeight;

            if (windowWidth / (float)windowHeight > targetAspect)
            {
                // ウィンドウが横長の場合
                viewportHeight = windowHeight;
                viewportWidth = (int)(viewportHeight * targetAspect);
                nWindowOffsetX = (windowWidth - viewportWidth) / 2;
            }
            else
            {
                // ウィンドウが縦長の場合
                viewportWidth = windowWidth;
                viewportHeight = (int)(viewportWidth / targetAspect);
                nWindowOffsetY = (windowHeight - viewportHeight) / 2;
            }

            // 基準解像度 1920x1080 に対するスケール倍率
            fWindowScaleX = viewportWidth / 1920f;
            fWindowScaleY = viewportHeight / 1080f;

            // 描画領域をビューポート内に制限
            Raylib.BeginScissorMode((int)nWindowOffsetX, (int)nWindowOffsetX, viewportWidth, viewportHeight);
        }
        /// <summary>
        /// raylibに画面自動縮小とかでのサイズ調整はないのでそれを気合でやるようのもの
        /// </summary>
        public static int nWindowOffsetX, nWindowOffsetY;

        /// <summary>
        /// raylibに画面自動縮小とかでのサイズ調整はないのでそれを気合でやるようのもの
        /// </summary>
        public static float fWindowScaleX, fWindowScaleY;
    }
}
