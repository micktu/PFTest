using UnityEngine;


public class MenuSystem : MonoBehaviour
{
    public Canvas GameCanvas;

    public MainMenu MainMenuPrefab;
    public InGameMenu InGameMenuPrefab;
    public PostGameMenu PostGameMenuPrefab;


    private GameManager _gameManager;

    private MainMenu _mainMenu;
    private InGameMenu _inGameMenu;
    private PostGameMenu _postGameMenu;

    private BaseMenu _currentMenu;


    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
        _gameManager.GameStateChanged += OnGameStateChanged;

        Transform canvasTransform = GameCanvas.transform;

        _mainMenu = SpawnMenu(MainMenuPrefab);
        _mainMenu.StartGameAction = _gameManager.EnterGame;

        _inGameMenu = SpawnMenu(InGameMenuPrefab);
        _gameManager.GameContainer.ScoreChanged += _inGameMenu.UpdateScore;
        _inGameMenu.LoadStateAction = _gameManager.LoadState;
        _inGameMenu.SaveStateAction = _gameManager.SaveState;
        _inGameMenu.ChangeSpeedAction = _gameManager.SetGameSpeed;

        _postGameMenu = SpawnMenu(PostGameMenuPrefab);
        _postGameMenu.RestartGameAction = _gameManager.EnterGame;
        _postGameMenu.MainMenuAction = _gameManager.EnterMainMenu;
    }

    private T SpawnMenu<T>(T menuPrefab) where T : BaseMenu
    {
        T menu = Instantiate(menuPrefab);
        menu.transform.SetParent(GameCanvas.transform, false);
        menu.gameObject.SetActive(false);
        return menu;
    }

    private void OnGameStateChanged()
    {
        if (null != _currentMenu)
        {
            _currentMenu.LeaveMenu();
        }

        switch (_gameManager.State)
        {
            case GameManager.StateType.MainMenu:
                _currentMenu = _mainMenu;
                break;
            case GameManager.StateType.InGame:
                _currentMenu = _inGameMenu;
                break;
            case GameManager.StateType.PostGame:
                _postGameMenu.UpdateValues(_gameManager.LastGameResults);
                _currentMenu = _postGameMenu;
                break;
        }

        _currentMenu.EnterMenu();
    }
}
