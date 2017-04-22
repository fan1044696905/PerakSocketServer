using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//打开界面需要的数据模型
class UIModel
{
    public Type UIType;//UI界面对应的脚本类型
    public string Path;//界面对应的资源路径
    public object[] Params;//打开界面时所需要的参数
    public UIModel(Type uiType, string path, params object[] args)
    {
        UIType = uiType;
        Path = path;
        Params = args;
    }
}
public class UIManager:Singleton<UIManager>
{
    //打开的UI界面和打开后隐藏的界面
    private Dictionary<Type, BasePanel> dicOpenPanels = new Dictionary<Type, BasePanel>();
    // 当前需要打开界面对应的数据Model(要加载的界面放在栈里面)
    private Stack<UIModel> needOpenModels = new Stack<UIModel>();

    public override void Init()
    {
        CoroutineController.Instance.StartCoroutine(CheckDestoryPanel());
    }

    #region ----- UI Contanier -----
    private Transform uiContainer = null;
    /// <summary>
    /// UI Root GameObject
    /// 所有UI对象的根节点
    /// </summary>
    public Transform UIContainer
    {
        get
        {
            if (uiContainer==null)
            {
                GameObject rootUI = GameObject.FindGameObjectWithTag(GameDefine.rootUITag);
                if (rootUI==null)
                {
                    UnityEngine.Object prefab = Resources.Load("Prefabs/RootUI");
                    rootUI = GameObject.Instantiate(prefab) as  GameObject;
                    rootUI.name = "RootUI";
                }
                uiContainer = rootUI.transform.Find("Container");
            }
            return uiContainer;
        }
    }
    #endregion


    /// <summary>
    /// 打开指定界面,可有可无参数
    /// (如打开背包的某个标签页)
    /// </summary>
    /// <typeparam name="T">要打开的界面对应的脚本类型</typeparam>
    /// <param name="array">打开界面所需要的参数</param>
    public void OpenPanel<T>(params object[] args) where T : MonoBehaviour
    {
        Type[] uiTypes = new Type[]{typeof(T)};
        OpenPanel(uiTypes);
    }

    /// <summary>
    /// 打开一些界面
    /// </summary>
    /// <param name="uiTypes">界面数组</param>
    public void OpenPanel(Type[]uiTypes)
    {
        OpenPanel(false,uiTypes);
    }

    /// <summary>
    /// 打开指定界面，关闭其他可关的UI界面
    /// </summary>
    /// <typeparam name="T">要打开的界面对应的脚本类型</typeparam>
    /// <param name="array">可变数组</param>
    public void OpenPanelCloseOther<T>(params object[] args) where T : MonoBehaviour
    {
        Type [] uiTypes = new Type[]{typeof(T)};
        OpenPanel(true,uiTypes,args);
    }

