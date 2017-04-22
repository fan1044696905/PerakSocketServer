using System;
using UnityEngine;
using System.Collections;
/*
    启动加载 全局类
 */
public class StartGame : MonoBehaviour {
   
    
    void Awake()
    {
        Application.targetFrameRate = 45;
        //UIManager.Instance.OpenPanel<LoginPanel>();
    }

    void Start ()
	{
	}
    

}
