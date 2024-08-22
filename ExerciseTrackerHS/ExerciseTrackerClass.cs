using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Diagnostics;


namespace ExerciseTrackerHS
{

    public class ColorPair
    {
        private string _colorName;
        private Color _selectedColor;

        public ColorPair(string colorName, Color color)
        {
            _colorName = colorName;
            _selectedColor = color;
        }

        public string ColorName
        {
            get
            {
                return _colorName;
            }
        }

        public Color SelectedColor 
        {
            get
            {
                return _selectedColor;
            }
        }

    }
   public class ExerciseLog
    {
        private DateTime _dateLogged;
        private int _minsExercised;

        public ExerciseLog(DateTime dateLogged, int minsExercised)
        {
            _dateLogged = dateLogged;
            _minsExercised = minsExercised;
        }

        public DateTime DateLogged
        {
            get
            {
                return _dateLogged;
            }
        }

        public int MinsExercised
        {
            get
            {
                return _minsExercised;
            }
        }
    }

    public class ExerciseLogger
    {
        private Dictionary<int, ExerciseLog> _exerciseLogs;
        private const string _fileName = "Fitness.txt";
        
        public ExerciseLogger()
        {
            _exerciseLogs = new Dictionary<int, ExerciseLog>();
            LoadDataFromFile();
        }

        public void AddorUpdateLog(ExerciseLog exerciseLog)
        {
            bool _addOrUpdateSuccess = false;
            if (_exerciseLogs.ContainsKey(exerciseLog.DateLogged.DayOfYear))
            {
                _exerciseLogs[exerciseLog.DateLogged.DayOfYear] = exerciseLog;
                _addOrUpdateSuccess = true;
            }
            else
            {
                _exerciseLogs.Add(exerciseLog.DateLogged.DayOfYear, exerciseLog);
                _addOrUpdateSuccess = true;
            }

            if (_addOrUpdateSuccess)
            {
                SaveDataToFile(_exerciseLogs);
            }
        }

        public ExerciseLog GetExerciseLogged(int dayIndex)
        {
            if (_exerciseLogs.ContainsKey(dayIndex))
            {
                return _exerciseLogs[dayIndex];
            }
            else
            {
                return null;
            }
        }

        public int Count()
        {
            return _exerciseLogs.Count;
        }

        public int CatchUpMins(int curAveMin)
        {
            int MaxDay = DateTime.Now.DayOfYear; //_exerciseLogs.Keys.Max();
            int MinsToDate = 365 * curAveMin;
            int TotExerMins = 0;
            int MinsToCatchUp = 0;
            int ActualDailyCatchUp = 0;
            foreach (var log in _exerciseLogs.Values)
            {
                TotExerMins += log.MinsExercised;
            }
            MinsToCatchUp = MinsToDate - TotExerMins;
            ActualDailyCatchUp = MinsToCatchUp / (365 - MaxDay);
            return ActualDailyCatchUp;
        }

        public string HoursDone(int curAveMin)
        {
            int MaxDays = DateTime.Now.DayOfYear; //_exerciseLogs.Keys.Max();
            int TotExerMins = 0;
            int ActualHours = 0;
            int ActualMins = 0;
            int ExpectedHours = 0;
            int ExpectedMins = 0;

            ExpectedHours = (MaxDays * curAveMin) / 60;
            ExpectedMins = (MaxDays * curAveMin) % 60;

            foreach (var log in _exerciseLogs.Values)
            {
                TotExerMins += log.MinsExercised;
            }
            if (TotExerMins > 0)
            {
                ActualHours = TotExerMins / 60;
                ActualMins = TotExerMins % 60;
            }
            return $"Total exercise = {ActualHours.ToString("D3")}:{ActualMins.ToString("D2")} hours out of {ExpectedHours.ToString("D3")}:{ExpectedMins.ToString("D2")}";
        }
        public int AverageExercised()
        {
            //Set variables to calculate the averageminutes
            int SumMinutesExercised = 0;
            int aveExerMins = 0;
            if (_exerciseLogs.Count > 0)
            {
                SumMinutesExercised = _exerciseLogs.Values.Sum(item => item.MinsExercised);

                //To calculate the average minutes exercised, we need to divide by the today.
                //We can't use the built in function of Linq method as the
                //data stored might not have exercise logged for each day

                if (SumMinutesExercised > 0)
                {
                    aveExerMins = SumMinutesExercised / DateTime.Now.DayOfYear;
                }
            }
            return aveExerMins;
        }

        public void LoadDataFromFile()
        {
            var _localPath = FileSystem.Current.AppDataDirectory;
            var _filePath = Path.Combine(_localPath, _fileName);

            if (File.Exists(Path.Combine(_localPath, _fileName)))
            {
                string jsonData = File.ReadAllText(_filePath);
                _exerciseLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, ExerciseLog>>(jsonData);
                //Debug.WriteLine($"Display count of Exercise Logs loaded from file: {_exerciseLogs.Count.ToString()}");
            }
            else
            {
                _exerciseLogs = new Dictionary<int, ExerciseLog>();
            }
        }

        public void ResetDataFromFile() 
        {
            var _localPath = FileSystem.Current.AppDataDirectory;
            var _filePath = Path.Combine(_localPath, _fileName);

            if (File.Exists(_filePath)) 
            { 
                File.Delete(_filePath); 
            }
        }

        public void SaveDataToFile(Dictionary<int, ExerciseLog> exerciseLogs)
        {
            var _localPath = FileSystem.Current.AppDataDirectory;
            var _filePath = Path.Combine(_localPath, _fileName);
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(exerciseLogs, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, jsonData);
            Debug.WriteLine($"Path to find data file: {_filePath}");
        }


    }

    public class UserData
    {
        public Color foreground = Colors.White;
        public Color background = Colors.Black;
        public int maxDailyExercise = 30;

        public UserData(Color foregroundColor, Color backgroundColor, int dailyExerciseMins) 
        {
            foreground = foregroundColor;
            background = backgroundColor;
            maxDailyExercise = dailyExerciseMins;
        }

        public void LoadPreferences(string default_foregroundColor, string default_backgroundColor, string default_dailyExerciseMins)
        {
            foreground = Color.Parse(Preferences.Get("ForegroundColor", default_foregroundColor));
            background = Color.Parse(Preferences.Get("BackgroundColor", default_backgroundColor));
            maxDailyExercise = Convert.ToInt32(Preferences.Get("MaxDailyExercise", default_dailyExerciseMins));
        }

        public void SavePreferences()
        {
            Preferences.Set("ForegroundColor", foreground.ToHex().ToString());
            Preferences.Set("BackgroundColor", background.ToHex().ToString());
            Preferences.Set("MaxDailyExercise", maxDailyExercise.ToString());
        }


    }
}