    /// <summary>
    /// 打开一些UI界面，关掉其他可关的UI界面
    /// </summary>
    /// <param name="uitypes"></param>
    public void OpenPanelCloseOther(Type []uitypes)
    {
        OpenPanel(true,uitypes);
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    /// <param name="isCloseOther">是否关闭其他面板</param>
    /// <param name="uiTypes"></param>
    /// <param name="args"></param>
    private void OpenPanel(bool isCloseOther,Type[]uiTypes,params object[]args)
    {
        if (isCloseOther)
        {
            CloseOtherPanel();
        }
        int max = uiTypes.Length;
        for (int i = 0; i < max; i++)
        {
            Type _uitype = uiTypes[i];//需要打开的某一个UI
            BasePanel panel;
            //如果字典里面有且不是null，直接打开界面，如果不存在就Load界面Prefab 然后Instantiate出来
            if (dicOpenPanels.TryGetValue(_uitype, out panel) && panel != null)
            {
                panel.SetVisable(true);
            }
            //需要加载的
            else
            {
                //防止字典中存在为空再添加同样类型的panel
                dicOpenPanels.Remove(_uitype);
                string path = GameDefine.GetPath(_uitype);
                needOpenModels.Push(new UIModel(_uitype, path, args));
            }
        }

        if (needOpenModels.Count > 0)
        {
            CoroutineController.Instance.StartCoroutine(AsyncLoadData());
        }
    }

    IEnumerator<int> AsyncLoadData()
    {
        UIModel model = null;
        UnityEngine.Object prefab = null;
        GameObject uiObj = null;
        if (needOpenModels != null && needOpenModels.Count > 0)
        {
            do
            {
                model = needOpenModels.Pop();
                prefab = Resources.Load(model.Path);
                if (prefab != null)
                {
                    //5.5.0API
                    uiObj = GameObject.Instantiate(prefab,UIContainer,false) as GameObject;
                    uiObj.name = prefab.name;
                    //uiObj.transform.SetParent(UIContainer,false);
                    BasePanel panel = uiObj.GetComponent<BasePanel>();
                    if (panel == null)
                    {
                        panel = uiObj.AddComponent(model.UIType) as BasePanel;
                    }
                    if (panel != null)
                    {
                        if (panel.GetType().ToString() != uiObj.name)
                        {
                            GameObject.Destroy(uiObj.GetComponent<BasePanel>());
                            panel = uiObj.AddComponent(model.UIType) as BasePanel;
                        }

                        //调用界面打开方法参数传递方法
                        panel.SetUIWhenOpening(model.Params);
                        panel.SetVisable(true);
                    }
                    dicOpenPanels.Add(model.UIType, panel);
                }
                else
                {
                    Debug.LogError(prefab + " 预制体为空");
                    yield break;
                }

            } while (needOpenModels.Count > 0);
        }
        yield return 0;
    }

    #region ----- ShowPanel HidePanel -----

    public void ShowPanel<T>(BasePanel panel) where T : BasePanel
    {
        ShowPanel(typeof(T),panel);
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="uiType"></param>
    /// <param name="panel"></param>
    public void ShowPanel(Type uiType,BasePanel panel)
    {
        panel.SetVisable(true);
        if (dicOpenPanels.ContainsKey(uiType)==false)
        {
            dicOpenPanels.Add(uiType,panel);
        }
    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    /// <param name="uiType"></param>
    public void HidePanel(Type uiType)
    {
        ClosePanel(uiType,false);
    }

    /// <summary>
    /// 销毁延时销毁的界面
    /// </summary>
    /// <param name="uiType"></param>
    public void DestroyPanel(Type uiType)
    {
        BasePanel panel = dicOpenPanels.TryGetValueExtension(uiType);
        if (panel!=null)
        {
            GameObject.Destroy(panel.gameObject);
        }
        dicOpenPanels.Remove(uiType);
    }

    #endregion

    /// <summary>
    /// 关闭其他可关闭的界面
    /// </summary>
    private void CloseOtherPanel()
    {
        List<Type> keys = new List<Type>(dicOpenPanels.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            ClosePanel(keys[i]);
        }
    }
    #region ----- Close Panle API -----

    /// <summary>
    /// 关闭指定类型界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ClosePanel<T>() where T : BasePanel
    {
        ClosePanel(typeof(T),false);
    }

    public void ClosePanel(Type [] uiTypes)
    {
        int max = uiTypes.Length;
        for (int i = 0; i < max; i++)
        {
            ClosePanel(uiTypes[i]);
        }
    }

    /// <summary>
    /// 关闭界面，移除已经销毁的界面，如果需要忽视批处理就不关闭
    /// </summary>
    /// <param name="uiType"></param>
    /// <param name="isBatch">是否批处理</param>
    public void ClosePanel(Type uiType,bool isBatch = false)
    {
        BasePanel panel;
        if (dicOpenPanels.TryGetValue(uiType,out panel)&&panel!=null)
        {
            
            if (panel.IgnoreDontActive)
            {
                return;
            }
            if (isBatch && panel.IgnoreBatchClose)
                return;
            if (panel.IsShow)
                panel.SetVisable(false);
        }
        else
        {
            dicOpenPanels.Remove(uiType);
        }
    }

    #endregion

    /// <summary>
    /// 检查需要延时关闭的界面
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckDestoryPanel()
    {
        Debug.Log("开始检查");
        float checkTimes = 5f;
        while (true)
        {
            yield return  new WaitForSeconds(checkTimes);
            List<Type> removeList = new List<Type>();

            List<Type> dicKeyList = new List<Type>(dicOpenPanels.Keys);
            for (int i = 0; i < dicKeyList.Count; i++)
            {
                BasePanel panel = dicOpenPanels.TryGetValueExtension(dicKeyList[i]);
                if (panel == null)
                {
                    dicOpenPanels.Remove(dicKeyList[i]);
                    continue;
                }
                float timer = panel.ReduceDestroyTimer(checkTimes);
                if (timer <= 0)
                    removeList.Add(dicKeyList[i]);
            }
            //bug ? foreach遍历出现 out of sync 的错误
            //foreach (var item in dicOpenPanels)
            //{
            //    if (item.Value == null)
            //    {
            //        dicOpenPanels.Remove(item.Key);
            //        continue;
            //    }
            //    float timer = item.Value.ReduceDestroyTimer(checkTimes);
            //    if (timer <= 0)
            //        removeList.Add(item.Key);
            //}

            for (int i = 0; i < removeList.Count; i++)
            {
                DestroyPanel(removeList[i]);
            }
        }
    }
    
}
