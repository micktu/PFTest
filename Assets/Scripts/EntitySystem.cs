using System;
using System.Collections.Generic;
using UnityEngine;


public class EntitySystem
{
    public List<Entity> AllEntities;

    public Action<int> EntityDied; 

    private List<int>[,] _collisionGrid;
    private int _numCellsX;
    private int _numCellsY;

    private GameConfig _gameConfig;


    public void Init(GameConfig gameConfig)
    {
        _gameConfig = gameConfig;
    }

    public void Reset()
    {
        float maxRadius = _gameConfig.maxUnitRadius;
        _numCellsX = Mathf.CeilToInt(_gameConfig.gameAreaWidth) + 3;
        _numCellsY = Mathf.CeilToInt(_gameConfig.gameAreaHeight) + 3;
        _collisionGrid = new List<int>[_numCellsX, _numCellsY];

        int numEntities = _gameConfig.numUnitsToSpawn;
        AllEntities = new List<Entity>(numEntities);
    }

    public void Update(float deltaTime)
    {
        for (int i = 0; i < AllEntities.Count; i++)
        {
            Entity entity = AllEntities[i];

            if (!entity.IsAlive) continue;

            Vector2 position = entity.Position;
            Vector2 velocity = entity.Velocity;
            float radius = entity.Radius;

            // Radius offset collapses the circle to a point.  
            float extentX = _gameConfig.gameAreaWidth / 2.0f - radius;
            float extentY = _gameConfig.gameAreaHeight / 2.0f - radius;

            Cell minCell = entity.Meta.MinCell;
            Cell maxCell = entity.Meta.MaxCell;

            Vector2 normal = new Vector2();

            // We use the grid to minimize unnecessary checks.
            for (int y = minCell.Y; y <= maxCell.Y; y++)
            {
                for (int x = minCell.X; x <= maxCell.X; x++)
                {
                    //Check collision against other entities.
                    foreach (int otherIndex in _collisionGrid[x, y])
                    {
                        if (otherIndex == entity.Meta.Index) continue;

                        Entity other = AllEntities[otherIndex];
                        if (!other.IsAlive) continue;

                        Vector2 otherPosition = other.Position;
                        float radiusSum = radius + other.Radius;

                        if (Vector2.Distance(position, otherPosition) > radiusSum) continue;

                        // We have a collision or a contained circle.
                        // For simplicity, we bounce in opposite directions.
                        if (entity.TeamIndex == other.TeamIndex)
                        {

                            Vector2 direction = (otherPosition - position).normalized;

                            velocity = -direction * velocity.magnitude;

                            other.Velocity = direction * other.Velocity.magnitude;
                        }
                        else
                        {
                            // Opposing entities get their sizes reduced.
                            float distance = (otherPosition - position).magnitude;
                            entity.Radius = distance - other.Radius;
                            other.Radius = distance - radius;
                        }

                        AllEntities[otherIndex] = other;
                        break;
                    }

                    // Game area border collision.
                    if (position.x <= -extentX)
                    {
                        normal = Vector2.right;
                    }
                    else if (position.x >= extentX)
                    {
                        normal = Vector2.left;
                    }
                    else if (position.y < -extentY)
                    {
                        normal = Vector2.up;

                    }
                    else if (position.y >= extentY)
                    {
                        normal = Vector2.down;
                    }

                    if (normal.x != 0.0f || normal.y != 0.0f)
                    {
                        break;
                    }
                }
            }

            if (entity.Radius < 0.5f)
            {
                entity.IsAlive = false;
                RemoveFromGrid(ref entity);
                AllEntities[i] = entity;
                EntityDied(i);
                continue;
            }

            // If it intersects the wall, reflect it.
            if (normal.x != 0.0f || normal.y != 0.0f)
            {
                velocity -= 2.0f * Vector2.Dot(velocity, normal) * normal;
            }

            position += velocity * deltaTime;

            RemoveFromGrid(ref entity);
            entity.Velocity = velocity;
            entity.Position = position;
            
            AddToGrid(ref entity);
            AllEntities[i] = entity;

        }
    }
    
