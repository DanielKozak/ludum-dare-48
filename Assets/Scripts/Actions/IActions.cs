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
}
