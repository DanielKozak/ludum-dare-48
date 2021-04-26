using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BananaYoyo : MonoBehaviour
{
    void Start()
    {
        float pos = 0.3f;
        transform.DOLocalMoveY(0.7f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

}
