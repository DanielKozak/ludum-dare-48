using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IActions
{
    public class Action_PlaceAttractor : IAction
    {
        public override void Execute()
        {


        }
    }
    public class Action_Generic : IAction
    {

        public Action action;

        public Action_Generic(Action a)
        {
            action = a;
        }
        public override void Execute()
        {
            action?.Invoke();
        }
    }

    public class Action_ResetCursor : IAction
    {
        public override void Execute()
        {
            InputManager.ResetCursor();
            UIManager.Instance.OnResetCursor();

        }
    }
    public class Action_TrySelect : IAction
    {
        public override void Execute()
        {
            Debug.Log($"TrySelect");
            if (InputManager.GetMouseOver() == null)
            {
                Debug.Log($"Nothing here to select");

                if (InputManager.GetSelectedBuilding() != null)
                {
                    Debug.Log($"Deselecting");

                    InputManager.ResetCursor();
                }
                return;
            }
            Debug.Log($"Selecting {InputManager.GetMouseOver().name}");

            InputManager.SetSelectedBuilding(InputManager.GetMouseOver());
            // if (InputManager.GetSelectedBuilding().OnLeftClick != null) InputManager.SetKeyAction(KeyCode.Mouse0, InputManager.GetSelectedBuilding().OnLeftClick);
            if (InputManager.GetSelectedBuilding().OnRightClick != null) InputManager.SetKeyAction(KeyCode.Mouse1, InputManager.GetSelectedBuilding().OnRightClick);

        }
    }
    public class Action_DropProbe : IAction
    {
        public override void Execute()
        {
            if (!BlueprintCursorFollower.CanPlace)
            {
                //Effect
                return;
            }
            var pos = MapUtils.GetCellFromMousePos();
            var go = GameManager.Instantiate(DataContainer.Instance.ProbePrefab, GameManager.Instance.transform);
            go.GetComponent<Probe>().Drop(pos);
            go.name = "Probe";



        }
    }

    public class Action_PlaceExtractor : IAction
    {
        public override void Execute()
        {
            if (!BlueprintCursorFollower.CanPlace)
            {
                //Effect
                return;
            }

            var pos = MapUtils.GetCellFromMousePos();
            var go = GameManager.Instantiate(DataContainer.Instance.ExtractorPrefab, GameManager.Instance.transform);
            go.name = "BananaExtractor";
            go.GetComponent<BananaExtractor>().Place(pos);

            //Effects


            InputManager.ResetCursor();
            // UIManager.Instance.OnResetCursor();

        }
    }
    public class Action_PlaceRefinery : IAction
    {
        public override void Execute()
        {
            if (!BlueprintCursorFollower.CanPlace)
            {
                //Effect
                return;
            }

            var pos = MapUtils.GetCellFromMousePos();
            var go = GameManager.Instantiate(DataContainer.Instance.RefineryPrefab, GameManager.Instance.transform);
            go.name = "Banana Oil Refinery";
            go.GetComponent<Refinery>().Place(pos);

            //Effects


            InputManager.ResetCursor();
            // UIManager.Instance.OnResetCursor();

        }
    }


    public class Action_PlaceRoad : IAction
    {
        public override void Execute()
        {
            if (!BlueprintCursorFollower.CanPlace)
            {
                //Effect
                return;
            }

            var pos = MapUtils.GetCellFromMousePos();
            MapController.Instance.SetTileValue(pos, 2);
            GameManager.Instance.SetBananaBalance(-5);

            // UIManager.Instance.OnResetCursor();

        }
    }

    public class Action_Bulldoze : IAction
    {
        public override void Execute()
        {
            if (InputManager.GetMouseOver() != null)
            {
                var building = InputManager.GetMouseOver();
                GameManager.Instance.SetBananaBalance(Mathf.FloorToInt(building.price * 0.5f));
                GameManager.Destroy(InputManager.GetMouseOver());
                return;
            }


            var pos = MapUtils.GetCellFromMousePos();

            int tileType = MapController.Instance.TerrainMap.GetValue(pos);

            if (tileType == -1) return;
            MapController.Instance.SetTileValue(pos, tileType - 1);

            // UIManager.Instance.OnResetCursor();

        }
    }
    public class Action_OrderTruckMove : IAction
    {
        Truck truck;
        (int, int) targetPos;
        public Action_OrderTruckMove(Truck agent, (int, int) target)
        {
            truck = agent;
            targetPos = target;
        }
        public Action_OrderTruckMove(Truck agent)
        {
            truck = agent;

        }
        public override void Execute()
        {
            targetPos = MapUtils.GetCellFromMousePos();
            truck.AddOrder(new TruckActions.MoveTo(truck, targetPos));
        }
    }
}
