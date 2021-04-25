using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class ToastController : MonoBehaviour
{
    public static ToastController Instance;
    private void Awake() => Instance = this;
    [SerializeField] private GameObject ToastPrefab;

    Color urgentColor = new Color(1f, 0.5f, 0.3f, 1f);

    public List<string> ToastHistory = new List<string>();


    public void Toast(string text, bool urgent = false, float duration = 4f)
    {
        Debug.Log($"<color=cyan>Toast </color>{text}");
        GameObject newToast = Instantiate(ToastPrefab);
        newToast.transform.SetParent(gameObject.transform);
        // newToast.transform.SetAsFirstSibling();
        var tmpText = newToast.GetComponent<TextMeshProUGUI>();
        var color = urgent ? urgentColor : Color.white;

        tmpText.text = text;
        tmpText.color = color;

        ToastHistory.Add(text);
        AudioManager.PlaySound("toast");
        float oldY = newToast.GetComponent<RectTransform>().anchoredPosition.y;
        tmpText.DOColor(new Color(0, 0, 0, 0), duration).SetDelay(duration * 0.5f);
        newToast.GetComponent<RectTransform>().DOAnchorPosY(50, duration).SetDelay(duration * 0.5f);
        Destroy(newToast, duration);

    }

}
