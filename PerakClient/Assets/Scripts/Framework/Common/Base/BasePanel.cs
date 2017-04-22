using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 动画类型
/// </summary>
public enum E_TweenType
{
    /// <summary>
    /// 无动画
    /// </summary>
    None,
    /// <summary>
    /// 世界坐标偏移动画
    /// </summary>
    Move,
    /// <summary>
    /// 局部坐标偏移动画
    /// </summary>
    LocalMove,
    /// <summary>
    /// 缩放动画
    /// </summary>
    Scale,
    /// <summary>
    /// 旋转动画
    /// </summary>
    Rotate,
    /// <summary>
    /// 局部坐标和缩放
    /// </summary>
    LocalMoveAndScale,
    /// <summary>
    /// 局部坐标和旋转
    /// </summary>
    LocalMoveAndRotate,
    /// <summary>
    /// 缩放和旋转
    /// </summary>
    ScaleAndRotate,
    /// <summary>
    /// 位移缩放和旋转
    /// </summary>
    LocalMoveAndScaleAndRotate
}

public abstract class BasePanel : MonoBehaviour
{

    #region ----- DOTween -----

    protected Tweener moveTweener;
    protected Tweener localMoveTweener;
    protected Tweener scaleTweener;
    protected Tweener rotateTweener;

    /// <summary>
    /// 在构造函数中重写动画类型  不需要动画则无须重写
    /// </summary>
    protected E_TweenType _TweenType = E_TweenType.None;
    protected Vector3 moveEndV3 = Vector3.zero;
    protected Vector3 localMoveEndV3 = Vector3.zero;
    protected Vector3 scaleEndV3 = Vector3.one;
    protected Vector3 rotateEndV3 = new Vector3(0,0,-720);
    protected RotateMode e_RotateMode = RotateMode.LocalAxisAdd;
    protected Ease e_Ease = Ease.Linear;
    protected float duration = 0.3f;
    //private bool isNeedPlayForward = true;
    #endregion

    /// <summary>
    /// 是否立即被销毁
    /// </summary>
    [NonSerialized]
    public bool IsDestroy = false;
    /// <summary>
    /// 忽视批量关闭操作，CloseOtherPanel时该界面不会被关闭(需要在Awake或者OnEnable中设置为True)
    /// </summary>
    [NonSerialized]
    public bool IgnoreBatchClose = false;
    /// <summary>
    /// 忽视隐藏界面后该界面处于不可见状态，当该值设置为True后，界面将一直处于可见状态
    /// </summary>
    [NonSerialized] public bool IgnoreDontActive = false;
    /// <summary>
    /// 界面实际是否显示
    /// </summary>
    public bool IsShow
    {
        get
        {
            return gameObject.activeSelf; 
            
        }
    }

    #region ----- 延时销毁对象 -----

    /// <summary>
    /// 延时销毁事件，子类需要指定
    /// </summary>
    protected float DestroyDelay = 10f;
    protected float DestroyTimer;//需要在SetVisable(bool isShow)方法中重置

    /// <summary>
    /// 减少销毁计时器时间(UIManager调用)
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public float ReduceDestroyTimer(float deltaTime)
    {
        if (IgnoreBatchClose || IsShow)
        {
            return 1f;
        }
        DestroyTimer -= deltaTime;
        return DestroyTimer;
    }

    #endregion

    /// <summary>
    /// 如果有参数传递进来重写该方法接受参数
    /// </summary>
    /// <param name="args"></param>
    public virtual void SetParams(params object[] args)
    {

    }

