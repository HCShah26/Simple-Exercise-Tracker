using System.Diagnostics;


namespace ExerciseTrackerHS
{
    public partial class MainPage : ContentPage
    {
        public UserData userPreferences;
        public ExerciseLogger logger;

        int _dailyAveExercise = 0;
        

        public MainPage()
        {
            InitializeComponent();

            InitializeUserPreferences();
            
            logger = new ExerciseLogger();
            //logger.LoadDataFromFile();
            
            UpdateUI();

            DisplayStats();

        }

        private void InitializeUserPreferences()
        {
            userPreferences = new UserData(Colors.Black, Colors.White, 30);
            userPreferences.LoadPreferences(
                userPreferences.foreground.ToArgbHex().ToString(),
                userPreferences.background.ToArgbHex().ToString(),
                userPreferences.maxDailyExercise.ToString()
                );
        }

        private void OnLogPageClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LogPage(userPreferences, logger));
            //UpdateStats();
        }

        private void OnSettingsPageClick(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SettingsPage(userPreferences, logger));
        }

        private void UpdateUI()
        {
            this.BackgroundColor = userPreferences.background;
            UpdateColors(MainStack);
        }

        private void UpdateColors(Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is Layout childLayout)
                {
                    UpdateColors(childLayout);
                }    
                else
                {
                    switch (child)
                    {
                        case Label label:
                            label.TextColor = userPreferences.foreground;
                            break;
                        case Button button:
                            button.TextColor = userPreferences.foreground;
                            break;
                    }
                }
            }
        }

        private void DisplayStats()
        {
            _dailyAveExercise = logger.AverageExercised();
            if (_dailyAveExercise > userPreferences.maxDailyExercise)
            {
                DisplayAveExercised.TextColor = userPreferences.foreground;
            }
            else
            {
                if (userPreferences.foreground == Colors.Red)
                {
                    DisplayAveExercised.TextColor = Colors.OrangeRed;
                }
                else
                {
                    DisplayAveExercised.TextColor = Colors.Red;
                }
            }
            DisplayAveExercised.Text = $"Daily average execise minutes: {_dailyAveExercise}";
            //SemanticScreenReader.Announce(DisplayAveExercised.Text);


            DisplayExerciseStats.Text = ($"Exercise plan catchup minutes: " +
                $"{logger.CatchUpMins(userPreferences.maxDailyExercise)}" +
                $" {Environment.NewLine} " +
                $"{logger.HoursDone(userPreferences.maxDailyExercise)}");
            //SemanticScreenReader.Announce(DisplayExerciseStats.Text);

        }

    }

}
