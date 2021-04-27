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

    public List<ToastInstance> ToastHistory = new List<ToastInstance>();



    public void Toast(string text, bool persistent = false, bool urgent = false, float duration = 4f)
    {
        Debug.Log($"<color=cyan>Toast </color>{text}");
        GameObject newToast = Instantiate(ToastPrefab);
        var toastInstance = newToast.GetComponent<ToastInstance>();
        toastInstance.isPersistent = persistent;

        newToast.transform.SetParent(gameObject.transform);
        // newToast.transform.SetAsFirstSibling();
        var tmpText = toastInstance.text;
        var color = urgent ? urgentColor : Color.white;

        string persistentHint = " (click to dismiss)";
        string t;
        if (persistent) t = text + persistentHint;
        else t = text;
        tmpText.text = t;

        tmpText.color = color;

        if (persistent) ToastHistory.Add(toastInstance);
        if (urgent) AudioManager.PlaySound("toast_high");
        else AudioManager.PlaySound("toast_low");
        toastInstance.StartToastKill(duration);

    }


}