    public int SpawnEntity()
    {
        float fieldWidth = _gameConfig.gameAreaWidth;
        float fieldHeight = _gameConfig.gameAreaHeight;

        float radius = UnityEngine.Random.Range(_gameConfig.minUnitRadius, _gameConfig.maxUnitRadius);

        Vector2 position = new Vector2
        {
            x = UnityEngine.Random.Range(-fieldWidth / 2.0f + radius, fieldWidth / 2.0f - radius),
            y = UnityEngine.Random.Range(-fieldHeight / 2.0f + radius, fieldHeight / 2.0f - radius)
        };

        Vector2 velocity = UnityEngine.Random.insideUnitCircle.normalized *
            UnityEngine.Random.Range(_gameConfig.minUnitSpeed, _gameConfig.maxUnitSpeed);

        Entity entity = new Entity
        {
            Position = position,
            Velocity = velocity,
            Radius = radius,
            TeamIndex = UnityEngine.Random.Range(0, 2),
            IsAlive = true,

            Meta = new EntityMeta
            {
                Index = AllEntities.Count
            }
        };

        AddToGrid(ref entity);
        AllEntities.Add(entity);
        return AllEntities.Count - 1;
    }

    private void RemoveFromGrid(ref Entity entity)
    {
        Cell minCell = entity.Meta.MinCell;
        Cell maxCell = entity.Meta.MaxCell;

        for (int y = minCell.Y; y <= maxCell.Y; y++)
        {
            for (int x = minCell.X; x <= maxCell.X; x++)
            {
                _collisionGrid[x, y].Remove(entity.Meta.Index);
            }
        }
    }

    private void AddToGrid(ref Entity entity)
    {
        SetEntityCells(ref entity);

        Cell minCell = entity.Meta.MinCell;
        Cell maxCell = entity.Meta.MaxCell;

        for (int y = minCell.Y; y <= maxCell.Y; y++)
        {
            for (int x = minCell.X; x <= maxCell.X; x++)
            {
                if (null == _collisionGrid[x, y])
                {
                    _collisionGrid[x, y] = new List<int>();
                }

                _collisionGrid[x, y].Add(entity.Meta.Index);
            }
        }
    }

    private void SetEntityCells(ref Entity entity)
    {
        float maxRadius = _gameConfig.maxUnitRadius;
        float offsetX = _gameConfig.gameAreaWidth / 2.0f;
        float offsetY = _gameConfig.gameAreaHeight / 2.0f;
        float radius = entity.Radius;

        int minX = Mathf.FloorToInt(offsetX + entity.Position.x - radius) + 1;
        if (minX < 0) minX = 0; 
        int maxX = Mathf.CeilToInt(offsetX + entity.Position.x + radius) + 1;
        if (maxX > _numCellsX - 1) maxX = _numCellsX - 1;

        int minY = Mathf.FloorToInt(offsetY + entity.Position.y - radius) + 1;
        if (minY < 0) minY = 0;
        int maxY = Mathf.CeilToInt(offsetY + entity.Position.y + radius) + 1;
        if (maxY > _numCellsY - 1) maxY = _numCellsY - 1;

        entity.Meta.MinCell = new Cell { X = minX, Y = minY };
        entity.Meta.MaxCell = new Cell { X = maxX, Y = maxY };
    }

    public void Restore(List<Entity> entities)
    {
        _collisionGrid = new List<int>[_numCellsX, _numCellsY];
        AllEntities.Clear();

        for (int i = 0; i < entities.Count; i++)
        {
            Entity entity = entities[i];
            entity.Meta.Index = i;
            AddToGrid(ref entity);
            AllEntities.Add(entity);
        }
    }

}
