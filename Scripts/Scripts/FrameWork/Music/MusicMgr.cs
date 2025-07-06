using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManger<MusicMgr>
{
    #region �������������캯��
    //�������ֲ������
    private AudioSource bkMusic = null;
    private float bkMusicValue = 0.1f;
    private float soundValue = 0.1f;
    //������Ч��������Ķ���

    //�������ڲ��ŵ���Ч
    private List<AudioSource> soundList = new List<AudioSource>();
    //��Ч�Ƿ��ڲ���
    private bool soundIsPlay = true;
    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    /// <summary>
    /// �Ƴ����ڲ��ŵ���Ч
    /// </summary>
    private void Update()
    {
        if (!soundIsPlay)
            return;
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                soundList[i].clip = null;
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }
    #endregion
    #region �����������
    #region ���ű�������
    public void PlayBKMusic(string name)
    {
        //��̬������������������ڹ�����ʱ���Ƴ� ��������Ψһ
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }
        ABMgr.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        }, true);
    }
    #endregion
    #region ֹͣ��������
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }
    #endregion
    #region ��ͣ��������
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }
    #endregion
    #region ���ñ������ִ�С
    public void ChangeBKMusic(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = v;
    }
    #endregion
    #endregion
    #region ��Ч���
    #region ������Ч
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="name">��Ч��Ƭ�ļ���</param>
    /// <param name="isAsync">�Ƿ��첽����</param>
    /// <param name="isLoop">��Ч�Ƿ�ѭ��</param>
    /// <param name="callBack">���ؽ�����Ļص�</param>
    public void PlaySound(string name, bool isAsync = false, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {  
        ABMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            AudioSource source =PoolMgr.Instance.GetObj("Sound/SoundObj").GetComponent<AudioSource>();
            source.Stop();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //�洢��Ч������ ���ڼ�¼ �����ж��Ƿ�ֹͣ
            if(!soundList.Contains(source))
            soundList.Add(source);
            //���ݸ��ⲿʹ��
            callBack?.Invoke(source);
        }, isAsync);
    }
    #endregion
    #region ֹͣ����ָ����Ч
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
           source.clip=null;
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }
    #endregion
    #region �ı���Ч�Ĵ�С
    public void ChangeSoundValeue(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = v;
        }
    }
    #endregion
    #region ���Ż���ͣһ����Ч
    /// <summary>
    /// �������Ż�����ͣ������Ч
    /// </summary>
    /// <param name="isPlay"></param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if (isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Play();

            }
        }
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Pause();
            }
        }
    }
    /// <summary>
    /// �����Ч��ؼ�¼ ������ʱ������ջ����֮ǰȥ������
    /// </summary>
    public void ClearSound()
    {
        for (int i=0;i<soundList.Count;i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        soundList.Clear();
    }
    #endregion
    #endregion
}
