using System;
using System.Runtime.InteropServices;
using UdpSimulator.Xamls;

namespace UdpSimulator.ViewModels
{
    /// <summary>
    /// ダイアログ(エラー用)表示処理.
    /// </summary>
    public class DialogService
    {
        /// <summary>
        /// ダイアログ表示.
        /// </summary>
        /// <param name="title">タイトル.</param>
        /// <param name="message">エラーメッセージ.</param>
        /// <returns>DialogResult(ErrorDialogからの戻り値は常時null).</returns>
        public static bool? Show(string title, string message)
        {
            var rect = new RECT();
            GetWindowRect(GetForegroundWindow(), ref rect);

            var dialog = new ErrorDialog()
            {
                Title = title,
            };

            dialog.ErrorMessage.Text = message;
            dialog.Left = rect.left + ((rect.right - rect.left) / 2) - dialog.Width / 2;
            dialog.Top = rect.top + ((rect.bottom - rect.top) / 2) - dialog.Height / 2;

            return dialog.ShowDialog();
        }

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    }
}
