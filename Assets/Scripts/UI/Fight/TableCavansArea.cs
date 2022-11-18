﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq.Expressions;

public class TableCavansArea : UIBase
{
    public List<Sprite> imageList = new List<Sprite>(); // 0-9 的图片资源 非动态载入 不清除

    private GameObject MatchTips; // 匹配提示
    private GameObject StartBtn; // 开始按钮

    private Button mingPaiStart; // 明牌开始
    private Button defaultStart; // 默认开始
    private Button cancleMatch; // 取消匹配

    private Image shiWei; // 匹配时间十位
    private Image geWei;  // 匹配时间个位
    private int startTime = 30; // 起始计时

    private Queue<RectTransform> rectQueue = new Queue<RectTransform>(); // 动画 rectTrans队列
    private int sequenceIndex = 1; // 系列动画索引

    private void Awake()
    {
        StartBtn = transform.Find("StartBtn").gameObject;

        mingPaiStart = StartBtn.transform.Find("MingPaiStart").GetComponent<Button>();
        defaultStart = StartBtn.transform.Find("DefaultStart").GetComponent<Button>();
        mingPaiStart.onClick.AddListener(OnClickMingPaiStart);
        defaultStart.onClick.AddListener(OnClickDefaultStart);

        MatchTips = transform.Find("MatchTips").gameObject;
        cancleMatch = MatchTips.transform.Find("Cancle").GetComponent<Button>();
        cancleMatch.onClick.AddListener(OnClickCancleMatch);

        shiWei = MatchTips.transform.Find("Timer/ShiWei").GetComponent<Image>();
        geWei = MatchTips.transform.Find("Timer/Gewei").GetComponent<Image>();
    }

    private void Start()
    {
        SetMathchTipsActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        RemoveAllBtnListen();
        CancleAllInvoke();
        ClearAllCollision();

        DOTween.KillAll();
    }

    /// <summary>
    /// 明牌开始
    /// </summary>
    private void OnClickMingPaiStart()
    {
        SetMathchTipsActive(true);
        SetStartBtnActive(false);

        StartAnimation();
    }

    /// <summary>
    /// 默认开始
    /// </summary>
    private void OnClickDefaultStart()
    {
        SetMathchTipsActive(true);
        SetStartBtnActive(false);

        StartAnimation();
    }

    /// <summary>
    /// 取消匹配
    /// </summary>
    private void OnClickCancleMatch()
    {
        SetMathchTipsActive(false);
        SetStartBtnActive(true);

        ResetAnimaton();
    }

    /// <summary>
    /// 重置动画
    /// </summary>
    private void ResetAnimaton()
    {
        CancleAllInvoke();
        ClearAllCollision();
        startTime = 30;
        sequenceIndex = 1;
        DOTween.KillAll();
    }

    /// <summary>
    /// 开始动画
    /// </summary>

    private void StartAnimation()
    {
        StartTimer();
        var parent = MatchTips.transform.Find("AnimationTips");
        for (var i = 0; i < parent.childCount; i++)
        {
            var rectTrans = parent.GetChild(i).GetComponent<RectTransform>();
            rectQueue.Enqueue(rectTrans);
            if (i == parent.childCount - 1) AnimationForVertical();
        }
    }

    /// <summary>
    /// 匹配动画方法 垂直序列动画
    /// </summary>
    /// <param name="rectTrans"></param>
    public void AnimationForVertical()
    {
        var rectTrans = rectQueue.Dequeue();

        StartSequenceAnimation(rectTrans);

        rectQueue.Enqueue(rectTrans);

        if (sequenceIndex == 9)
        {
            sequenceIndex = 1;
            Invoke(nameof(AnimationForVertical), .5f);
        }
        else
        {
            sequenceIndex++;
            Invoke(nameof(AnimationForVertical), .1f);
        }
    }

    /// <summary>
    ///  序列动画方法
    /// </summary>
    /// <param name="rectTrans"></param>

    private void StartSequenceAnimation(RectTransform rectTrans)
    {
        Sequence sequence = DOTween.Sequence();
        var startPos = rectTrans.anchoredPosition;
        var endPos = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y + 12.0f);
        var t1 = DOTween.To(() => rectTrans.anchoredPosition, x => rectTrans.anchoredPosition = x, endPos, .2f);
        var t2 = DOTween.To(() => rectTrans.anchoredPosition, x => rectTrans.anchoredPosition = x, startPos, .2f);
        sequence
            .Append(t1)
            .Append(t2);
        sequence.SetEase(Ease.Flash);
        sequence.onKill = () =>
        {
            rectTrans.anchoredPosition = startPos;
        }; // 被杀了就重置位置
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    private void StartTimer()
    {
        if (startTime <= 0) startTime = 30;
        SwitchCase(startTime % 10, geWei);  // 个位
        SwitchCase((startTime % 100) / 10, shiWei); // 十位
        startTime -= 1;
        Invoke(nameof(StartTimer), 1.0f);
    }

    /// <summary>
    /// 计时动画条件
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="targerImage"></param>
    private void SwitchCase(int condition, Image targerImage)
    {
        switch (condition)
        {
            case 0:
                targerImage.overrideSprite = imageList[0];
                break;

            case 1:
                targerImage.overrideSprite = imageList[1];
                break;

            case 2:
                targerImage.overrideSprite = imageList[2];
                break;

            case 3:
                targerImage.overrideSprite = imageList[3];
                break;

            case 4:
                targerImage.overrideSprite = imageList[4];
                break;

            case 5:
                targerImage.overrideSprite = imageList[5];
                break;

            case 6:
                targerImage.overrideSprite = imageList[6];
                break;

            case 7:
                targerImage.overrideSprite = imageList[7];
                break;

            case 8:
                targerImage.overrideSprite = imageList[8];
                break;

            case 9:
                targerImage.overrideSprite = imageList[9];
                break;

            default:
                break;
        }
        targerImage.SetNativeSize();
    }

    /// <summary>
    /// 设置匹配提示显示
    /// </summary>
    /// <param name="value"></param>
    private void SetMathchTipsActive(bool value) => MatchTips.SetActive(value);

    /// <summary>
    /// 设置起始按钮显示
    /// </summary>
    /// <param name="value"></param>
    private void SetStartBtnActive(bool value) => StartBtn.SetActive(value);

    /// <summary>
    /// 移除所有按钮监听
    /// </summary>
    private void RemoveAllBtnListen()
    {
        mingPaiStart.onClick.RemoveAllListeners();
        defaultStart.onClick.RemoveAllListeners();
        cancleMatch.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 取消所有异步事件
    /// </summary>
    private void CancleAllInvoke()
    {
        CancelInvoke(nameof(AnimationForVertical));
        CancelInvoke(nameof(StartTimer));
    }

    /// <summary>
    /// 清除所有集合
    /// </summary>
    private void ClearAllCollision()
    {
        rectQueue.Clear();
    }
}