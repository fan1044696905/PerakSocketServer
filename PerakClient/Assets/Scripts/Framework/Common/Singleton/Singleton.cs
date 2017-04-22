
using System;
using UnityEngine;


public abstract class Singleton<T> where T : class, new()
{
     
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XHEngine.Singleton`1"/> class.
    /// </summary>
    protected Singleton()
    {
        if (null != _instance)
            throw new SingletonException("This " + (typeof(T)).ToString() + " Singleton Instance is not null !!!");
        Init();
    }


    /// <summary></summary>
    /// Init this Singleton.
    /// </summary>
    public virtual void Init() { }
}

	

