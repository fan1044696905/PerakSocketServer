using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private Transform thisTrans;
    private Button _loginButton;
    private Button _registerButton;
    //private Button _knapsackButton;
    //private Button _taskButton;
    void Awake()
    {
        SoundManager.Instance.PlaySound(E_PlayMusic.开始场景);
        IgnoreDontActive = true;
        thisTrans = transform;
        _loginButton = thisTrans.GetComponent<Button>("GameObject/Btns/LoginBtn");
        _registerButton = thisTrans.GetComponent<Button>("GameObject/Btns/RegisterBtn");
        StartCoroutine(OpenPanelOnAwake());
        _loginButton.AddEventListener(E_TouchType.OnClick,LoginButtonOnClick,null);
        _registerButton.AddEventListener(E_TouchType.OnClick, RegisterButtonOnClcik, null);
       // _taskButton.AddEventListener(E_TouchType.OnClick,TaskButtonOnClick,null);
        
    }
    private void RegisterButtonOnClcik(GameObject listener, object args, params object[] _params)
    {
        SoundManager.Instance.PlaySound(E_PlayMusic.普通按钮点击);
        UIManager.Instance.OpenPanel<RegisterPanel>();
    }
    private void LoginButtonOnClick(GameObject listener, object args, params object[] _params)
    {
        //UIManager.Instance.OpenPanelCloseOther<ServerPanel>();
    }
    private void TaskButtonOnClick(GameObject listener, object args, object[] _params)
    {
        
    }

    void Start()
    {

    }
    

    

    //Bug? 第一次加载界面卡顿
    //Bug? 暂时用下面的解决方案(不推荐)
    IEnumerator OpenPanelOnAwake() 
    {
        UIManager.Instance.OpenPanel<RegisterPanel>();
        //UIManager.Instance.OpenPanel<ServerPanel>();
        yield return new WaitForEndOfFrame();
        UIManager.Instance.HidePanel(typeof(RegisterPanel));
        //UIManager.Instance.HidePanel(typeof(ServerPanel));
    }

}
