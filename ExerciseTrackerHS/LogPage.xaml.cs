using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

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
        SemanticScreenReader.Announce(DisplayAveExercised.Text);


        DisplayExerciseStats.Text = ($"Exercise plan catchup minutes: " +
            $"{_logger.CatchUpMins(_userPreferences.maxDailyExercise)}" +
        $" {Environment.NewLine} " +
            $"{_logger.HoursDone(_userPreferences.maxDailyExercise)}");
        SemanticScreenReader.Announce(DisplayExerciseStats.Text);

    }
    private async void OnDateChanged(object sender, DateChangedEventArgs e)
    {
        int dayOfYear = 0;
        int startYear = DTPicker_ExerciseDate.MinimumDate.Year;
        int currentYear = e.NewDate.Year;

        //Check if year has changed -> Reset
        if (currentYear == startYear) 
        { 
            ExerciseLog thisRecord = null;

            exerciseDate = e.NewDate;
            dayOfYear = exerciseDate.DayOfYear;
            thisRecord = _logger.GetExerciseLogged(dayOfYear);

            if (thisRecord != null ) 
            {
                lblAlreadyLogged.Text = $"Exercise already logged for {exerciseDate.ToString("d")}";
                lblSlider.Text = thisRecord.MinsExercised.ToString();
                if (slider.Maximum >= thisRecord.MinsExercised)
                {
                    slider.Value = thisRecord.MinsExercised;
                }
                LogFitnessBtn.Text = "Modify Logged Minutes";
                LogFitnessBtn.IsEnabled = false;
            }
            else
            {
                lblSlider.Text = "";
                slider.Value = 0;
                LogFitnessBtn.Text = "Log Exercise Minutes";
                lblAlreadyLogged.Text = "";
            }
        }
        else
        {
            //Reset data?
            bool resetResponse = await DisplayAlert("You have started a new year!", "Would you like to reset the data?", "Yes", "No");
            if (resetResponse)
            {
                _logger.ResetDataFromFile();
                DTPicker_ExerciseDate.MinimumDate = new DateTime(e.NewDate.Year, 01, 01);
                DTPicker_ExerciseDate.MaximumDate = new DateTime(e.NewDate.Year + 1,1, 31);
            }
            
        }
    }
    private void OnSliderValueChanged(object sender, EventArgs e)
    {
        if (slider.Value > 0)
        {
            LogFitnessBtn.IsEnabled = true;
        }
        else
        {
            LogFitnessBtn.IsEnabled = false;
        }
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

    private void ResetLogBtn_Clicked(object sender, EventArgs e)
    {

    }
}