using UnityEngine;
using DG.Tweening;
using TMPro;

public abstract class Building : MonoBehaviour
{

    public TMP_Text FirstResourceText;
    public TMP_Text SecondResourceText;
    public IAction OnLeftClick;
    public IAction OnRightClick;

    public GameObject Selector;

    public bool isGarage = false;
    public bool isShip = false;
    public bool isTemple = false;
    public bool isRefinery = false;
    public bool isExtractor = false;

    public AudioSource audioSource;


    public int price;
    public bool isTruck = false;
    internal bool isSelected = false;

    private void OnMouseEnter()
    {
        InputManager.SetMouseOver(this);
        Debug.Log($"OnMouseEnter {gameObject.name}");
    }
    private void OnMouseExit()
    {
        InputManager.SetMouseOver(null);
        Debug.Log($"OnMouseExit {gameObject.name}");

    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Select()
    {

        CameraControllerTopDown.Instance.SetCameraPos(transform.position);
        isSelected = true;
        if (isTruck) InputManager.SetKeyAction(KeyCode.Mouse1, new IActions.Action_OrderTruckMove((Truck)this));

        ShowInfo();
        if (Selector == null) return;
        Selector.GetComponentInChildren<Shapes.Disc>().transform.DOKill();

        Selector.gameObject.SetActive(true);
        // Selector.GetComponentInChildren<Shapes.Disc>().transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
        Selector.GetComponentInChildren<Shapes.Disc>().transform.DOLocalRotate(new Vector3(0, 0, 180f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);
    }
    public void Deselect()
    {
        isSelected = false;
        Debug.Log($"Deselect {name}");
        UIManager.Instance.HideInfoPanel();
        if (Selector == null) return;
        Selector.GetComponentInChildren<Shapes.Disc>().transform.DOKill();
        Selector.gameObject.SetActive(false);

    }

    public abstract void ShowInfo();
}