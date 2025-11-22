using HaImageViewer_BG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace HaImageViewer_BG
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        [DllImport("uxtheme.dll", SetLastError = true, EntryPoint = "#132")]
        private static extern int ShouldAppsUseDarkMode();

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static bool IsDarkMode { get { return ShouldAppsUseDarkMode() == 1; } }

        public MainWindow(string sourcePath, string prefix, List<string> tags, string superTag)
        {
            InitializeComponent();
            SetTheme();
            viewModel = new MainViewModel(sourcePath, prefix, tags, superTag);
            DataContext = viewModel;
        }

        private void SetTheme()
        {
            var windowHandle = new WindowInteropHelper(this).EnsureHandle();

            int useDarkMode = IsDarkMode ? 1 : 0;
            DwmSetWindowAttribute(windowHandle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Ctrl+Left/Right jumps 10
                if (e.Key == Key.Right)
                    viewModel.NextImage(10);
                else if (e.Key == Key.Left)
                    viewModel.PreviousImage(10);
            }
            else
            {
                // Normal Left/Right jumps 1
                if (e.Key == Key.Right)
                    viewModel.NextImage(1);
                else if (e.Key == Key.Left)
                    viewModel.PreviousImage(1);
            }
        }

        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string currentFile = viewModel.GetCurrentFilePath();
            if (!string.IsNullOrEmpty(currentFile))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(currentFile) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}");
                }
            }
        }
    }
}