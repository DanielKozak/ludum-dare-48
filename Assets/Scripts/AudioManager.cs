using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    static List<AudioClip> SFXList;
    static List<AudioClip> MusicList;

    static AudioSource SFXChannel;
    static AudioSource SFX2Channel;
    static AudioSource SFX3Channel;
    static AudioSource MusicChannel;
    static AudioSource AmbientChannel;


    public void Init()
    {

        SFXChannel = GameObject.Find("SFXChannel").GetComponent<AudioSource>();
        SFX2Channel = GameObject.Find("SFX2Channel").GetComponent<AudioSource>();
        SFX3Channel = GameObject.Find("SFX3Channel").GetComponent<AudioSource>();
        MusicChannel = GameObject.Find("MusicChannel").GetComponent<AudioSource>();
        AmbientChannel = GameObject.Find("AmbientChannel").GetComponent<AudioSource>();

        SFXList = new List<AudioClip>();
        MusicList = new List<AudioClip>();

        SFXList.AddRange(Resources.LoadAll<AudioClip>("Audio/SFX"));
        MusicList.AddRange(Resources.LoadAll<AudioClip>("Audio/Music"));

        Debug.Log($"<color=yellow>AudioManager-> Init(); Loaded {SFXList.Count} sounds and {MusicList.Count} music tracks</color>");
        foreach (var item in SFXList)
        {
            Debug.Log($"<color=yellow>AudioManager-> SFX: {item.name}</color>");
        }

        // int musicId = Random.Range(0, MusicList.Count);
        // MusicChannel.clip = MusicList[musicId];
        // ToggleMusic();
    }

    public static void PlaySoundLocal(AudioSource source, string clipName)
    {
        source.loop = false;
        if (isMuted) return;
        if (source.isPlaying)
            source.Stop();
        foreach (var clip in SFXList)
        {
            if (clip.name == clipName)
            {
                source.clip = clip;
                source.Play();
                return;
            }
            Debug.Log($"AudioManager no clip {clipName}");
        }
    }
    public static void PlaySound(string clipName)
    {
        if (isMuted) return;

        AudioSource freeSource;
        if (SFXChannel.isPlaying)
        {
            if (SFX2Channel.isPlaying)
            {
                if (SFX3Channel.isPlaying)
                {
                    SFXChannel.Stop();
                    freeSource = SFXChannel;
                }
                else
                {
                    freeSource = SFX3Channel;
                }
            }
            else
            {
                freeSource = SFX2Channel;
            }
        }
        else
        {
            freeSource = SFXChannel;

        }

        foreach (var clip in SFXList)
        {
            if (clip.name == clipName)
            {
                freeSource.clip = clip;
                freeSource.Play();
                return;
            }
        }
    }
    public static void PlaySoundDelayed(string clipName, float delay)
    {
        if (isMuted) return;

        Instance.StartCoroutine(DelayedSfxRoutine(clipName, delay));
    }

    static IEnumerator DelayedSfxRoutine(string clipname, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlaySound(clipname);
    }


    public static void PlaySoundContinuous(AudioSource source, string clipName)
    {
        if (isMuted) return;

        source.loop = true;

        foreach (var clip in SFXList)
        {
            if (clip.name == clipName)
            {
                source.clip = clip;
                source.Play();
                return;
            }
        }
    }
    public static void StopSoundContinuous(AudioSource source)
    {
        if (isMuted) return;

        if (source.isPlaying)
            source.Stop();
    }
    static float musicTime;
    public static void ToggleMusic()
    {

        if (MusicChannel.isPlaying)
        {
            musicTime = MusicChannel.time;
            MusicChannel.Stop();
        }
        else
        {
            MusicChannel.time = musicTime;
            MusicChannel.Play();
        }
    }
    static bool isMuted = false;
    public static void ToggleSFX()
    {
        isMuted = !isMuted;
        Debug.Log("isMutedSFX " + isMuted);
        SFXChannel.mute = !SFXChannel.mute;
        SFX2Channel.mute = !SFX2Channel.mute;
        SFX3Channel.mute = !SFX3Channel.mute;
        AmbientChannel.mute = !AmbientChannel.mute;
    }
}
