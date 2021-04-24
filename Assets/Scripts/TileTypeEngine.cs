using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTypeEngine : MonoBehaviour
{
    public static TileTypeEngine Instance;
    public Sprite SandSprite;
    public Sprite WaterSprite;
    public Sprite GroundSprite;
    public Sprite ConcreteSprite;
    public Sprite JungleSprite;


    private void Awake() => Instance = this;
}
