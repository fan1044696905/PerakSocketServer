using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneControlManager:Singleton<SceneControlManager> {

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="isHaveLoadingPanel">是否需要Loading界面</param>
    /// <param name="needLoadSceneName">要加载的场景名字</param>
    public void LoadScene(bool isHaveLoadingPanel, string needLoadSceneName)
    {
        SoundManager.Instance.StopBgmOnLoad();
        if (isHaveLoadingPanel)
        {
            PlayerPrefs.SetString(GameDefine.needLoadSceneName, needLoadSceneName);
            //UIManager.Instance.OpenPanelCloseOther<LoadingPanel>();
        }
        else
        {
            SceneManager.LoadSceneAsync(needLoadSceneName);
        }
    }
}
