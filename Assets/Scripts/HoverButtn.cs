using UnityEngine;
using UnityEngine.EventSystems;


public class HoverButtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text1;
    public string text2;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowBuildHint(text1, text2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideBuildHint();
    }

}