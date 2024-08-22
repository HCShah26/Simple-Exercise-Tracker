using Microsoft.Extensions.Logging;

namespace ExerciseTrackerHS;

public partial class LogPage : ContentPage
{
    int currentYear = 0;
    int _dailyAveExercise = 0;
    string loggedMinMsg = "Exercise minutes logged ";
    string startDate = "-01-01";
    string endDate = "-01-31";
    DateTime exerciseDate;

    private UserData _userPreferences;
    private ExerciseLogger _logger;
    public LogPage(UserData userPreferences, ExerciseLogger logger)
    {
        InitializeComponent();
        _userPreferences = userPreferences;
        _logger = logger;

        SetLogPageSettings();

        UpdateUI();

        DisplayStats();

    }

    private void SetLogPageSettings()
    {
        this.BackgroundColor = _userPreferences.background;
        slider.Maximum = _userPreferences.maxDailyExercise;

        currentYear = DateTime.Now.Year;
        startDate = currentYear.ToString() + startDate;
        endDate = (currentYear + 1).ToString() + endDate;
        DTPicker_ExerciseDate.MinimumDate = DateTime.ParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        DTPicker_ExerciseDate.MaximumDate = DateTime.ParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        DTPicker_ExerciseDate.Date = DateTime.Now;
        DTPicker_ExerciseDate.Format = "dd/MM/yyyy";
        exerciseDate = DTPicker_ExerciseDate.Date;

    }

    private void UpdateUI()
    {
        this.BackgroundColor = _userPreferences.background;
        UpdateColors(LogStack);
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
                        label.TextColor = _userPreferences.foreground;
                        break;
                    case Button button:
                        button.TextColor = _userPreferences.foreground;
                        break;
                }
            }
        }
    }


    private void DisplayStats()
    {
        _dailyAveExercise = _logger.AverageExercised();
        if (_dailyAveExercise > _userPreferences.maxDailyExercise)
        {
            DisplayAveExercised.TextColor = _userPreferences.foreground;
        }
        else
        {
            if (_userPreferences.foreground == Colors.Red)
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
            $"{_logger.CatchUpMins(_userPreferences.maxDailyExercise)}" +
        $" {Environment.NewLine} " +
            $"{_logger.HoursDone(_userPreferences.maxDailyExercise)}");
        //SemanticScreenReader.Announce(DisplayExerciseStats.Text);

    }
    private void OnDateChanged(object sender, DateChangedEventArgs e)
    {
        exerciseDate = e.NewDate;
    }
    private void OnSliderValueChanged(object sender, EventArgs e)
    {
        LogFitnessBtn.IsEnabled = true;
        lblSlider.Text = Convert.ToInt32(slider.Value).ToString();
    }

    private void OnLogFitnessClicked(object sender, EventArgs e)
    {
        if (exerciseDate.ToString().Length > 0 && slider.Value.ToString().Length > 0)
        {
            ExerciseLog _log = new ExerciseLog(exerciseDate, Convert.ToInt32(slider.Value));
            _logger.AddorUpdateLog(_log);
            _logger.LoadDataFromFile();
            DisplayStats();
            slider.Value = 0;
            LogFitnessBtn.IsEnabled= false;
        }
    }

    public void OnHomePageClicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new MainPage());
    }

    public void OnSettingsPageClick(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new SettingsPage(_userPreferences, _logger));
    }
}