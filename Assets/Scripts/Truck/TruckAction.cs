using UnityEngine;
public abstract class TruckAction
{

    public float timer;
    public int amount = 1;

    public (int, int) location;
    public int type = 0;
    public bool isCompleted = false;

    public abstract void OnCompleted();
    public abstract void Init();
}