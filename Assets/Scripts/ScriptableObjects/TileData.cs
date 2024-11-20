using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "GrandGames/Tile")]
public class TileData : ScriptableObject
{
    public TileType tileType;
    public string tileName;
    public Sprite tileSprite;
}

public enum TileType
{
    Purple,
    Red,
    Blue,
    Yellow,
    Cream,
    Brown,
    Green,
    Orange
}