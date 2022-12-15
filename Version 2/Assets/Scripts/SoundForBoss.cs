using UnityEngine;

public class SoundForBoss : MonoBehaviour
{
    public AudioClip stepSound;
    public AudioClip sword1Sound;
    public AudioClip sword2Sound;
    public AudioClip sword3Sound;
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
    public AudioClip combo1Sound;
    public AudioClip combo2Sound;
    public AudioClip hurt1Sound;
    public AudioClip hurt2Sound;
    public AudioClip hurt3Sound;
    public AudioClip dieSound;
    public AudioClip win1Sound;
    public AudioClip win2Sound;
    public AudioClip win3Sound;

    private AudioSource _audioBoss;
    private float _timer;
    private Boss _boss;
    private Player _player;
    private bool _count;

    private void Awake()
    {
        _audioBoss = GetComponent<AudioSource>();
        _boss = GetComponent<Boss>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (_timer <= 0) _timer += Time.deltaTime;
        else _timer = 0;

        if (_player.deathAnimation && !_player.recoveryAfterDeath && !_count)
        {
            _count = true;
            PlayWinSound();
        }
    }

    public void PlayStepSound()
    {
        _audioBoss.PlayOneShot(stepSound);
    }

    public void PlayAttackSound()
    {
        if (_timer < 0) return;
        var i = RandomClip();
        switch (i)
        {
            case <= 50:
                _audioBoss.PlayOneShot(attack1Sound);
                break;
            default:
                _audioBoss.PlayOneShot(attack2Sound);
                break;
        }

        _timer -= 0.5f;
    }

    public void PlayComboSound()
    {
        if (_timer < 0) return;
        var i = RandomClip();
        switch (i)
        {
            case <= 50:
                _audioBoss.PlayOneShot(combo1Sound);
                break;
            default:
                _audioBoss.PlayOneShot(combo2Sound);
                break;
        }

        _timer -= 2.5f;
    }

    public void PlayWinSound()
    {
        _audioBoss.bypassEffects = true;
        var i = RandomClip();
        switch (i)
        {
            case <= 30:
                _audioBoss.clip = win1Sound;
                _audioBoss.Play();
                break;
            case <= 60:
                _audioBoss.clip = win3Sound;
                break;
            default:
                _audioBoss.clip = win2Sound;
                break;
        }

        _timer -= 3f;
    }
    
    public void PlayHurtSound()
    {
        if (_timer < 0) return;
        var i = RandomClip();
        switch (i)
        {
            case <= 50:
                _audioBoss.PlayOneShot(hurt1Sound);
                break;
            case <= 60:
                _audioBoss.PlayOneShot(hurt3Sound);
                break;
            default:
                _audioBoss.PlayOneShot(hurt2Sound);
                break;
        }

        _timer -= 0.2f;
    }

    public void PlayDeathSound()
    {
        _audioBoss.clip = dieSound;
        _audioBoss.Play();
        _boss.SetDeath();
    }

    public void PlaySwordSound(int i)
    {
        switch (i)
        {
            case 1:
                _audioBoss.PlayOneShot(sword1Sound);
                break;
            case 2:
                _audioBoss.PlayOneShot(sword2Sound);
                break;
            case 3:
                _audioBoss.PlayOneShot(sword3Sound);
                break;
        }
    }
    
    private static int RandomClip() {
        return Random.Range(0, 101);
    }
}
