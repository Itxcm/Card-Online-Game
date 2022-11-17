﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BottomArea : UIBase
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartAnimation();
    }

    /// <summary>
    /// 起始动画
    /// </summary>
    private void StartAnimation()
    {
        var endPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector3(0.0f, rectTransform.anchoredPosition.y - rectTransform.rect.height, 0.0f);
        DOTween.To(() => rectTransform.anchoredPosition, x => rectTransform.anchoredPosition = x, endPos, .4f);
    }
}