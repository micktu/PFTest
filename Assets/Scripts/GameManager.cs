using System;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public string ConfigFileName = "GameConfig";

    public GameSystem GameContainer;

    public enum StateType
    {
        MainMenu,
        InGame,
        PostGame
    }

    public StateType State { get; private set; }

    public Action GameStateChanged;
    public Action ScreenSizeChanged;

    public GameResults LastGameResults;

    private GlobalConfig _globalConfig;

    private MenuSystem _menuSystem;

    private ScreenOrientation _lastScreenOrientation;
    private int _lastScreenWidth;
    private int _lastScreenHeight;

    void Awake()
    {
        _globalConfig = GlobalConfig.Load(ConfigFileName);
        _menuSystem = GetComponent<MenuSystem>();
        _menuSystem.Init(this);

        GameContainer.GameEnded += EnterPostGame;
        GameContainer.Init(this, _globalConfig.GameConfig);
    }

    void Start()
    {
        EnterMainMenu();
    }

    private void Update()
    {
        CheckScreenResize();
    }

    void EnterState(StateType newState)
    {
        State = newState;

        GameStateChanged?.Invoke();
    }

    public void EnterMainMenu()
    {
        GameContainer.gameObject.SetActive(false);
        EnterState(StateType.MainMenu);
    }

    public void EnterGame()
    {
        GameContainer.Reset();
        GameContainer.gameObject.SetActive(true);
        EnterState(StateType.InGame);
    }

    public void EnterPostGame(GameResults results)
    {
        LastGameResults = results;
        EnterState(StateType.PostGame);
    }

    private void CheckScreenResize()
    {
        ScreenOrientation orientation = Screen.orientation;
        int width = Screen.width;
        int height = Screen.height;

        if (orientation != _lastScreenOrientation ||
            width != _lastScreenWidth ||
            height != _lastScreenHeight)
        {
            ScreenSizeChanged?.Invoke();

            _lastScreenOrientation = orientation;
            _lastScreenWidth = width;
            _lastScreenHeight = height;
        }
    }

    public void SaveState()
    {
        SaveState saveState = GameContainer.MakeSaveState();
        string json = JsonUtility.ToJson(saveState);
        PlayerPrefs.SetString("SaveState", json);
    }

    public void LoadState()
    {
        string json = PlayerPrefs.GetString("SaveState");

        if (string.IsNullOrEmpty(json)) return;

        SaveState saveState = JsonUtility.FromJson<SaveState>(json);
        GameContainer.RestoreFromSaveState(saveState);
    }

    public void SetGameSpeed(float speed)
    {
        GameContainer.TimeScale = speed;
    }
}
