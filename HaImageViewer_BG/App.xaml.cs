using HaImageViewer_BG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HaImageViewer_BG
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var args = Environment.GetCommandLineArgs().Skip(1).ToList();
            if (args.Count < 2)
            {
                MessageBox.Show("Usage: HaImageViewer_BG <sourcePath> <prefix> [tags...] [--super SuperTag]");
                Shutdown();
                return;
            }

            string sourcePath = args[0];
            string prefix = args[1];

            var tags = new List<string>();
            string superTag = null;

            for (int i = 2; i < args.Count; i++)
            {
                if (args[i] == "--super" && i + 1 < args.Count)
                {
                    superTag = args[i + 1];
                    i++;
                }
                else
                {
                    tags.Add(args[i]);
                }
            }

            var mainWindow = new MainWindow(sourcePath, prefix, tags, superTag);
            mainWindow.Show();
        }
    }
}