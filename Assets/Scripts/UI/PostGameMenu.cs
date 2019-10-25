using UnityEngine.Events;
using UnityEngine.UI;

public class PostGameMenu : BaseMenu
{
    public Text WinningTeamText;
    public Text ScoreText;
    public Text TimeText;
    public Button RestartButton;
    public Button MainMenuButton;

    public UnityAction RestartGameAction;
    public UnityAction MainMenuAction;

    void Start()
    {
        RestartButton.onClick.AddListener(RestartGameAction);
        MainMenuButton.onClick.AddListener(MainMenuAction);
    }

    public void UpdateValues(GameResults results)
    {
        if (results.WinningTeam == 0)
        {
            WinningTeamText.text = "Blue Team Wins!";
        }
        else if (results.WinningTeam == 1)
        {
            WinningTeamText.text = "Red Team Wins!";
        }

        ScoreText.text = string.Format("{0} guys left alive.", results.TeamScore);
        TimeText.text = string.Format("Game lasted {0:###.##} seconds.", results.GameTime);
    }
}
