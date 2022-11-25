﻿using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : UIBase
{
    private Button registBtn;
    private InputField usernameInput;
    private InputField passwordInput;
    private InputField passwordRepeatInput;
    private PromptMsg promptMsg = new PromptMsg();
    private SocketMsg socketMsg = new SocketMsg();

    private void Awake()
    {
        registBtn = transform.Find("Account/RegistBtn").GetComponent<Button>();
        usernameInput = transform.Find("Account/Username/Input").GetComponent<InputField>();
        passwordInput = transform.Find("Account/Password/Input").GetComponent<InputField>();
        passwordRepeatInput = transform.Find("Account/AdfirmPassword/Input").GetComponent<InputField>();
    }

    private void Start()
    {
        registBtn.onClick.AddListener(OnClickRegist);

        Bind(UIEvent.Regist_Panel_Active);
        SetPanelActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        registBtn.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.Regist_Panel_Active:
                SetPanelActive((bool)message);
                break;

            default:
                break;
        }
    }

    private void OnClickClose()
    {
        SetPanelActive(false);
    }

    private void OnClickRegist()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            promptMsg.Change("账号不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.Prompt_Msg, promptMsg);
            return;
        }
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            promptMsg.Change("密码不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.Prompt_Msg, promptMsg);
            return;
        }
        if (string.IsNullOrEmpty(passwordRepeatInput.text))
        {
            promptMsg.Change("重复不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.Prompt_Msg, promptMsg);
            return;
        }

        if (passwordRepeatInput.text != passwordInput.text)
        {
            promptMsg.Change("输入的密码不一致!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.Prompt_Msg, promptMsg);
            return;
        }

        AccountDto accountDto = new AccountDto
        {
            Account = usernameInput.text,
            Password = passwordInput.text
        };
        socketMsg.Change(OpCode.Account, AccountCode.Regist_Cres, accountDto);
        Dispatch(AreaCode.NET, 0, socketMsg);
    }
}