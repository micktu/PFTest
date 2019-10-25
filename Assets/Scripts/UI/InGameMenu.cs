using UnityEngine.Events;
using UnityEngine.UI;


public class InGameMenu : BaseMenu
{
    public Text WinningText;
    public Text BlueScore;
    public Text RedScore;
    public Button LoadButton;
    public Button SaveButton;
    public Slider SpeedSlider;

    public UnityAction LoadStateAction;
    public UnityAction SaveStateAction;
    public UnityAction<float> ChangeSpeedAction;

    void Start()
    {
        LoadButton.onClick.AddListener(LoadStateAction);
        SaveButton.onClick.AddListener(SaveStateAction);

        SpeedSlider.minValue = 0.25f;
        SpeedSlider.maxValue = 4.0f;
        SpeedSlider.value = 1.0f;
        SpeedSlider.onValueChanged.AddListener(ChangeSpeedAction);
    }

    public void UpdateScore(int[] teamScores)
    {
        int blueScore = teamScores[0];
        int redScore = teamScores[1];

        BlueScore.text = string.Format("Blue guys: {0}", blueScore);
        RedScore.text = string.Format("Red guys: {0}", redScore);

        if (blueScore > redScore)
        {
            WinningText.text = "Blue team is winning!";
        }
        else if (redScore > blueScore)
        {
            WinningText.text = "Red team is winning!";
        }
    }
}
