using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Truck : Building
{
    (int x, int y) cellPosition;

    [NonSerialized] public int MaxLoad = 50;
    [NonSerialized] public int CurrentLoad = 0;
    [NonSerialized] public int Speed = 0;
    Queue<TruckAction> orders = new Queue<TruckAction>();
    Queue<Vector3> path;
    Vector3 currentDest;

    TruckAction CurrentOrder = null;

    private void Start()
    {
        isTruck = true;
        StartCoroutine(AIUpdateRoutine());
    }
    float timer = 0;
    private void Update()
    {
        (int x, int y) currentCellPos = MapUtils.GetCellByWorldPos(transform.position);
        if (currentCellPos != cellPosition) cellPosition = currentCellPos;
        if (CurrentOrder.type != 1)
        {
            timer += Time.deltaTime;
            //TODO visualise
            if (timer >= CurrentOrder.timer)
            {
                CurrentOrder.isCompleted = true;
                CurrentOrder.OnCompleted();
                return;
            }
            return;
        }
        if (CurrentOrder.type == 1)
        {
            if (currentDest == null)
            {
                if (path == null)
                {
                    var mapc = MapController.Instance;
                    Pathfinder.FindPath(mapc.TerrainMap, mapc.MapW, mapc.MapH, currentCellPos, CurrentOrder.location, out path);
                }
                return;
            }
            if (currentDest != null && Vector3.Distance(currentDest, transform.position) > 0.1f)
            {
                transform.position += (transform.position - currentDest).normalized * Time.deltaTime * Speed;
                return;
            }
            if (currentDest != null && Vector3.Distance(currentDest, transform.position) < 0.1f)
            {
                if (MapUtils.GetCellByWorldPos(transform.position) == CurrentOrder.location)
                {
                    CurrentOrder.isCompleted = true;
                    CurrentOrder.OnCompleted();
                    path = null;
                    return;
                }
                if (path.Count > 0)
                {
                    currentDest = path.Dequeue();
                }
                else { Debug.LogError("something unexpected happened with truck AI. Reach out to me pls"); }
            }
        }
    }

    public void AddOrder(TruckAction order)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (order.location != cellPosition || order.type != 1)
                orders.Enqueue(new TruckActions.MoveTo(this, order.location));
            orders.Enqueue(order);
        }
        else
        {
            orders.Clear();
            CurrentOrder.isCompleted = true;
            if (order.location != cellPosition || order.type != 1)
                orders.Enqueue(new TruckActions.MoveTo(this, order.location));
            orders.Enqueue(order);
        }
    }
    bool toasted = false;
    IEnumerator AIUpdateRoutine()
    {
        while (true)
        {
            if (CurrentOrder == null)
            {
                if (orders.Count == 0)
                {
                    if (!toasted)
                    {
                        ToastController.Instance.Toast("A truck is Idle");
                        toasted = true;
                    }
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                else
                {
                    CurrentOrder = orders.Dequeue();
                    timer = 0;
                    toasted = false;
                }
            }
            else yield return new WaitForTaskCompleted(CurrentOrder);
        }
    }

    // public override void Select()
    // {
    //     throw new System.NotImplementedException();
    // }

    // public override void Deselect()
    // {
    //     throw new System.NotImplementedException();
    // }
    public override void ShowInfo()
    {

    }

    public class WaitForTaskCompleted : CustomYieldInstruction
    {
        TruckAction action;
        public WaitForTaskCompleted(TruckAction currentTask)
        {
            action = currentTask;
            Debug.Log("Waiting for task");
        }
        public override bool keepWaiting
        {
            get
            {
                return !action.isCompleted;
            }
        }
    }
}