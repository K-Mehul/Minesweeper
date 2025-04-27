using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    public AudioClip clickSound;
    public AudioClip explosionSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        // Make it Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup audio sources
            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Optional: for background music
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopSound()
    {
        musicSource.Stop();
        sfxSource.Stop();
    }

    public void PlayClick()
    {
        PlaySound(clickSound);
    }

    public void PlayExplosion()
    {
        PlaySound(explosionSound);
    }

    public void PlayWin()
    {
        PlaySound(winSound);
    }

    public void PlayLose()
    {
        PlaySound(loseSound);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
