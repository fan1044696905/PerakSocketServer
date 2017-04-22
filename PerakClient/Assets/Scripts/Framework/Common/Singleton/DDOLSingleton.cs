// /*************************************************************************


// **************************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// DDOL singleton. Dont Destroy On Load
/// </summary>
public abstract class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{
	protected static T _Instance = null;
	public static T Instance {
		get {
			if (null == _Instance) {
				GameObject go = GameObject.Find ("DDOLGameManager");
                if (null == go)
                {
                    go = new GameObject("DDOLGameManager");//创建一个空物体并声明为不可销毁的对象
                }
                DontDestroyOnLoad(go);
                _Instance = go.GetOrAddComponent<T>();
			}
			return _Instance;
		}
	}

	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	private void OnApplicationQuit ()
	{
		_Instance = null;
	}
}
	