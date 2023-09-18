using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Game Audio Clips:")]
    public AudioSource Shoot_Audio;
    public AudioSource BOOM_Audio;
    public AudioSource Fusion_Audio;
    public AudioSource BackGround_Music;
    public AudioSource Get2x_Audio;

    private bool IsVolumeOn = true;
    public void Music_On()
    {
        BackGround_Music.volume = 0;
    }

    public void Music_Off()
    {
        BackGround_Music.volume = 0.2f;
    }

    public void Volume_On()// Pressed the Volume_On button means volume is Off
    {
        IsVolumeOn = false;
    }

    public void Volume_Off() // Pressed the Volume_Off button means volume is On
    {
        IsVolumeOn = true;
    }

    public void PlayAudioSource(AudioSource Audio_Source)
    {
        if (IsVolumeOn)
        {
            // Play the Audio Source
            Audio_Source.Play();
        }
    }
}
