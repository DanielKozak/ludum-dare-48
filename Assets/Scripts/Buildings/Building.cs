using UnityEngine;
using DG.Tweening;
public abstract class Building : MonoBehaviour
{
    public IAction OnLeftClick;
    public IAction OnRightClick;

    public GameObject Selector;
    public bool isTruck = false;
    internal bool isSelected = false;
    private void OnMouseEnter()
    {
        InputManager.SetMouseOver(this);
        // Debug.Log($"OnMouseEnter {gameObject.name}");
    }
    private void OnMouseExit()
    {
        InputManager.SetMouseOver(null);
        // Debug.Log($"OnMouseExit {gameObject.name}");

    }

    public void Select()
    {
        isSelected = true;
        Selector.gameObject.SetActive(true);
        Selector.GetComponentInChildren<Shapes.Disc>().transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
    public void Deselect()
    {
        isSelected = false;
        Selector.GetComponentInChildren<Shapes.Disc>().transform.DOKill();
        Selector.gameObject.SetActive(false);
    }

    public abstract void ShowInfo();
}