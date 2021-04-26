using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ToastController : MonoBehaviour
{
    public static ToastController Instance;
    private void Awake() => Instance = this;
    [SerializeField] private GameObject ToastPrefab;

    Color urgentColor = new Color(1f, 0.5f, 0.3f, 1f);

    public List<string> ToastHistory = new List<string>();



    public void Toast(string text, bool persistent = false, bool urgent = false, float duration = 4f)
    {
        Debug.Log($"<color=cyan>Toast </color>{text}");
        GameObject newToast = Instantiate(ToastPrefab);
        newToast.GetComponent<ToastInstance>().isPersistent = persistent;

        newToast.transform.SetParent(gameObject.transform);
        // newToast.transform.SetAsFirstSibling();
        var tmpText = newToast.GetComponent<TextMeshProUGUI>();
        var color = urgent ? urgentColor : Color.white;

        string persistentHint = " (click to dismiss) ";
        string t;
        if (persistent) t = text + persistentHint;
        else t = text;
        tmpText.text = t;

        tmpText.color = color;

        ToastHistory.Add(text);
        AudioManager.PlaySound("toast");
        newToast.GetComponent<ToastInstance>().StartToastKill(duration);

    }


}
