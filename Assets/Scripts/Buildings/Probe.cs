using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Probe : Building
{
    int revealRadius = 10;
    public Shapes.Disc revealer;

    (int x, int y) position;
    Color firstColor = new Color(1, 0.7f, 0, 0.5f);
    Color secondColor = new Color(1, 0.7f, 0, 0f);
    public void Drop((int x, int y) dropPosition)
    {
        position = dropPosition;
        revealer.Radius = 0;
        revealer.Thickness = 0;
        revealer.ColorOuter = firstColor;
        Vector3 dropPosV3 = MapUtils.GetWorldPosByCell(dropPosition);
        Vector3 offsetDropPosV3 = MapUtils.GetWorldPosByCell(dropPosition);
        offsetDropPosV3.x -= 20f;
        offsetDropPosV3.y += 50f;
        offsetDropPosV3.z = -24f;

        GameManager.Instance.SetBananaBalance(-price);

        transform.position = offsetDropPosV3;
        AudioManager.PlaySoundLocal(audioSource, "drop");
        GetComponentInChildren<SpriteRenderer>().transform.DOShakePosition(2f, 2f, 30).OnComplete(() => StartCoroutine(RevealRoutine()));

        transform.DOMoveX(dropPosV3.x, 2f);
        transform.DOMoveY(dropPosV3.y, 2f).OnComplete(() => { AudioManager.PlaySound("ping"); });



        ToastController.Instance.Toast("Dropped probe");
        DOTween.To(() => revealer.Radius, x => revealer.Radius = x, revealRadius, 2f).SetDelay(2f);// = dir;
        DOTween.To(() => revealer.Thickness, x => revealer.Thickness = x, revealRadius * 0.5f, 2f).SetDelay(2f);// = dir;
        DOTween.To(() => revealer.Thickness, x => revealer.Thickness = x, revealRadius * 0.5f, 2f).SetDelay(2f);// = dir;
        DOTween.To(() => revealer.ColorOuter, x => revealer.ColorOuter = x, secondColor, 2f).SetDelay(2f);// = dir;
        StartCoroutine(RevealRoutine());

        //TODO Place repulsor

    }

    IEnumerator RevealRoutine()
    {
        bool revealing = true;
        List<(int, int)> revealed = new List<(int, int)>();
        yield return null;
        Vector3 mv3Pos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        Vector3 cv3Pos = new Vector3();
        while (revealing)
        {
            int currentDist = Mathf.FloorToInt(revealer.Radius);
            for (var i = position.x - currentDist; i <= position.x + currentDist; i++)
                for (var j = position.y - currentDist; j <= position.y + currentDist; j++)
                {
                    cv3Pos.x = i;
                    cv3Pos.y = j;
                    cv3Pos.z = 0;
                    if (Vector3.Distance(mv3Pos, cv3Pos) < currentDist)
                    {
                        if (revealed.Contains((i, j))) continue;
                        if (MapController.Instance.BananaMap.GetValue(i, j) > 0)
                        {
                            MapController.Instance.RevealBanana(i, j);
                            revealed.Add((i, j));

                        }
                    }

                }
            if (currentDist >= revealer.Radius)
            {
                revealing = false;
            }
            yield return null;
        }






        Destroy(gameObject, 4f);
    }

    public override void ShowInfo()
    {

    }
}
