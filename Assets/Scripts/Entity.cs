using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Entity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Radius;
    public int TeamIndex;
    public bool IsAlive;

    [NonSerialized]
    public EntityMeta Meta;
}

public struct EntityMeta
{
    public int Index;
    public Cell MinCell;
    public Cell MaxCell;
}

public struct Cell
{
    public int X;
    public int Y;

    public override int GetHashCode()
    {
        return X ^ Y;
    }
}

public struct GameResults
{
    public int WinningTeam;
    public int TeamScore;
    public float GameTime;
}

[Serializable]
public struct SaveState
{
    public List<Entity> Entities;
    public float GameTime;
    public float TimeToSpawn;
}
