using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace NOBOIShooter.Data
{
    [Serializable]
    public class Score
    {
        public int ScoreGet { get; set; }
        public DateTime ScoreDate { get; set; }

        // Parameterless ctor for System.Text.Json deserialization.
        public Score() { }

        public Score(int score, DateTime time)
        {
            ScoreGet = score;
            ScoreDate = time;
        }

    }
    class ScoreData
    {
        private const string SAVE_FILE_NAME = "noboi_shooter_scores.json";

        private static string GetSavePath()
        {
            // BinaryFormatter + relative "sav.dat" fails on Android/iOS (read-only
            // working dir) and is blocked on .NET 8/9. Use a per-app folder + JSON.
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                if (string.IsNullOrEmpty(folder))
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (string.IsNullOrEmpty(folder))
                    folder = AppDomain.CurrentDomain.BaseDirectory;
                Directory.CreateDirectory(folder);
                return Path.Combine(folder, SAVE_FILE_NAME);
            }
            catch
            {
                return SAVE_FILE_NAME;
            }
        }

        public List<Score> ScoresTables { get; private set; }

        public ScoreData ()
        {
            if (ScoresTables == null)
                ScoresTables = new List<Score>();
            LoadSave();
        }
        public void Add(Score score)
        {
            ScoresTables.Add(score);
        }

        public void Sort()
        {
            ScoresTables.Sort(delegate(Score x, Score y) {
                return y.ScoreGet.CompareTo(x.ScoreGet);
            });
        }

        public void SaveGame()
        {

            try
            {
                string json = JsonSerializer.Serialize(ScoresTables);
                File.WriteAllText(GetSavePath(), json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while saving the game: " + ex.Message);
            }

        }

        public void LoadSave()
        {
            try
            {
                string path = GetSavePath();
                if (!File.Exists(path))
                {
                    ScoresTables = new List<Score>();
                    return;
                }
                string json = File.ReadAllText(path);
                ScoresTables = JsonSerializer.Deserialize<List<Score>>(json) ?? new List<Score>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while loading the game: " + ex.Message);
                if (ScoresTables == null)
                    ScoresTables = new List<Score>();
            }
        }
    }
}
