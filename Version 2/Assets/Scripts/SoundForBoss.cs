using UnityEngine;

public class SoundForBoss : MonoBehaviour
{
    public AudioClip stepSound;
    public AudioClip swordMissSound;
    public AudioClip swordInSound;
    public AudioClip swordShieldSound;
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
    private AudioSource _endAudio;
    private float _timer;
    private Boss _boss;
    private Player _player;
    private bool _count;

    private void Awake()
    {
        _audioBoss = GetComponent<AudioSource>();
        _boss = GetComponent<Boss>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _endAudio = GameObject.FindWithTag("Point").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_timer <= 0) _timer += Time.deltaTime;
        else _timer = 0;

        if (_player.deathAnimation && !_player.recoveryAfterDeath && !_count)
        {
            _timer = -10f;
            _count = true;
            Invoke(nameof(PlayWinSound), 2f);
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
        if (!_boss.active) return;
        var i = RandomClip();
        _audioBoss.Stop();
        switch (i)
        {
            case <= 30:
                _endAudio.PlayOneShot(win1Sound);
                break;
            case <= 60:
                _endAudio.PlayOneShot(win3Sound);
                break;
            default:
                _endAudio.PlayOneShot(win2Sound);
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
        _audioBoss.Stop();
        _endAudio.PlayOneShot(dieSound);
        _boss.SetDeath();
    }

    public void PlaySwordSound(int i)
    {
        switch (i)
        {
            case 1:
                _audioBoss.PlayOneShot(swordMissSound);
                break;
            case 2:
                _audioBoss.PlayOneShot(swordInSound);
                break;
            case 3:
                _audioBoss.PlayOneShot(swordShieldSound);
                break;
        }
    }
    
    private static int RandomClip() {
        return Random.Range(0, 101);
    }
}
