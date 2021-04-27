using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Truck : Building
{
    public (int x, int y) cellPosition;

    public Transform GraphicsTransform;
    [NonSerialized] public int MaxLoad = 50;
    int MaxLoad_price = 25;
    [NonSerialized] public int CurrentLoad = 0;
    [NonSerialized] public int Speed = 2;
    int Speed_price = 50;

    [NonSerialized] public int CargoType = -1;
    [NonSerialized] public int LoadingSpeed = 5;
    int LoadingSpeed_price = 10;

    public Shapes.Line LoadIndicator;
    Queue<TruckAction> orders = new Queue<TruckAction>();
    Queue<Vector3> path;
    Vector3 currentDest;

    SpriteRenderer TruckSprite;



    TruckAction CurrentOrder = null;

    private void Start()
    {
        gameObject.name = "Truck " + GameManager.Instance.TruckCount;
        GameManager.Instance.TruckCount += 1;

        AudioManager.PlaySoundLocal(audioSource, "truck_spawn");
        TruckSprite = GetComponentInChildren<SpriteRenderer>();
        transform.rotation = Quaternion.Euler(0, 0, -90);
        // GetComponentInChildren<SpriteRenderer>().
        isTruck = true;
        OnRightClick = new IActions.Action_Generic(() =>
        {
            ShowInfo();
        });
        StartCoroutine(AIUpdateRoutine());
    }
    float timer = 0;
    string currentOrderName;

    public void SetCargoType(int cargoType)
    {
        //effects
        TruckSprite.sprite = DataContainer.Instance.TruckSprites[cargoType + 1];
        CargoType = cargoType;
    }
    void ResetOrder()
    {
        LoadIndicator.transform.parent.gameObject.SetActive(false);
        LoadIndicator.Start = new Vector3(-1, 0, 0);
        LoadIndicator.End = new Vector3(-1, 0, 0);

        timer = 0;
        CurrentOrder = null;
        currentDest = Vector3.zero;
        path = null;
        currentOrderName = "";
    }
    bool justSpawned = true;
    private void Update()
    {
        (int x, int y) currentCellPos = MapUtils.GetCellByWorldPos(transform.position);
        if (currentCellPos != cellPosition) cellPosition = currentCellPos;

        if (justSpawned)
        {
            AddMoveOrder(new TruckActions.MoveTo(this, (cellPosition.x + 3, cellPosition.y)));
            justSpawned = false;
        }
        if (CurrentOrder == null) return;
        currentOrderName = CurrentOrder.GetType().Name;

        if (CurrentOrder.type != 1)
        {
            if (timer == 0)
            {
                LoadIndicator.transform.parent.gameObject.SetActive(true);
                LoadIndicator.Start = new Vector3(-1, 0, 0);
                LoadIndicator.End = new Vector3(-1, 0, 0);
                Vector3 end = new Vector3(1, 0, 0);
                // loadIndicatorNewValue = (Mathf.PI * 2) / CurrentLoad;
                Debug.Log(CurrentOrder.timer);
                DOTween.To(() => LoadIndicator.End, x => LoadIndicator.End = x, end, CurrentOrder.timer);

            }
            timer += Time.deltaTime;
            if (timer >= CurrentOrder.timer)
            {
                CurrentOrder.isCompleted = true;
                CurrentOrder.OnCompleted();
                ResetOrder();
                return;
            }
            return;
        }
        if (CurrentOrder.type == 1)
        {
            if (currentDest == Vector3.zero)
            {

                if (path == null)
                {
                    // Debug.Log("Path null");

                    var mapc = MapController.Instance;
                    Pathfinder.FindPath(mapc.TerrainMap, mapc.MapW, mapc.MapH, currentCellPos, CurrentOrder.location, out path);
                    return;
                }
                if (path.Count > 0)
                {
                    // Debug.Log("set new dest");

                    currentDest = path.Dequeue();
                }
                // Debug.Log($"Dest 0, pathcount =  {path.Count}");
                return;
            }
            if (currentDest != Vector3.zero && Vector3.Distance(currentDest, transform.position) > 0.1f)
            {
                // Debug.Log($"delta {(currentDest)}");
                // Debug.Log($"delta {(transform.position)}");
                // Debug.Log($"delta {(currentDest - transform.position)}");
                // Debug.Log($"delta {(currentDest - transform.position).normalized}");
                // Debug.Log($"delta {Time.deltaTime}");
                // Debug.Log($"delta {Speed}");

                //  Debug.Log($"Moving delta {(currentDest - transform.position).normalized * Time.deltaTime * Speed}");
                // Vector2 delt =
                Vector3 dir = (currentDest - transform.position).normalized;
                DOTween.To(() => GraphicsTransform.up, x => GraphicsTransform.up = x, dir, 0.3f);// = dir;
                transform.position += dir * Time.deltaTime * Speed;
                return;
            }
            if (currentDest != Vector3.zero && Vector3.Distance(currentDest, transform.position) < 0.1f)
            {
                // Debug.Log("arrived tile");

                if (MapUtils.GetCellByWorldPos(transform.position) == CurrentOrder.location)
                {
                    Debug.Log("Arrived goal");

                    CurrentOrder.isCompleted = true;
                    CurrentOrder.OnCompleted();
                    ResetOrder();
                    return;
                }
                if (path.Count > 0)
                {
                    // Debug.Log("set new dest");

                    currentDest = path.Dequeue();
                }
                else { Debug.LogError("something unexpected happened with truck AI. Reach out to dev after he sleeps for a week pls"); }
            }
        }
    }

    public void AddOrder(TruckAction order)
    {
        Debug.Log("Adding new order to " + orders.Count + " existing");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if ((order.location != cellPosition && order.type != 1) || order.type != 1)
            {
                bool valid = AddMoveOrder(new TruckActions.MoveTo(this, order.location));
                if (valid) orders.Enqueue(order);
                Debug.Log("Adding new order to " + orders.Count + " existing, mode shift");
                return;
            }
            if (order.type == 1) AddMoveOrder(new TruckActions.MoveTo(this, order.location));
            else orders.Enqueue(order);
            Debug.Log("Adding new order to " + orders.Count + " existing, mode shift 1");

        }
        else
        {

            ResetOrder();
            orders.Clear();

            if ((order.location != cellPosition && order.type != 1) || order.type != 1)
            {
                bool valid = AddMoveOrder(new TruckActions.MoveTo(this, order.location));
                if (valid) orders.Enqueue(order);
                Debug.Log("Adding new order to " + orders.Count + " existing, mode normal");
                return;
            }
            if (order.type == 1) AddMoveOrder(new TruckActions.MoveTo(this, order.location));
            else orders.Enqueue(order);
            Debug.Log("Adding new order to " + orders.Count + " existing, mode normal 1");

        }
    }

    bool AddMoveOrder(TruckActions.MoveTo action)
    {
        var mapc = MapController.Instance;
        Queue<Vector3> tempPath;

        bool valid = Pathfinder.FindPath(mapc.TerrainMap, mapc.MapW, mapc.MapH, cellPosition, action.location, out tempPath);

        Debug.Log("Path is valid " + valid);

        if (valid)
        {
            orders.Enqueue(action);
            return true;
        }
        return false;
    }
    bool toasted = false;
    IEnumerator AIUpdateRoutine()
    {
        while (true)
        {
            // Debug.Log($"Q count {orders.Count}");

            if (CurrentOrder == null)
            {
                if (orders.Count == 0)
                {

                    if (!toasted)
                    {
                        ToastController.Instance.Toast("A truck is Idle");
                        toasted = true;
                    }
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    Debug.Log($"Q count {orders.Count}");
                    CurrentOrder = orders.Dequeue();
                    CurrentOrder.Init();
                    if (CurrentOrder.type == 1) currentDest = Vector3.zero;
                    //ToastController.Instance.Toast($"Truck order set {CurrentOrder.GetType().Name}");
                    currentOrderName = CurrentOrder.GetType().Name;

                    timer = 0;
                    toasted = false;
                }
            }
            else
            {
                if (!CurrentOrder.isCompleted)
                    yield return new WaitForSeconds(0.3f);
            }
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

    int maxSpeed = 5;
    public override void ShowInfo()
    {
        if (!isSelected) return;
        List<UIManager.InfoItemStruct> items = new List<UIManager.InfoItemStruct>();
        string load = "";
        switch (CurrentLoad)
        {
            case -1:
                load = "Nothing";
                break;
            case 0:
                load = "Bananas";
                break;
            case 2:
                load = "Fuel";
                break;
            case 3:
                load = "Monkeys!";
                break;
        }

        //items.Add(new UIManager.InfoItemStruct("Carrying:", $"{load}", false));
        items.Add(new UIManager.InfoItemStruct("Current Load:", $"{CurrentLoad}", false));
        items.Add(new UIManager.InfoItemStruct("Maximum Load:", $"{MaxLoad}", true, MaxLoad_price,
            () => { MaxLoad += 10; MaxLoad_price += 25; }));
        if (Speed >= maxSpeed) items.Add(new UIManager.InfoItemStruct("Speed:", $"{Speed}", false));
        else items.Add(new UIManager.InfoItemStruct("Speed:", $"{Speed}", true, Speed_price,
             () => { Speed += 1; Speed_price = Mathf.FloorToInt((Speed_price) * 1.5f); }));
        items.Add(new UIManager.InfoItemStruct("Loading Speed:", $"{LoadingSpeed}", true, LoadingSpeed_price,
            () => { LoadingSpeed += 1; LoadingSpeed_price *= 2; }));


        UIManager.Instance.ShowInfoPanel(this, name, items);
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
                if (action == null) return true;
                return !action.isCompleted;

            }
        }
    }
}