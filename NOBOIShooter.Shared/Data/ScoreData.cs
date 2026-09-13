using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NOBOIShooter.Data
{
    [Serializable]
    public class Score
    {
        public int ScoreGet { get; set; }
        public DateTime ScoreDate { get;  set; }

        public Score(int score, DateTime time)
        {
            ScoreGet = score;
            ScoreDate = time;
        }
    
    }
    class ScoreData
    {
        private const string SAVE_FILE_NAME = "sav.dat";
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
                using FileStream fileStream = new FileStream(SAVE_FILE_NAME, FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, ScoresTables);
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
                using FileStream fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                ScoresTables = binaryFormatter.Deserialize(fileStream) as List<Score>;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while loading the game: " + ex.Message);
            }
        }
    }
}
