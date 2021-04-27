using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public enum CursorMode { Select, Build }
    public static CursorMode CurrentCursorMode;
    static Dictionary<KeyCode, IAction> KeyActionDictionary;
    static List<KeyCode> DirtyKeyCodeList;

    static Building MouseOverBuilding;
    static Building CurrenSelectedBuilding;


    public static void SetSelectedBuilding(Building b)
    {
        if (CurrentCursorMode == CursorMode.Build) return;

        if (b == null)
        {
            // Debug.Log($"Deselect");
            if (CurrenSelectedBuilding != null)
            {
                CurrenSelectedBuilding.Deselect();
                AudioManager.StopSoundContinuous(CurrenSelectedBuilding.audioSource);
            }


        }
        else
        {
            if (CurrenSelectedBuilding != null) CurrenSelectedBuilding.Deselect();
            // Debug.Log($"Select {b.name}");
            if (b != null) b.Select();
            if (b.isTruck) AudioManager.PlaySoundContinuous(b.audioSource, "truck_ambient");
            if (b.isRefinery) AudioManager.PlaySoundContinuous(b.audioSource, "refinery_ambient_2");
            if (b.isShip) AudioManager.PlaySoundContinuous(b.audioSource, "ship_ambient");
            if (b.isExtractor) AudioManager.PlaySoundContinuous(b.audioSource, "extractor_ambient");

        }
        CurrenSelectedBuilding = b;
    }
    public static Building GetSelectedBuilding()
    {
        return CurrenSelectedBuilding;
    }

    public static void SetMouseOver(Building b)
    {
        if (CurrentCursorMode == CursorMode.Build) return;

        MouseOverBuilding = b;
        if (b != null)
        {
            InputManager.SetKeyAction(KeyCode.Mouse1, b.OnRightClick);
        }
        else
        {

            if (UIManager.Instance.ContextMenuInstance == null)
            {
                if (CurrenSelectedBuilding == null)
                {
                    return;
                }

                if (CurrenSelectedBuilding.isTruck)
                {
                    var pos = MapUtils.GetCellFromMousePos();

                    InputManager.SetKeyAction(KeyCode.Mouse1, new IActions.Action_OrderTruckMove((Truck)CurrenSelectedBuilding));
                    return;
                }

                InputManager.RemoveKeyAction(KeyCode.Mouse1);
            }


        }
    }
    public static Building GetMouseOver()
    {
        return MouseOverBuilding;
    }
    private void Start()
    {
        DirtyKeyCodeList = new List<KeyCode>();
        KeyActionDictionary = new Dictionary<KeyCode, IAction>();
        ResetCursor();
    }

    public static void SetKeyAction(KeyCode code, IAction action)
    {
        Debug.Log($"SetKeyAction {code} : {action.GetType().Name}");

        KeyActionDictionary[code] = action;
    }
    public static void RemoveKeyAction(KeyCode code)
    {
        Debug.Log($"RemoveKayAction {code}");

        DirtyKeyCodeList.Add(code);
    }

    void Update()
    {
        for (var i = 0; i < DirtyKeyCodeList.Count - 1; i++)
        {
            DirtyKeyCodeList.RemoveAt(i);
            KeyActionDictionary.Remove(DirtyKeyCodeList[i]);
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        BlueprintCursorFollower.Update();
        if (Input.GetMouseButtonUp(0))
        {
            if (KeyActionDictionary.ContainsKey(KeyCode.Mouse0))
            {
                // Debug.Log($"Mouse0");

                AudioManager.PlaySound("click");
                KeyActionDictionary[KeyCode.Mouse0].Execute();
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (KeyActionDictionary.ContainsKey(KeyCode.Mouse1))
            {
                AudioManager.PlaySound("click");
                KeyActionDictionary[KeyCode.Mouse1].Execute();
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            if (KeyActionDictionary.ContainsKey(KeyCode.Mouse2))
            {
                AudioManager.PlaySound("click");
                KeyActionDictionary[KeyCode.Mouse2].Execute();
            }
        }

        // foreach (var item in KeyActionDictionary)
        // {
        //     if (item.Key == KeyCode.Mouse0 || item.Key == KeyCode.Mouse1 || item.Key == KeyCode.Mouse2) continue;
        //     if (Input.GetKeyUp(item.Key))
        //     {
        //         AudioManager.PlaySound("click");
        //         KeyActionDictionary[item.Key].Execute();
        //     }
        // }


    }

    public static void ResetCursor()
    {
        UIManager.Instance.OnResetCursor();
        CurrentCursorMode = CursorMode.Select;
        BlueprintCursorFollower.EndCursorBlueprintFollow();
        RemoveKeyAction(KeyCode.Mouse1);
        SetKeyAction(KeyCode.Mouse0, new IActions.Action_TrySelect());
        SetSelectedBuilding(null);
        // SetKeyAction(KeyCode.Escape, new IActions.Action_ResetCursor());

    }


}


