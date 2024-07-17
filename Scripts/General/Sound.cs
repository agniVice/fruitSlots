using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    public void PlaySound(AudioClip audioClip, float pitch = 1f, float volume = 1f)
    {
        _source.clip = audioClip;
        _source.pitch = pitch;
        _source.volume = volume;
        _source.Play();

        Destroy(gameObject, audioClip.length + 1);
    }
}