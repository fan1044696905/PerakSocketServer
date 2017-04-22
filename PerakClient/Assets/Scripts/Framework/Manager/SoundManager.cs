using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum E_PlayMusic
{
    None,
    普通按钮点击,
    开始场景,
    //TODO 》》》》》》》
    //TODO 》》》》》》》
    //TODO 》》》》》》》
}

public class SoundManager:DDOLSingleton<SoundManager>
{
    private AudioSource _audioSource;

    private readonly Dictionary<string,AudioClip> _audioClipDict = new Dictionary<string, AudioClip>(); 
    /// <summary>
    /// 播放音乐(音效)
    /// </summary>
    /// <param name="ac">要播放的音乐</param>
    private void PlaySound(AudioClip ac)
    {
        AudioSource.PlayClipAtPoint(ac,Camera.main.transform.position);
    }

    /// <summary>
    /// 通过路径播放音乐(音效)
    /// </summary>
    /// <param name="soundPath">Resource中的路径</param>
    private void PlaySoundByPath(string soundPath)
    {
        AudioClip ac = _audioClipDict.TryGetValueExtension(soundPath);
        if (ac == null)
        {
            ac = Resources.Load<AudioClip>(soundPath);
            _audioClipDict.Add(soundPath, ac);
        }
        PlaySound(ac);
    }

    /// <summary>
    /// 通过路径播放背景音乐
    /// </summary>
    /// <param name="bgmPath"></param>
    /// <param name="volume"></param>
    private void PlaybgmByPath(string bgmPath,float volume = 0.5f)
    {
        AudioClip ac = _audioClipDict.TryGetValueExtension(bgmPath);
        if (ac == null)
        {
            ac = Resources.Load<AudioClip>(bgmPath);
            _audioClipDict.Add(bgmPath,ac);
        }
        Playbgm(ac,volume);
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="ac"></param>
    /// <param name="volume"></param>
    private void Playbgm(AudioClip ac,float volume)
    {
        if (_audioSource == null)
        {
            GameObject go = new GameObject(GameDefine.audioSourceTag);
            go.tag = GameDefine.audioSourceTag;
            _audioSource = go.GetOrAddComponent<AudioSource>();
            DontDestroyOnLoad(go);
        }
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
        _audioSource.clip = ac;
        _audioSource.volume = volume;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    /// <summary>
    /// 加载场景的时候关闭
    /// </summary>
    public void StopBgmOnLoad()
    {
        _audioSource.Stop();
    }

    /// <summary>
    /// 播放音乐/音效
    /// </summary>
    /// <param name="e_PlayMusic"></param>
    public void PlaySound(E_PlayMusic e_PlayMusic)
    {
        switch (e_PlayMusic)
        {
            case E_PlayMusic.普通按钮点击:
                PlaySoundByPath("Sound/UI/普通按钮点击");
                break;
            case E_PlayMusic.开始场景:
                PlaybgmByPath("Sound/Bg/大闹天宫");
                break;
        }
    }

    public void Play()
    {

    }
}
