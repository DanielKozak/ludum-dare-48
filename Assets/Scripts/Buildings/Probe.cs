using UnityEngine;
using DG.Tweening;

public class Probe : Building
{
    public void Drop((int x, int y) dropPosition)
    {
        Vector3 dropPosV3 = MapUtils.GetWorldPosByCell(dropPosition);
        Vector3 offsetDropPosV3 = MapUtils.GetWorldPosByCell(dropPosition);
        offsetDropPosV3.x -= 20f;
        offsetDropPosV3.y += 50f;
        offsetDropPosV3.z = -24f;

        transform.position = offsetDropPosV3;
        AudioManager.PlaySound("dropPod");
        GetComponentInChildren<SpriteRenderer>().transform.DOShakePosition(2f, 1f, 30);

        transform.DOMoveX(dropPosV3.x, 2f);
        transform.DOMoveY(dropPosV3.y, 2f);

        AudioManager.PlaySound("ping");

        ToastController.Instance.Toast("Dropped probe");
        //TODO  EFFECT

        //TODO REVEAL BANANAS

        //TODO Place repulsor

        Destroy(gameObject, 3f);
    }
    // public override void Select()
    // {
    //     throw new System.NotImplementedException();
    // }

    // public override void Deselect()
    // {
    //     throw new System.NotImplementedException();
    // }
    public override void ShowInfo()
    {

    }
}
