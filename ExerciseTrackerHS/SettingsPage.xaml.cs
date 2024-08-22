using System.Diagnostics;

namespace ExerciseTrackerHS;

public partial class SettingsPage : ContentPage
{
	private UserData _userPreferences;
    private ExerciseLogger _logger;
    private ColorPair _colorPair;
    private readonly string[] _colors = { "Black", "White", "Gray", "Silver", "Red", "Lime", "Blue", "Yellow", "Cyan", "Magenta", "Maroon", "Olive", "Green", "Purple", "Teal", "Navy" };
    private Color _foregroundColor;
    private Color _backgroundColor;
    private Color _selectedColor;
	public SettingsPage(UserData userPreferences, ExerciseLogger logger)
	{
		InitializeComponent();

        _userPreferences = userPreferences;
        _logger = logger;

        _foregroundColor = _userPreferences.foreground;
        _backgroundColor = _userPreferences.background;
        maxExerciseSlider.Value = _userPreferences.maxDailyExercise;
        maxExerciseValue.Text = Convert.ToInt32(maxExerciseSlider.Value).ToString();

        PopulatePickers();
        
        UpdateUI();

    }

    private void PopulatePickers()
    {
       

        foreach (string color in _colors)
        {
            colorPicker.Items.Add(color);
        }

    }


    private void UpdateUI()
    {
        this.BackgroundColor = _userPreferences.background;
        UpdateColors(SettingStack);
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
                    case Picker picker:
                        picker.TextColor = _userPreferences.foreground;
                        break;
                    case Slider slider:
                        slider.MinimumTrackColor = _userPreferences.foreground;
                        slider.MaximumTrackColor = _userPreferences.foreground;
                        slider.BackgroundColor = _userPreferences.background;
                        break;
                }
            }
        }
    }
    private void OnSliderValueChanged(object sender, EventArgs e)
    {
        maxExerciseValue.Text = Convert.ToInt32(maxExerciseSlider.Value).ToString();
    }

    public void OnColorSelected(object sender, EventArgs e)
    {
        _selectedColor = Color.Parse(colorPicker.SelectedItem.ToString());
    }

    public void OnForegroundButtonClicked(object sender, EventArgs e)
    {
        _foregroundColor = _selectedColor;
        foregroundLabel.BackgroundColor = _foregroundColor;
    }

    public void OnBackgroundButtonClicked(object sender, EventArgs e)
    {
        _backgroundColor = _selectedColor;
        backgroundLabel.BackgroundColor = _backgroundColor;
    }

    public void OnSaveButtonClicked(object sender, EventArgs e)
    {
        _userPreferences.maxDailyExercise = Convert.ToInt32(maxExerciseSlider.Value);
        _userPreferences.foreground = _foregroundColor;
        _userPreferences.background = _backgroundColor;
        _userPreferences.SavePreferences();
        UpdateUI();
    }

    public void OnHomePageClicked(object sender, EventArgs e)
	{
        Navigation.PushModalAsync(new MainPage());
    }

	public void OnLogPageClicked(object sender, EventArgs e)
	{
        Navigation.PushModalAsync(new LogPage(_userPreferences, _logger));
    }
}