using System;
using System.Collections.Generic;
using UnityEngine;


public class GameSystem : MonoBehaviour
{
    public GameObject GameArea;
    public GameObject EntityPrefab;
    public Material EntityMaterial;
    public int NumTeams = 2;
    public Color[] TeamColors;
    public float TimeScale = 1.0f;

    public Action<GameResults> GameEnded;
    public Action<int[]> ScoreChanged;

    private GameManager _gameManager;
    private GameConfig _gameConfig;

    public EntitySystem EntitySystem { get; private set; }
    private List<GameObject> _entityObjects;
    private List<Material> _teamMaterials;

    private int[] _teamCount;

    private float _gameTime;
    private float _nextSpawnTime;

    private bool _isActive;

    public void Init(GameManager gameManager, GameConfig gameConfig)
    {
        _gameManager = gameManager;
        _gameConfig = gameConfig;

        _gameManager.ScreenSizeChanged += OnScreenSizeChanged;

        EntitySystem = new EntitySystem();
        EntitySystem.Init(_gameConfig);
        EntitySystem.EntityDied += OnEntityDied;

        _entityObjects = new List<GameObject>(_gameConfig.numUnitsToSpawn);

        _teamMaterials = new List<Material>();
        foreach (Color color in TeamColors)
        {
            Material material = new Material(EntityMaterial);
            material.color = color;
            _teamMaterials.Add(material);
        }
    }

    public void Reset()
    {
        GameArea.transform.localScale = new Vector3
        {
            x = _gameConfig.gameAreaWidth,
            y = _gameConfig.gameAreaHeight,
            z = 1.0f
        };

        foreach (GameObject entityObject in _entityObjects.ToArray())
        {
            Destroy(entityObject);
        }

        _entityObjects.Clear();

        _nextSpawnTime = Time.realtimeSinceStartup + _gameConfig.unitSpawnDelay / 1000.0f;
        _teamCount = new int[NumTeams];
        _gameTime = 0.0f;

        EntitySystem.Reset();

        _isActive = true;
    }

    void Start()
    {
        OnScreenSizeChanged();
    }

    void Update()
    {
        if (!_isActive) return;

        float time = Time.realtimeSinceStartup;

        List<Entity> allEntities = EntitySystem.AllEntities;

        if (time >= _nextSpawnTime && _entityObjects.Count < _gameConfig.numUnitsToSpawn)
        {
            int index = EntitySystem.SpawnEntity();
            Entity entity = allEntities[index];
            _teamCount[entity.TeamIndex]++;
            ScoreChanged(_teamCount);

            MakeEntityObject(entity);

            _nextSpawnTime = GetNextSpawnTime();
        }

        time = Time.deltaTime * TimeScale;
        EntitySystem.Update(time);
        _gameTime += time;

        for (int i = 0; i < allEntities.Count; i++)
        {
            GameObject go = _entityObjects[i];

            Transform transform = go.transform;
            transform.localPosition = allEntities[i].Position;
            transform.localScale = Vector3.one * allEntities[i].Radius * 2.0f;
        }

        // Debugging thing
        //if (_gameTime > 3.0f)
        //{
        //    EndGame();
        //}
    }

    private void MakeEntityObject(Entity entity)
    {
        GameObject go = Instantiate(EntityPrefab);
        go.GetComponentInChildren<MeshRenderer>().sharedMaterial = _teamMaterials[entity.TeamIndex];

        if (!entity.IsAlive)
        {
            go.SetActive(false);
        }

        _entityObjects.Add(go);
    }

    private void OnScreenSizeChanged()
    {
        int width = Screen.width;
        int height = Screen.height;

        float size;
        if (width > height)
        {
            size = _gameConfig.gameAreaHeight;
        }
        else
        {
            size = _gameConfig.gameAreaWidth / width * height;
        }

        Camera.main.orthographicSize = size / 2.0f * 1.1f;
        Camera.main.transform.position = new Vector3
        {
            x = -size * 0.2f,
            y = 0.0f,
            z = -10.0f
        };
    }

    float GetNextSpawnTime()
    {
        return Time.realtimeSinceStartup + _gameConfig.unitSpawnDelay / 1000.0f;
    }

    public void OnEntityDied(int index)
    {
        Entity entity = EntitySystem.AllEntities[index];
        _teamCount[entity.TeamIndex]--;
        _entityObjects[index].SetActive(false);
        ScoreChanged(_teamCount);

        if (_teamCount[entity.TeamIndex] <= 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        _isActive = false;

        int winningTeam = _teamCount[1] > _teamCount[0] ? 1 : 0;

        GameResults results = new GameResults
        {
            TeamScore = _teamCount[winningTeam],
            WinningTeam = winningTeam,
            GameTime = _gameTime
        };

        GameEnded(results);
    }

    public SaveState MakeSaveState()
    {
        return new SaveState
        {
            Entities = EntitySystem.AllEntities,
            GameTime = _gameTime,
            TimeToSpawn = _nextSpawnTime
        };
    }

    public void RestoreFromSaveState(SaveState saveState)
    {
        Reset();
        EntitySystem.Restore(saveState.Entities);

        foreach(Entity entity in EntitySystem.AllEntities)
        {
            if (entity.IsAlive)
            {
                _teamCount[entity.TeamIndex]++;
            }
            MakeEntityObject(entity);
        }

        _gameTime = saveState.GameTime;
        _nextSpawnTime = saveState.TimeToSpawn;
        ScoreChanged(_teamCount);
    }
}
