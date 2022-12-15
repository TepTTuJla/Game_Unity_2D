using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundForEnemy : MonoBehaviour
{
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
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

    public void PlayAttackSound(bool hit)
    {
        if (hit) _audioEnemy.PlayOneShot(attack1Sound);
        else _audioEnemy.PlayOneShot(attack2Sound);
    }

    public void PlayParryShield()
    {
        _audioEnemy.PlayOneShot(parrySound);
    }
}
