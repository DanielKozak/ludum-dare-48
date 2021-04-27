using UnityEngine;
using TMPro;
using DG.Tweening;
public class ToastInstance : MonoBehaviour
{
    public bool isPersistent;

    public TMP_Text text;
    public float duration = 0.5f;
    public void Kill()
    {
        Debug.Log("Toast mouse up");
        float oldY = GetComponent<RectTransform>().anchoredPosition.y;
        text.DOColor(new Color(0, 0, 0, 0), duration).SetDelay(duration * 0.5f);
        GetComponent<RectTransform>().DOAnchorPosY(50, duration).SetDelay(duration * 0.5f);
        Destroy(gameObject, duration);
    }

    public void StartToastKill(float time)
    {
        if (isPersistent) return;
        duration = time;
        float oldY = GetComponent<RectTransform>().anchoredPosition.y;
        text.DOColor(new Color(0, 0, 0, 0), duration).SetDelay(duration * 0.5f);
        GetComponent<RectTransform>().DOAnchorPosY(50, duration).SetDelay(duration * 0.5f);
        Destroy(gameObject, duration);
    }


}

