using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake() => Instance = this;
    public GameObject BuildHintWindow;
    public TMP_Text BuildHintText;
    public TMP_Text BuildStatText;

    public GameObject InfoPanel;
    public GameObject InfoItemPrefab;

    bool hintPinned = false;


    public void ShowBuildHint(string text1, string text2 = "")
    {
        if (hintPinned) return;
        BuildHintText.text = text1;
        BuildStatText.text = text2;

        BuildHintWindow.SetActive(true);
    }

    public void HideBuildHint()
    {
        BuildHintWindow.SetActive(false);

    }

    public void OnBuildClicked(int id)
    {
        Debug.Log("Build " + id);
        if (InputManager.CurrentCursorMode == InputManager.CursorMode.Build) return;
        InputManager.CurrentCursorMode = InputManager.CursorMode.Build;
        Sprite s = DataContainer.Instance.ConcreteGhostSprite;
        int fMode = 0;

        switch (id)
        {
            case 0: //probe
                ShowBuildHint($"<color=red> One time use </color>orbital dropped probe. Reveals bananas. Temporarily scares off monkeys");
                hintPinned = true;
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 3;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_DropProbe());

                break;
            case 1: //extractor
                ShowBuildHint($"Extracts bananas from nature. Removes banana trees when they deplete. Must be fueled. <color=orange>Caution: </color> monkeys will steal bananas. pick up regularly with truck",
                    $"Must be placed on concrete");
                hintPinned = true;

                s = DataContainer.Instance.ExtractorGhostSprite;
                fMode = 1;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_PlaceExtractor());

                break;
            case 2: //oilrig
                ShowBuildHint($"Produces banana oil from bananas. Uses monkeys as slave labour. <color=orange>Caution: </color> monkeys will escape regularly", $"Must be placed on concrete");
                hintPinned = true;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_PlaceRefinery());

                s = DataContainer.Instance.OilGhostSprite;
                fMode = 1;

                break;
            case 3: //trap
                ShowBuildHint($"Traps monkeys as they stroll by. <color=orange>Caution: </color> monkeys will escape regularly, use truck to collect", $"Must be placed on concrete");
                hintPinned = true;

                s = DataContainer.Instance.TrapGhostSprite;
                fMode = 1;

                break;
            case 4: //concrete
                ShowBuildHint($"Concrete road. used as a base for all your industrialisation efforts.\n <color=red> Say NO to rainforests! </color>", $"Must be placed ajacent to another road");
                hintPinned = true;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_PlaceRoad());

                s = DataContainer.Instance.ConcreteGhostSprite;
                fMode = 2;

                break;
            case 5: // truck
                // ShowBuildHint($"Just a truck. Can be used to transport bananas and/or monkeys. Semi-intelligent driver (monkey?)", $"Drives on concrete");

                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 4;
                break;
            case 6:
                ShowBuildHint($"Bulldoze for half price", $"");
                hintPinned = true;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_Bulldoze());

                s = DataContainer.Instance.ConcreteGhostSprite;
                fMode = 3;
                break;

        }
        Debug.Log("Build confirm " + id);

        BlueprintCursorFollower.StartCursorBlueprintFollow(s, fMode);

    }


    public void OnResetCursor()
    {
        hintPinned = false;
        HideBuildHint();
        HideContextMenu();

    }

    public GameObject ContextMenuInstance;
    public GameObject ContextMenuPrefab;
    public GameObject ContextMenuButtonPrefab;

    Action oldTruckAction;
    public void ShowContextMenu(string firstText, Action firstAction, string secondText = null, Action secondAction = null, string thirdText = null, Action thirdAction = null)
    {

        if (ContextMenuInstance != null) return;

        InputManager.SetKeyAction(KeyCode.Mouse1, new IActions.Action_Generic(() => UIManager.Instance.HideContextMenu()));
        InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_Generic(() => UIManager.Instance.HideContextMenu()));

        ContextMenuInstance = Instantiate(ContextMenuPrefab, transform);

        var button = Instantiate(ContextMenuButtonPrefab);
        button.transform.SetParent(ContextMenuInstance.transform, true);
        button.GetComponentInChildren<TMP_Text>().text = firstText;
        button.GetComponent<Button>().onClick.AddListener(() => firstAction());
        button.GetComponent<Button>().onClick.AddListener(() => HideContextMenu());

        if (secondAction != null)
        {
            button = Instantiate(ContextMenuButtonPrefab);
            button.transform.SetParent(ContextMenuInstance.transform, true);
            button.GetComponentInChildren<TMP_Text>().text = secondText;
            button.GetComponent<Button>().onClick.AddListener(() => secondAction());
            button.GetComponent<Button>().onClick.AddListener(() => HideContextMenu());

        }
        if (thirdAction != null)
        {
            button = Instantiate(ContextMenuButtonPrefab);
            button.transform.SetParent(ContextMenuInstance.transform, true);
            button.GetComponentInChildren<TMP_Text>().text = thirdText;
            button.GetComponent<Button>().onClick.AddListener(() => thirdAction());
            button.GetComponent<Button>().onClick.AddListener(() => HideContextMenu());

        }





        var ContextMenuRectTransform = ContextMenuInstance.GetComponent<RectTransform>();


        var width = ContextMenuRectTransform.sizeDelta.x;
        var height = ContextMenuRectTransform.sizeDelta.x;

        // ContextMenuRectTransform.sizeDelta = new Vector2(width, height);

        bool right = transform.position.x + width + 10 <= Screen.width;
        bool up = transform.position.y + height + 10 <= Screen.height;

        Vector2 newPivot = new Vector2();
        newPivot.x = right ? 0 : 1;
        newPivot.y = up ? 0 : 1;

        ContextMenuRectTransform.pivot = newPivot;
        //TODO to mouse position
        // ContextMenuRectTransform.anchoredPosition = RectTransformUtility.SC.main.ScreenToViewportPoint(Input.mousePosition);
        var scale = ContextMenuRectTransform.localScale;
        var halfScale = scale * 0.5f;
        ContextMenuRectTransform.localScale = halfScale;
        ContextMenuRectTransform.DOScale(scale, 0.05f);

    }

    public void HideContextMenu()
    {
        if (ContextMenuInstance != null)
        {
            Destroy(ContextMenuInstance);

            InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_TrySelect());
            if (InputManager.GetSelectedBuilding() != null && InputManager.GetSelectedBuilding().isTruck)
                InputManager.SetKeyAction(KeyCode.Mouse1, new IActions.Action_OrderTruckMove((Truck)InputManager.GetSelectedBuilding()));

        }
    }

    public List<GameObject> InfoPanelItems = new List<GameObject>();
    public void ShowInfoPanel(string name, List<string> infoItems)
    {
        if (!InfoPanel.activeSelf)
        {
            InfoPanel.GetComponentInChildren<TMP_Text>().text = name;
            foreach (var item in infoItems)
            {
                var go = Instantiate(InfoItemPrefab, InfoPanel.transform);
                go.GetComponentInChildren<TMP_Text>().text = item;
                InfoPanelItems.Add(go);
            }
            InfoPanel.SetActive(true);
            return;
        }
    }
    public void UpdateInfoPanel(int index, string value)
    {
        if (InfoPanel.activeSelf)
        {
            InfoPanelItems[index].GetComponent<TMP_Text>().text = value;
        }
    }
    public void HideInfoPanel()
    {
        InfoPanelItems.Clear();
        if (InfoPanel.activeSelf)
        {
            for (var i = 1; i < InfoPanel.transform.childCount; i++)
            {
                Destroy(InfoPanel.transform.GetChild(i).gameObject);
            }
            InfoPanel.SetActive(false);

        }
    }
}