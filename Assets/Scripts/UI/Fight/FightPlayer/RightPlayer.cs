﻿using Protocol.Constant;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPlayer : BasePlayer
{
    public override void Awake()
    {
        base.Awake();
        timer = transform.Find("Operate/Timer");
        beanCount = transform.Find("UserInfo/BeanBox/Count").GetComponent<Text>();
        cardAmout = transform.Find("CardStack/Amount").GetComponent<Text>();
        Bind(UIEvent.Right_User_Render,
            UIEvent.Right_User_Leave,
            UIEvent.Dispatch_Card,
            UIEvent.Turn_GrabLandowner,
            UIEvent.GrabLandowner_Success,
            UIEvent.Send_Quick_Chat,
            UIEvent.Send_ZiDingYi_Chat,
            UIEvent.Send_Emoji_Chat,
            UIEvent.Turn_Deal,
            UIEvent.Deal_Card_Sucess
            );
    }

    public override void Execute(int eventCode, object message)
    {
        base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.Right_User_Render:
                RenderShow((UserDto)message);
                break;

            case UIEvent.Right_User_Leave:
                RenderHide();
                break;

            case UIEvent.Dispatch_Card:
                StartCountAnimation();
                break;

            case UIEvent.Turn_GrabLandowner:
                TurnDto turnDto = (TurnDto)message;
                if (turnDto.isFirst)
                {
                    if (turnDto.currentId == userDto.Id) StartGrabLandowner = Show_Timer;
                }
                else if (turnDto.currentId == userDto.Id) Show_DontGrabe();
                else if (turnDto.nextId == userDto.Id) Show_Timer();
                break;

            case UIEvent.GrabLandowner_Success:
                GrabDto grabDto = (GrabDto)message;
                HideOperate();
                Dispatch(AreaCode.UI, UIEvent.Show_TableCard, grabDto.TableCardList); // 显示底牌
                if (grabDto.Uid == userDto.Id) cardAmout.text = "20";
                break;

            case UIEvent.Turn_Deal:
                if ((int)message == userDto.Id) Show_Timer();
                break;

            case UIEvent.Deal_Card_Sucess:
                var dealDtos = (DealDto)message;
                HideOperate();
                if (dealDtos.Uid != userDto.Id) return;
                count -= dealDtos.SelectCardList.Count;
                cardAmout.text = count.ToString();
                break;

            default:
                break;
        }
    }
}