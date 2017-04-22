using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RegisterPanel : BasePanel
{

    private Transform _thisTrans;
    private Transform _closeBtnTrans;
    void Awake()
    {
        _thisTrans = transform;
        //_closeBtnTrans = _thisTrans.Find("Close");
        _TweenType = E_TweenType.LocalMoveAndScaleAndRotate;
        transform.localPosition = new Vector3(-1000, 0, 0);
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        //UIEventTriggerListener.Get(closeBtnTrans).onClick += CloseButtonOnClick;
    }

    public void RegisterButtonOnClick()
    {
        

        //UIManager.Instance.OpenPanelCloseOther<LoginPanel>();
    }
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    UIManager.Instance.HidePanel(typeof(RegisterPanel));
        //}
    }


    public void CloseButtonOnClick()
    {
        
        UIManager.Instance.HidePanel(typeof(RegisterPanel));
    }

}
