using System.Diagnostics;


namespace ExerciseTrackerHS
{
    public partial class MainPage : ContentPage
    {
        public UserData userPreferences;
        public ExerciseLogger logger;

        int _dailyAveExercise = 0;


        // This override method of onAppearing is called to fix the 
        // issue with SemanticScreenReader.Announce failing to execute
        // because the Page is not loaded when the Annouce method is called.

        // To fix this we are calling the Dispatcher by using async and wait ad adding a short delay, 
        // this will ensure that the page is loaded before calling the Annouce method
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.Dispatch(async () => 
            {
                await Task.Delay(500);
                SemanticScreenReader.Announce(DisplayAveExercised.Text);
                SemanticScreenReader.Announce(DisplayCatchupMins.Text);
                SemanticScreenReader.Announce(DisplayExerciseStats.Text);
            });
        }


        public MainPage()
        {
            InitializeComponent();

            InitializeUserPreferences();
            
            logger = new ExerciseLogger();

            UIHelper.UpdateUI(MainStack, userPreferences.foreground, userPreferences.background);

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
        }


        private void OnSettingsPageClick(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SettingsPage(userPreferences, logger));
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

            DisplayCatchupMins.Text = $"Exercise plan catchup minutes: {logger.CatchUpMins(userPreferences.maxDailyExercise)}";
            //SemanticScreenReader.Announce(DisplayCatchupMins.Text);

            DisplayExerciseStats.Text = $"{logger.HoursDone(userPreferences.maxDailyExercise)}";
            //SemanticScreenReader.Announce(DisplayExerciseStats.Text);  
        }
    }
}
