using UnityEngine;

public class SoundForPlayer : MonoBehaviour
{
    public AudioClip attackSound;
    public AudioClip attackEnemySound;
    public AudioClip runSound;
    public AudioClip wallSlideSound;
    public AudioClip landingSound;
    public AudioClip rollSound;
    public AudioClip shieldOnSound;
    public AudioClip shieldParrySound;
    public AudioClip shieldNoParrySound;


    public AudioClip takingDamageClip1;
    public AudioClip takingDamageClip2;
    public AudioClip deathClip1;
    public AudioClip deathClip2;
    public AudioClip recoveryClip1;
    public AudioClip recoveryClip2;
    public AudioClip recoveryClip3;
    public AudioClip rageClip1;
    public AudioClip rageClip2;
    public AudioClip attackClip1;
    public AudioClip attackClip2;
    public AudioClip attackClip3;
    public AudioClip attackClip4;
    public AudioClip blockClip1;
    public AudioClip blockClip2;
    public AudioClip openChestClip;
    public AudioClip deathEnemyClip1;
    public AudioClip deathEnemyClip2;
    public AudioClip deathBossClip1;
    public AudioClip deathBossClip2;

    private Boss _boss;
    private AudioSource _audioPlayer;
    public bool enemy;
    private float _timer;
    public float changeReplica = 2.5f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_boss.deathAnimation) DeathBossReplica();
    }

    private void Awake()
    {
        _boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
        _audioPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
    }

    public void PlayStep()
    {
        _audioPlayer.PlayOneShot(runSound);
    }

    public void PlayAttack(){
        if (enemy) _audioPlayer.PlayOneShot(attackEnemySound);
        else _audioPlayer.PlayOneShot(attackSound);
    }

    public void PlayRoll(){
        _audioPlayer.PlayOneShot(rollSound);
    }

    public void PlayLanding(){
        _audioPlayer.PlayOneShot(landingSound);
    }


    public void PlayAttackReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        switch (i)
        {
            case <= 25:
                _audioPlayer.PlayOneShot(attackClip2);
                _timer = 0;
                break;
            case <= 50:
                _audioPlayer.PlayOneShot(attackClip3);
                _timer = 0;
                break;
            case <= 75:
                _audioPlayer.PlayOneShot(attackClip4);
                _timer = 0;
                break;
            case <= 90:
                _audioPlayer.PlayOneShot(attackClip1);
                _timer = 0;
                break;
        }
    }

    public void PlayTakingDamageReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        if (i <= 50){
            _audioPlayer.PlayOneShot(takingDamageClip1);
            _timer = 0;
            return;
        }
        _audioPlayer.PlayOneShot(takingDamageClip2);
        _timer = 0;
    }

    public void PlayDeathReplica(){
        var i = RandomClip();
        //if (_timer < changeReplica) return;
        if (i <= 50){
            _audioPlayer.PlayOneShot(deathClip1);
            _timer = 0;
            return;
        }
        _audioPlayer.PlayOneShot(deathClip2);
        _timer = 0;
    }

    public void PlayRecoveryReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        switch (i)
        {
            case <= 33:
                _audioPlayer.PlayOneShot(recoveryClip1);
                _timer = 0;
                break;
            case <= 66:
                _audioPlayer.PlayOneShot(recoveryClip2);
                _timer = 0;
                break;
            default:
                _audioPlayer.PlayOneShot(recoveryClip3);
                _timer = 0;
                break;
        }
    }

    public void PlayRageReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        if (i <= 50) {
            _audioPlayer.PlayOneShot(rageClip1);
            _timer = 0;
            return;
        }
        _audioPlayer.PlayOneShot(rageClip2);
        _timer = 0;
    }

    public void PlayBlockReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        switch (i)
        {
            case <= 30:
                _audioPlayer.PlayOneShot(blockClip1);
                _timer = 0;
                break;
            case <= 60:
                _audioPlayer.PlayOneShot(blockClip2);
                _timer = 0;
                break;
        }
    }

    public void OpenChestReplica(){
        if (_timer < changeReplica) return;
        _audioPlayer.PlayOneShot(openChestClip);
        _timer = 0;
    }

    public void DeathEnemyReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        switch (i)
        {
            case <= 30:
                _audioPlayer.PlayOneShot(deathEnemyClip1);
                _timer = 0;
                break;
            case <= 60:
                _audioPlayer.PlayOneShot(deathEnemyClip1);
                _timer = 0;
                break;
        }
    }

    private void DeathBossReplica(){
        var i = RandomClip();
        if (_timer < changeReplica) return;
        if (i <= 50) {
            _audioPlayer.PlayOneShot(deathBossClip1);
            _timer = 0;
            return;
        }
        _audioPlayer.PlayOneShot(deathBossClip2);
        _timer = 0;
    }

    private static int RandomClip() {
        return Random.Range(0, 101);
    }
}
