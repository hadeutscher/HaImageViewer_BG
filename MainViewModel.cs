using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace HaImageViewer_BG
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<string> chosenFiles;
        private int currentIndex;

        private BitmapImage currentImageSource;
        public BitmapImage CurrentImageSource
        {
            get => currentImageSource;
            private set
            {
                currentImageSource = value;
                OnPropertyChanged(nameof(CurrentImageSource));
            }
        }

        public MainViewModel(string sourcePath, string prefix, List<string> selectedTags, string superTag)
        {
            chosenFiles = AnalyzeDirectory(sourcePath, prefix, selectedTags, superTag).Values.ToList();
            currentIndex = 0;
            UpdateImage();
        }

        public void NextImage(int step = 1)
        {
            if (currentIndex + step < chosenFiles.Count)
            {
                currentIndex += step;
            }
            else
            {
                currentIndex = chosenFiles.Count - 1;
            }
            UpdateImage();
        }

        public void PreviousImage(int step = 1)
        {
            if (currentIndex - step >= 0)
            {
                currentIndex -= step;
            }
            else
            {
                currentIndex = 0;
            }
            UpdateImage();
        }

        public string GetCurrentFilePath()
        {
            if (chosenFiles.Count == 0) return null;
            return chosenFiles[currentIndex];
        }

        private void UpdateImage()
        {
            if (chosenFiles.Count == 0) return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(chosenFiles[currentIndex]);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            CurrentImageSource = bitmap;
        }

        private Dictionary<string, string> AnalyzeDirectory(string path, string prefix, List<string> selectedTags, string superTag)
        {
            var filesInfo = new List<(string SeqId, List<string> Tags, string Filename)>();

            foreach (var filename in Directory.GetFiles(path, "*.png"))
            {
                var name = Path.GetFileName(filename);
                if (!name.StartsWith(prefix + "_"))
                    continue;

                string remainder = name.Substring(prefix.Length + 1, name.Length - prefix.Length - 5);
                var parts = remainder.Split('_');
                if (parts.Length < 1) continue;

                string seqId = parts[0];
                var tags = parts.Skip(1).ToList();
                filesInfo.Add((seqId, tags, filename));
            }

            var chosenFiles = new Dictionary<string, string>();
            foreach (var seqId in filesInfo.Select(f => f.SeqId).Distinct().OrderBy(s => s, new NaturalStringComparer()))
            {
                var candidates = filesInfo.Where(f => f.SeqId == seqId);

                int Score((string SeqId, List<string> Tags, string Filename) f)
                {
                    int score = 0;
                    score += f.Tags.Intersect(selectedTags).Count() * 10;
                    score -= f.Tags.Except(selectedTags).Count() * 5;
                    if (!string.IsNullOrEmpty(superTag) && f.Tags.Contains(superTag))
                        score += 50;
                    return score;
                }

                var bestFile = candidates.OrderByDescending(Score).First();
                chosenFiles[seqId] = bestFile.Filename;
            }

            return chosenFiles;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}