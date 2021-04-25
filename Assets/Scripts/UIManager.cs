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

    public void OnBuildClicked(int id)
    {
        if (InputManager.CurrentCursorMode == InputManager.CursorMode.Build) return;
        InputManager.CurrentCursorMode = InputManager.CursorMode.Build;
        Sprite s = DataContainer.Instance.ConcreteGhostSprite;
        int fMode = 0;
        switch (id)
        {
            case 0: //probe
                BuildHintText.text = $"<color=red> One time use </color>orbital dropped probe. Reveals bananas. Temporarily scares off monkeys";
                BuildStatText.text = $"";

                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 3;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_DropProbe());

                break;
            case 1: //extractor
                BuildHintText.text = $"Extracts bananas from nature. Removes banana trees when they deplete. Must be fueled. <color=orange>Caution: </color> monkeys will steal bananas. pick up regularly with truck";
                BuildStatText.text = $"Must be placed on concrete";

                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 3;
                InputManager.SetKeyAction(KeyCode.Mouse0, new IActions.Action_PlaceExtractor());

                break;
            case 2: //oilrig
                BuildHintText.text = $"Produces banana oil from bananas. Uses monkeys as slave labour. <color=orange>Caution: </color> monkeys will escape regularly";
                BuildStatText.text = $"Must be placed on concrete";

                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 1;

                break;
            case 3: //trap

                BuildHintText.text = $"Traps monkeys as the stroll by. <color=orange>Caution: </color> monkeys will escape regularly, use truck to collect";
                BuildStatText.text = $"Must be placed on concrete";
                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 1;

                break;
            case 4: //concrete
                BuildHintText.text = $"<color=red> One time use </color>orbital dropped probe. Reveals bananas. Temporarily scares off monkeys";
                BuildStatText.text = $"Must be placed ajacent to concrete";

                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 1;

                break;
            case 5: // truck
                BuildHintText.text = $"Just a truck. Can be used to transport bananas and/or monkeys. Semi-intelligent driver (monkey?)";
                BuildStatText.text = $"Drives on concrete";
                BuildHintWindow.SetActive(true);
                s = DataContainer.Instance.ProbeGhostSprite;
                fMode = 2;
                break;
            case 6:
                break;

        }

        BlueprintCursorFollower.StartCursorBlueprintFollow(s, fMode);

    }


    public void OnResetCursor()
    {
        BuildHintWindow.SetActive(false);
        HideContextMenu();

    }

    public GameObject ContextMenuInstance;
    public GameObject ContextMenuPrefab;
    public GameObject ContextMenuButtonPrefab;


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
        }
    }

    public List<GameObject> InfoPanelItems = new List<GameObject>();
    public void ShowInfoPanel(string name, List<string> infoItems)
    {
        InfoPanel.GetComponentInChildren<TMP_Text>().text = name;
        if (!InfoPanel.activeSelf)
        {
            foreach (var item in infoItems)
            {
                var go = Instantiate(InfoItemPrefab, InfoPanel.transform);
                go.GetComponent<TMP_Text>().text = item;
                InfoPanelItems.Add(go);
            }
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
            while (InfoPanel.transform.childCount > 1)
            {
                Destroy(InfoPanel.transform.GetChild(1));
            }
        }
    }
}