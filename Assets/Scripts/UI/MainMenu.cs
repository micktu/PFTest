using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    public Button StartButton;

    public UnityAction StartGameAction;

    void Start()
    {
        StartButton.onClick.AddListener(StartGameAction);
    }
}
