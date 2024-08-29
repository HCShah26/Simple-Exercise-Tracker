using Microsoft.Extensions.Logging;

namespace ExerciseTrackerHS;

public partial class LogPage : ContentPage
{
    int currentYear = 0;
    int _dailyAveExercise = 0;
    string loggedMinMsg = "Exercise minutes logged ";
    string startMMDD = "-01-01";
    string startDate = "";
    string endMMDD = "-01-31";
    string endDate =  ""; 
    DateTime exerciseDate;

    private UserData _userPreferences;
    private ExerciseLogger _logger;


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
            SemanticScreenReader.Announce(DisplayExerciseStats.Text);
        });
    }


    public LogPage(UserData userPreferences, ExerciseLogger logger)
    {
        InitializeComponent();
        _userPreferences = userPreferences;
        _logger = logger;

        SetLogPageSettings();

        UIHelper.UpdateUI(LogStack, _userPreferences.foreground, _userPreferences.background);

        DisplayStats();
    }


    private void SetLogPageSettings()
    {
        this.BackgroundColor = _userPreferences.background;
        slider.Maximum = _userPreferences.maxDailyExercise;
        lblSlider.Text = Convert.ToInt32(slider.Value).ToString();
        SetDatePicker(DateTime.Now);
        exerciseDate = DTPicker_ExerciseDate.Date;
        SetSliderIfDataLogged(exerciseDate);
    }


    private void SetDatePicker(DateTime selectedDate)
    {
        currentYear = selectedDate.Year;
        startDate = currentYear.ToString() + startMMDD;
        endDate = (currentYear + 1).ToString() + endMMDD;
        DTPicker_ExerciseDate.MinimumDate = DateTime.ParseExact(startDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        DTPicker_ExerciseDate.MaximumDate = DateTime.ParseExact(endDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        DTPicker_ExerciseDate.Date = selectedDate;
        DTPicker_ExerciseDate.Format = "dd/MM/yyyy";

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

        DisplayCatchupMins.Text = $"Exercise plan catchup minutes: {_logger.CatchUpMins(_userPreferences.maxDailyExercise)}";
        //SemanticScreenReader.Announce(DisplayCatchupMins.Text);

        DisplayExerciseStats.Text = $"{_logger.HoursDone(_userPreferences.maxDailyExercise)}";
        //SemanticScreenReader.Announce(DisplayExerciseStats.Text);  
    }


    private void OnDateChanged(object sender, DateChangedEventArgs e)
    {
        CheckForReset( exerciseDate, e.NewDate ); //Call function to check if Reset is required?
        SetSliderIfDataLogged(e.NewDate);

        exerciseDate = e.NewDate;
    }


    private async void CheckForReset(DateTime currentDate, DateTime selectedDate)
    {
        if (currentDate.Year != selectedDate.Year)
        {
            bool userResponse = await DisplayAlert("Confirm Reset Data", "The year has changed, do you want to reset data?", "Yes", "No");
            if (userResponse) 
            {
                _logger.ResetNewYear();
                _logger.LoadDataFromFile();
                DisplayStats();
                SetDatePicker(selectedDate);
            }
        }
    }


    private void SetSliderIfDataLogged(DateTime selectedDate)
    {
        ExerciseLog exerciseLog = _logger.GetExerciseLog(selectedDate);
        if (exerciseLog != null)
        {

            slider.Value = exerciseLog.MinsExercised;
            lblSlider.Text = Convert.ToInt32(slider.Value).ToString();
            lblLogStatus.Text = "You have already logged exercise for the selected date, you can modify and log";
        }
        else
        {
            slider.Value = 0;
            lblSlider.Text = Convert.ToInt32(slider.Value).ToString();
            lblLogStatus.Text = "";
        }
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
            if (_logger.IfDataExists(exerciseDate))
            {
                lblLogStatus.Text = $"You have updated your exercise record! Date: {exerciseDate}, Minutes exercised: {slider.Value}";
            }
            else 
            {
                lblLogStatus.Text = $"You have recorded your exercise record! Date: {exerciseDate}, Minutes exercised: {slider.Value}";
            }
            _logger.AddorUpdateLog(_log);
            _logger.LoadDataFromFile();
            DisplayStats();
            //slider.Value = 0;
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