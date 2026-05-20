using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string SFX_VOLUME_KEY = "sfx_volume";
    private const string MUSIC_VOLUME_KEY = "music_volume";
    private const string SFX_MUTED_KEY = "sfx_muted";
    private const string MUSIC_MUTED_KEY = "music_muted";

    [SerializeField] private int sfxPoolSize = 5;

    private List<AudioSource> sfxSources = new List<AudioSource>();
    private AudioSource musicSource;

    private float sfxVolume = 1f;
    private float musicVolume = 0.6f;
    private bool sfxMuted = false;
    private bool musicMuted = false;

    public float SfxVolume => sfxVolume;
    public float MusicVolume => musicVolume;
    public bool SfxMuted => sfxMuted;
    public bool MusicMuted => musicMuted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = new GameObject("SFX_Source_" + i);
            go.transform.SetParent(transform);
            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxSources.Add(src);
        }

        GameObject musicGo = new GameObject("Music_Source");
        musicGo.transform.SetParent(transform);
        musicSource = musicGo.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.6f);
        sfxMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, 0) == 1;
        musicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, 0) == 1;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySFX called with null clip");
            return;
        }
        if (sfxMuted) return;

        AudioSource src = GetFreeSource();
        src.pitch = pitch;
        src.volume = sfxVolume * volumeScale;
        src.PlayOneShot(clip);
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource s in sfxSources)
        {
            if (!s.isPlaying) return s;
        }
        return sfxSources[0];
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlayMusic called with null clip");
            return;
        }
        musicSource.clip = clip;
        musicSource.volume = musicMuted ? 0f : musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        musicSource.volume = musicMuted ? 0f : musicVolume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
    }

    public void SetSfxMuted(bool muted)
    {
        sfxMuted = muted;
        PlayerPrefs.SetInt(SFX_MUTED_KEY, muted ? 1 : 0);
    }

    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;
        musicSource.volume = muted ? 0f : musicVolume;
        PlayerPrefs.SetInt(MUSIC_MUTED_KEY, muted ? 1 : 0);
    }
}
