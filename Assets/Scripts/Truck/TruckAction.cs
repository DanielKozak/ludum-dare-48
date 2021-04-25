using UnityEngine;
public abstract class TruckAction
{

    public float timer;
    public (int, int) location;
    public int type = 0;
    public bool isCompleted = false;

    public abstract void OnCompleted();
}