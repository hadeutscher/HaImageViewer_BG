using HaImageViewer_BG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace HaImageViewer_BG
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;

        public MainWindow(string sourcePath, string prefix, List<string> tags, string superTag)
        {
            InitializeComponent();
            viewModel = new MainViewModel(sourcePath, prefix, tags, superTag);
            DataContext = viewModel;
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