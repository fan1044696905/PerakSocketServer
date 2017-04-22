using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDefine
{
    #region ----- Tag -----

    public const string rootUITag = "RootUI";
    public const string audioSourceTag = "AudioSource";

    #endregion

    #region ----- Path -----

    public const string ui_prefabs = "Prefabs/UI/";

    #endregion

    #region ----- SceneName -----

    public const string needLoadSceneName = "NextSceneName";

    #endregion


    private static Dictionary<Type,string> dicPaths = new Dictionary<Type, string>(); 
    public static string GetPath<T>() where T : BasePanel
    {
        return GetPath(typeof (T));
    }

    /// <summary>
    /// 通过类型获取路径
    /// </summary>
    /// <param name="uiType">类型</param>
    /// <returns></returns>
    public static string GetPath(Type uiType)
    {
        //string path;
        //if (dicPaths.TryGetValue(uiType,out path))
        //{
        //    return path;
        //}
        //path = ui_prefabs + uiType.Name;
        //dicPaths.Add(uiType,path);
        //return path;

        string path = dicPaths.TryGetValueExtension(uiType);
        if (path == null)
        {
            path = ui_prefabs + uiType.Name;
            dicPaths.Add(uiType, path);
        }
        return path;
    }

    static GameDefine() 
    {
        //dicPaths.Add(typeof(RegisterPanel),ui_prefabs+"Login/RegisterPanel");
    }
}
