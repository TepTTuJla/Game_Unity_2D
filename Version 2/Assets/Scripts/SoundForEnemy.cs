using UnityEngine;

public class SoundForEnemy : MonoBehaviour
{
    public AudioClip axeMissSound;
    public AudioClip axeInSound;
    public AudioClip axeShieldSound;
    public AudioClip stepSound;
    public AudioClip landingSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip parrySound;
    private AudioSource _audioEnemy;

    private void Awake()
    {
        _audioEnemy = GetComponent<AudioSource>();
    }

    public void PlayStepSound()
    {
        _audioEnemy.PlayOneShot(stepSound);
    }

    public void PlayLandingSound()
    {
        _audioEnemy.PlayOneShot(landingSound);
    }

    public void PlayHurtSound()
    {
        if (hurtSound) _audioEnemy.PlayOneShot(hurtSound);
    }

    public void PlayDeathSound()
    {
        if (deathSound) _audioEnemy.PlayOneShot(deathSound);
    }

    public void PlayAttackSound(int i)
    {
        switch (i)
        {
            case 1:
                _audioEnemy.PlayOneShot(axeMissSound);
                break;
            case 2:
                _audioEnemy.PlayOneShot(axeInSound);
                break;
            case 3:
                _audioEnemy.PlayOneShot(axeShieldSound);
                break;
        }
    }

    public void PlayParryShield()
    {
        _audioEnemy.PlayOneShot(parrySound);
    }
}
