//*************************************
//
//  MonoBehavior扩展类  Get Or Add Component(获取或者添加组件) 
//
//*************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class MonoBehaviorExtend
{
    #region ------ GetOrAddComponent ------

    /// <summary>
    /// Get Or Add Component By Path
    /// 通过路径获取或者添加组件(可用于父对象通过路径获取子对象的组件)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go,string path = "") where T :Component
    {
        Transform t;
        //
        //路径为空表示操作自身
        if (string.IsNullOrEmpty(path))
        {
            t = go.transform;
        }
        //路径不为空表示操作子对象
        else
        {
            t = go.transform.Find(path);
        }
        if (t==null)
        {
            Debug.Log("传入路径有误，没有通过路径找到对象:"+ path+"    "+ t.name);
            return null;
        }
        T ret = t.gameObject.GetComponent<T>();
        if (ret==null)
        {
            ret = t.gameObject.AddComponent<T>();
        }
        return ret;
    }

    public static T GetOrAddComponent<T>(this Transform trans, string path = "") where T : Component
    {
        return trans.gameObject.GetOrAddComponent<T>(path);
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour trans, string path = "") where T : Component
    {
        return trans.gameObject.GetOrAddComponent<T>(path);
    }

    #endregion

    #region ------ GetComponent ------

    public static T GetComponent<T>(this Transform trans, string path) where T : Component
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        return trans.Find(path).GetComponent<T>();
    }

    public static T GetComponent<T>(this GameObject go, string path) where T : Component
    {
        return GetComponent<T>(go.transform,path);
    }

    public static T GetComponent<T>(this MonoBehaviour mono, string path) where T : Component
    {
        return GetComponent<T>(mono.transform,path);
    }

    #endregion
    

    public static TValue TryGetValueExtension<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        TValue value;
        dict.TryGetValue(key, out value);
        return value;
    }

    

    #region ---------- tranform gameobject button 添加事件 ----------
    public static EventListener AddEventListener(this Transform trans,E_TouchType touchType,OnTouchEvent onTouchEvent,params object [] args)
    {
        return trans.AddEventListener("",touchType,onTouchEvent,args);
    }

    public static EventListener AddEventListener(this Transform trans, string path, E_TouchType touchType,
        OnTouchEvent onTouchEvent, params object[] args)
    {
        if (string.IsNullOrEmpty(path))
        {
            trans = trans.Find(path);
        }
        EventListener listener = EventListener.Get(trans);
        listener.AddEventListener(touchType,onTouchEvent,args);
        return listener;
    }

    public static EventListener AddEventListener(this GameObject go, E_TouchType touchType, OnTouchEvent onTouchEvent, params object[] args)
    {
        return go.AddEventListener("", touchType, onTouchEvent, args);
    }

    public static EventListener AddEventListener(this GameObject go, string path, E_TouchType touchType,
        OnTouchEvent onTouchEvent, params object[] args)
    {
        if (string.IsNullOrEmpty(path))
        {
            go = go.transform.Find(path).gameObject;
        }
        EventListener listener = EventListener.Get(go);
        listener.AddEventListener(touchType, onTouchEvent, args);
        return listener;
    }
    public static EventListener AddEventListener(this Button btn, E_TouchType touchType, OnTouchEvent onTouchEvent, params object[] args)
    {
        return btn.AddEventListener("", touchType, onTouchEvent, args);
    }

    public static EventListener AddEventListener(this Button btn, string path, E_TouchType touchType,
        OnTouchEvent onTouchEvent, params object[] args)
    {
        if (string.IsNullOrEmpty(path))
        {
            btn = btn.transform.Find(path).GetOrAddComponent<Button>();
        }
        EventListener listener = EventListener.Get(btn);
        listener.AddEventListener(touchType, onTouchEvent, args);
        return listener;
    }
    #endregion


}
