using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using DG.Tweening;

public class ContextMenu : MonoBehaviour
{
    GameObject ToolTipPrefab;
    public string TextKey;

    bool isPinned;
    bool isShown;
    GameObject ToolTipInstance;
    RectTransform ToolTipRectTransform;
    TMP_Text ToolTipTMP_Text;

    bool isHovering;
    private void Awake()
    {
        ToolTipPrefab = Resources.Load<GameObject>("Prefabs/UI/ToolTipPrefab");
        //GetComponent<Button>().SetOnHoverDelayedAction(() => ShowToolTip(), 0.5f);
        //GetComponent<Button>().onPointerExit = () => HideToolTip();

    }
    private void ShowToolTip()
    {

        if (ToolTipInstance != null) return;
        ToolTipInstance = Instantiate(ToolTipPrefab);
        ToolTipInstance.transform.SetParent(transform);
        ToolTipRectTransform = ToolTipInstance.GetComponent<RectTransform>();

        ToolTipTMP_Text = ToolTipInstance.GetComponentInChildren<TMP_Text>();

        ToolTipTMP_Text.text = TextKey;
        ToolTipTMP_Text.ForceMeshUpdate();

        var width = ToolTipTMP_Text.textInfo.lineInfo[0].lineExtents.max.x * 2 + 10;
        var height = ToolTipTMP_Text.preferredHeight + 10;

        ToolTipRectTransform.sizeDelta = new Vector2(width, height);

        bool right = transform.position.x + width + 10 <= Screen.width;
        bool up = transform.position.y + height + 10 <= Screen.height;

        Vector2 newPivot = new Vector2();
        newPivot.x = right ? 0 : 1;
        newPivot.y = up ? 0 : 1;
        Vector2 newPosition = new Vector2();
        newPosition.x = right ? 10 : -10;
        newPosition.y = up ? 10 : -10;

        ToolTipRectTransform.pivot = newPivot;
        ToolTipRectTransform.anchoredPosition = newPosition;
        var scale = ToolTipRectTransform.localScale;
        var halfScale = scale * 0.5f;
        ToolTipRectTransform.localScale = halfScale;
        isShown = true;
        ToolTipRectTransform.DOScale(scale, 0.05f);

    }

    private void HideToolTip()
    {
        isShown = false;

        Destroy(ToolTipInstance);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        StartCoroutine(OnHoverDelayRoutine(1f));

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        HideToolTip();
    }


    IEnumerator OnHoverDelayRoutine(float delay)
    {

        yield return new WaitForSeconds(delay);
        if (isHovering)
            ShowToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isHovering = false;
        HideToolTip();
    }
}