    public virtual IEnumerator OnAsyncLoad()
    {
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// UIManager调用，其他地方不会调用该方法
    /// 打开UI需要传参数的时候调用
    /// </summary>
    /// <param name="args"></param>
    public void SetUIWhenOpening(params object[]args)
    {
        SetParams(args);
        StartCoroutine(OnAsyncLoad());
    }


    /// <summary>
    /// 物体禁用是调用，重写不要覆盖
    /// </summary>
    protected virtual void OnDisable()
    {
        //停止当前界面启动的所有协程
        StopAllCoroutines();
    }

    /// <summary>
    /// 设置等到最顶层
    /// </summary>
    public void SetTransToTop()
    {
        transform.SetAsFirstSibling();
    }

    /// <summary>
    /// 设置到最底层(一般只有ItemTooltip用到)
    /// </summary>
    public void SetTransToBottom()
    {
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 实际打开关闭界面代码
    /// </summary>
    /// <param name="isShow"></param>
    public void SetVisable(bool isShow)
    {
        OnPlaySound(isShow);
        //重置销毁计时器
        DestroyTimer = DestroyDelay;

        if (isShow == true)
        {
            gameObject.SetActive(isShow);
            // SetTransToTop();
        }

        //TODO>>>>>>>>>>>>>>>>启动 开始动画
        OnStartAnimation(isShow);
    }

    /// <summary>
    /// 打开关闭界面声音
    /// </summary>
    /// <param name="isShow"></param>
    protected virtual void OnPlaySound(bool isShow)
    {

    }

    /// <summary>
    /// 打开关闭界面动画开始播放
    /// 重写是覆盖基类方法功能，自己手动添加结束回调
    /// </summary>
    /// <param name="isShow"></param>
    protected virtual void OnStartAnimation(bool isShow)
    {
        //TODO 播放动画
        #region ----- 判断当前动画类型 -----
        switch (_TweenType)
        {
            case E_TweenType.None:

                //TODO 不播放任何动画》》》》》》》》》》》》
                if (isShow == false)
                {
                    gameObject.SetActive(false);
                }

                break;

            #region ----- 世界偏移 -----

            case E_TweenType.Move:

                if (moveTweener == null)
                {
                    moveTweener = transform.DOMove(moveEndV3, duration);
                    moveTweener.SetAutoKill(false);
                    moveTweener.Pause();
                }
                if (isShow)
                    moveTweener.PlayForward();
                else
                    moveTweener.PlayBackwards();
                    moveTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 局部偏移 -----

            case E_TweenType.LocalMove:

                if (localMoveTweener == null)
                {
                    localMoveTweener = transform.DOLocalMove(localMoveEndV3, duration);
                    localMoveTweener.SetAutoKill(false);
                    localMoveTweener.Pause();
                }
                if (isShow)
                    localMoveTweener.PlayForward();
                else
                    localMoveTweener.PlayBackwards();
                    localMoveTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 缩放 -----

            case E_TweenType.Scale:
                if (scaleTweener == null)
                {
                    scaleTweener = transform.DOScale(scaleEndV3, duration);
                    scaleTweener.SetAutoKill(false);
                    scaleTweener.Pause();
                }
                if (isShow)
                    scaleTweener.PlayForward();
                else
                    scaleTweener.PlayBackwards();
                    scaleTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 旋转 -----

            case E_TweenType.Rotate:

                if (rotateTweener == null)
                {
                    rotateTweener = transform.DOLocalRotate(rotateEndV3, duration, e_RotateMode).SetEase(e_Ease);
                    rotateTweener.SetAutoKill(false);
                    rotateTweener.Pause();
                }
                if (isShow)
                    rotateTweener.PlayForward();
                else
                    rotateTweener.PlayBackwards();
                    rotateTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 局部偏移 缩放 -----

            case E_TweenType.LocalMoveAndScale:
                if (localMoveTweener == null)
                {
                    localMoveTweener = transform.DOLocalMove(localMoveEndV3, duration);
                    localMoveTweener.SetAutoKill(false);
                    localMoveTweener.Pause();
                }
                if (scaleTweener == null)
                {
                    scaleTweener = transform.DOScale(scaleEndV3, duration);
                    scaleTweener.SetAutoKill(false);
                    scaleTweener.Pause();
                }
                
                if (isShow)
                {
                    localMoveTweener.PlayForward();
                    scaleTweener.PlayForward();
                }
                else
                {
                    localMoveTweener.PlayBackwards();
                    scaleTweener.PlayBackwards();
                }
                localMoveTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;
            #endregion

            #region ----- 局部偏移 旋转 -----

            case E_TweenType.LocalMoveAndRotate:

                if (localMoveTweener == null)
                {
                    localMoveTweener = transform.DOLocalMove(localMoveEndV3, duration);
                    localMoveTweener.SetAutoKill(false);
                    localMoveTweener.Pause();
                }
                if (rotateTweener == null)
                {
                    rotateTweener =
                        transform.DOLocalRotate(rotateEndV3, duration, e_RotateMode).SetEase(e_Ease);
                    rotateTweener.SetAutoKill(false);
                    rotateTweener.Pause();
                }
                if (isShow)
                {
                    localMoveTweener.PlayForward();
                    rotateTweener.PlayForward();
                }
                else
                {
                    localMoveTweener.PlayBackwards();
                    rotateTweener.PlayBackwards();
                }
                localMoveTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 缩放 旋转 -----

            case E_TweenType.ScaleAndRotate:

                if (scaleTweener == null)
                {
                    scaleTweener = transform.DOScale(scaleEndV3, duration);
                    scaleTweener.SetAutoKill(false);
                    scaleTweener.Pause();
                }
                if (rotateTweener == null)
                {
                    rotateTweener =
                        transform.DOLocalRotate(rotateEndV3, duration, e_RotateMode).SetEase(e_Ease);
                    rotateTweener.SetAutoKill(false);
                    rotateTweener.Pause();
                }
                if (isShow)
                {
                    scaleTweener.PlayForward();
                    rotateTweener.PlayForward();
                }
                else
                {
                    scaleTweener.PlayBackwards();
                    rotateTweener.PlayBackwards();
                }
                scaleTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;

            #endregion

            #region ----- 局部偏移 缩放 旋转 -----
            case E_TweenType.LocalMoveAndScaleAndRotate:
                if (localMoveTweener == null)
                {
                    localMoveTweener = transform.DOLocalMove(localMoveEndV3, duration);
                    localMoveTweener.SetAutoKill(false);
                    localMoveTweener.Pause();
                }
                if (scaleTweener == null)
                {
                    scaleTweener = transform.DOScale(scaleEndV3, duration);
                    scaleTweener.SetAutoKill(false);
                    scaleTweener.Pause();
                }
                if (rotateTweener == null)
                {
                    rotateTweener =
                        transform.DOLocalRotate(rotateEndV3, duration, e_RotateMode).SetEase(e_Ease);
                    rotateTweener.SetAutoKill(false);
                    rotateTweener.Pause();
                }
                if (isShow)
                {
                    localMoveTweener.PlayForward();
                    scaleTweener.PlayForward();
                    rotateTweener.PlayForward();
                }
                else
                {
                    localMoveTweener.PlayBackwards();
                    scaleTweener.PlayBackwards();
                    rotateTweener.PlayBackwards();
                }
                localMoveTweener.OnStepComplete(delegate { TweenOnSetComplete(isShow); });
                break;
            #endregion
        }
        #endregion

    }

    /// <summary>
    /// 打开关闭界面的动画结束回调，需要在派生类中重写，重写不要覆盖基类
    /// </summary>
    /// <param name="isShow"></param>
    protected virtual void TweenOnSetComplete(bool isShow)
    {
        if (IgnoreDontActive)
        {
            return;
        }
        if (isShow == false)
        {
            //重置销毁计时器
            DestroyTimer = DestroyDelay;
            //隐藏界面 需要在重写的动画回调结束之后设置为隐藏
            gameObject.SetActive(isShow);
            //如果是立即销毁就立即销毁
            if (IsDestroy)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
