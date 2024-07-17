using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance;

    public bool SoundEnabled { get; private set; }
    public bool MusicEnabled { get; private set; }

    [SerializeField] private string Sound = "Sound";
    [SerializeField] private string Music = "Music";

    [SerializeField] private AudioMixer _audioMixer;

    [Header("Game")]
    public AudioClip CardSpawned;
    public AudioClip CardTurned;
    public AudioClip CardIncorrect;
    public AudioClip CardCorrect;
    public AudioClip LevelFailed;
    public AudioClip LevelSuccess;
    public AudioClip RouletteSpin;

    [Header("Slot")]
    public AudioClip DefaultWin;
    public AudioClip BigWin;
    public AudioClip Scroll;
    public AudioClip LastScroll;
    public AudioClip SpinSound;

    [Header("Shop")]
    public AudioClip UpgradeSound;


    [SerializeField] private GameObject _soundPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        SoundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("SoundEnabled", 1));
        MusicEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("MusicEnabled", 0));

        UpdateSoundAndMusic();
    }
    public void ToggleSound()
    {
        SoundEnabled = !SoundEnabled;
        UpdateSoundAndMusic();
        Save();
    }
    public void ToggleMusic()
    {
        MusicEnabled = !MusicEnabled;
        UpdateSoundAndMusic();
        Save();
    }
    private void UpdateSoundAndMusic()
    {
        if (SoundEnabled)
            _audioMixer.SetFloat(Sound, 0f);
        else
            _audioMixer.SetFloat(Sound, -80f);
        if (MusicEnabled)
            _audioMixer.SetFloat(Music, 0f);
        else
            _audioMixer.SetFloat(Music, -80f);
    }
    public void PlaySound(AudioClip clip, float pitch, float volume = 1f)
    {
        if (SoundEnabled)
            Instantiate(_soundPrefab).GetComponent<Sound>().PlaySound(clip, pitch, volume);
    }
    public void Save()
    {
        PlayerPrefs.SetInt("SoundEnabled", Convert.ToInt32(SoundEnabled));
        PlayerPrefs.SetInt("MusicEnabled", Convert.ToInt32(MusicEnabled));
    }
}
