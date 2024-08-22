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
        maxExerciseValue.Text = Convert.ToInt32(maxExerciseSlider.Value).ToString();

        PopulatePickers();
        
        UpdateUI();

    }

    private void PopulatePickers()
    {
        List<ColorPair> colorView = null;
        colorView= new List<ColorPair>();
        //{
        //    new ColorPair{"Black", Colors.Black },
        //    new ColorPair{"White", Colors.White },
        //    new ColorPair{"Gray", Colors.Gray },
        //    new ColorPair{"Silver", Colors.Silver },
        //    new ColorPair{"Red", Colors.Red },
        //    new ColorPair{"Lime", Colors.Lime },
        //    new ColorPair{"Blue", Colors.Blue },
        //    new ColorPair{"Yellow", Colors.Yellow },
        //    new ColorPair{"Cyan", Colors.Cyan },
        //    new ColorPair{"Magenta" Colors.Magenta },
        //    new ColorPair{"Maroon", Colors.Maroon },
        //    new ColorPair{"Olive", Colors.Olive },
        //    new ColorPair{"Green", Colors.Green },
        //    new ColorPair{"Purple", Colors.Purple },
        //    new ColorPair{"Teal", Colors.Teal },
        //    new ColorPair{"Navy", Colors.Navy }
        //};
        //colorViewSelector.ItemsSource= colorView;

        foreach (string color in _colors)
        {
            colorPicker.Items.Add(color);
            colorView.Add(new ColorPair(color, Color.Parse(color)));
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
                        //button.BackgroundColor = _userPreferences.background;
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

    private void OnColorItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is ColorPair selectedItemColor)
        {
            colorDisplayBox.Color = selectedItemColor.SelectedColor;
            ((ListView)sender).SelectedItem = null; // Deselect item
        }
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