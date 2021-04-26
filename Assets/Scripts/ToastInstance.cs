using UnityEngine;
using TMPro;
using DG.Tweening;
public class ToastInstance : MonoBehaviour
{
    public bool isPersistent;
    public float duration = 0.5f;
    public void Kill()
    {
        Debug.Log("Toast mouse up");
        if (!isPersistent) return;
        float oldY = GetComponent<RectTransform>().anchoredPosition.y;
        GetComponent<TextMeshPro>().DOColor(new Color(0, 0, 0, 0), duration).SetDelay(duration * 0.5f);
        GetComponent<RectTransform>().GetComponent<RectTransform>().DOAnchorPosY(50, duration).SetDelay(duration * 0.5f);
        Destroy(gameObject, duration);
    }

    public void StartToastKill(float time)
    {
        if (isPersistent) return;
        duration = time;
        float oldY = GetComponent<RectTransform>().anchoredPosition.y;
        GetComponent<TMP_Text>().DOColor(new Color(0, 0, 0, 0), duration).SetDelay(duration * 0.5f);
        GetComponent<RectTransform>().GetComponent<RectTransform>().DOAnchorPosY(50, duration).SetDelay(duration * 0.5f);
        Destroy(gameObject, duration);
    }


}

