using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;

    public AudioClip shootSound;
    public AudioClip explosionSound;
    public AudioClip shieldSound;

    public AudioClip destroyTowerSound;
    public AudioClip DyingSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayShootSound()
    {
        if (shootSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(shootSound);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
}