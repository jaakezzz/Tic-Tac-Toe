using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip woodClickClip;
    public AudioClip woodPieceClip;
    public AudioClip winChimeClip;
    public AudioClip drawThudClip;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    void Awake()
    {
        // Standard Singleton Pattern to ensure only one AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps audio playing seamlessly between scenes (if you add more later)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMenuMusic(); // Start with the menu track
    }

    public void PlayMenuMusic()
    {
        // Prevent restarting the track if it's already playing
        if (musicSource.clip == menuMusic) return;

        musicSource.clip = menuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (musicSource.clip == gameMusic) return;

        musicSource.clip = gameMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // --- SOUND EFFECT TRIGGERS ---

    public void PlayClick()
    {
        sfxSource.PlayOneShot(woodClickClip);
    }

    public void PlayPlacePiece()
    {
        sfxSource.PlayOneShot(woodPieceClip);
    }

    public void PlayWin()
    {
        sfxSource.PlayOneShot(winChimeClip);
    }

    public void PlayDraw()
    {
        sfxSource.PlayOneShot(drawThudClip);
    }

    // --- VOLUME CONTROLS ---

    // Unity AudioMixers use Decibels (logarithmic), not standard 0.0 to 1.0 floats.
    // We use Mathf.Log10 to convert the linear slider value to accurate decibels.

    public void SetMasterVolume(float sliderValue)
    {
        // Prevent log(0) error by clamping the minimum value
        float volume = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float volume = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volume = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }
}