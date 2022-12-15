using UnityEngine;
using UnityEngine.UI;

public class Enemy: MonoBehaviour
{
    public int id;
    public float expRage;
    public bool canExpRageAfterDeath = true;

    public Transform observationCheck;
    public float radiusObservation;

    public Transform groundCheckRight;
    public Transform groundCheckLeft;
    private float _groundCheckRadius;
    public Transform wallsCheckUp;
    public Transform wallsCheckDown;
    private float _wallsCheckRadius;
    public float speed = 10f;

    public Slider healthBar;
    public float maxHealth;
    public float health = 40f;
    public bool death;

    public float findDistanceFace = 20f;
    public float findDistanceBack = 2f;
    private bool _onGround = true;
    private bool _onPlatform;
    private bool _withWalls;

    public float pushPower = 2f;
    public float timeIdle = 30f;
    private float _time;
    private int _idlePhase;
    public bool faceRight;
    private bool _idle = true;

    private Animator _animator;
    private Rigidbody2D _rb;

    public float damage = 3f;
    private float _attackTime;
    public float kdAttackConst = 2.0f;
    private float _kdAttack;
    public float attackDistance = 5f;
    private bool _attack;
    private float _stopTime;
    public LayerMask player;
    public LayerMask enemy;
    public Transform attackCheckLeftUp;
    public Transform attackCheckRightDown;
    private int _layerMaskForEnemy;

    public LayerMask walls;
    public LayerMask ground;
    public LayerMask platform;
    private Transform _playerObject;
    public Transform bodyPlayerObject;
    public GameObject warningAttack;
    public bool regeneration;
    private bool _invincibility;
    private bool _isDead;
    private SetDeathEnemyFromStart _isDeadClass;
    public bool isFlipX;
    private SoundForEnemy _soundEnemy;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundCheckRadius = groundCheckRight.GetComponent<CircleCollider2D>().radius;
        _wallsCheckRadius = wallsCheckUp.GetComponent<CircleCollider2D>().radius;
        _playerObject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        warningAttack.SetActive(false);
        _isDeadClass = GetComponent<SetDeathEnemyFromStart>();
        _soundEnemy = GetComponent<SoundForEnemy>();
        
        var layerMaskGround = 1 << 6;
        var layerMaskWalls = 1 << 7;
        var layerMaskPlayers = 1 << 9;
        _layerMaskForEnemy = layerMaskGround | layerMaskPlayers | layerMaskWalls;

        _kdAttack = Random.Range(kdAttackConst - 0.5f, kdAttackConst + 0.5f);
    }

    private void Update()
    {
        if (_isDeadClass) _isDead = _isDeadClass.isDead;
        if (death)
        {
            canExpRageAfterDeath = !_invincibility;
            
            warningAttack.SetActive(false);
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            if (_isDead)
            {
                _time = 15f;
                Invincibility();
            }
            else Regeneration();
        }
        else
        {
            healthBar.value = health;
            
            _animator.SetFloat("AirSpeed", _rb.velocity.y);
            CheckGrounded();
            CheckWalls();
            _animator.SetFloat("Speed", _rb.velocity.x);
            if ((_rb.velocity.x >= 1.1f || _rb.velocity.x <= -1.1f) &&
                Mathf.Abs(transform.position.x - _playerObject.transform.position.x) < 20f) _animator.SetInteger("AnimState", 2);
            else _animator.SetInteger("AnimState", _idlePhase);
            
            if (_idle) {
                Idle();
                _stopTime = 0;
            }
            else {
                AttackIdle();
                if (_stopTime >= 0.5f && !_attack) GoWithPlayer();
                else _stopTime += Time.deltaTime;
                if (_attack) _rb.velocity = new Vector2(0, _rb.velocity.y);
            }
        }
    }

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    private void Death()
    {
        if (health > 0) return;
        _soundEnemy.PlayDeathSound();
        _animator.SetTrigger("Death");
        death = true;
        healthBar.gameObject.SetActive(false);
        death = true;
        _time = 0;
    }

    private bool DetectionPlayer(){
        var hitPlayer = Physics2D.OverlapCircleAll(observationCheck.position, radiusObservation, player);
        if (hitPlayer.Length == 0) {
            return false;
        }
        
        var playerRight = bodyPlayerObject.position.x >= transform.position.x;
        RaycastHit2D hit;
        RaycastHit2D hitInEnemy;

        if (faceRight && playerRight || !faceRight && !playerRight)
            hitInEnemy = Physics2D.Raycast(
                new Vector2(observationCheck.position.x - 2f, observationCheck.position.y),
                bodyPlayerObject.position - observationCheck.position,
                findDistanceFace + 2f,
                _layerMaskForEnemy
            );
        else
            hitInEnemy = Physics2D.Raycast(
                new Vector2(observationCheck.position.x + 2f, observationCheck.position.y),
                bodyPlayerObject.position - observationCheck.position,
                findDistanceBack + 2f,
                _layerMaskForEnemy
            );


        if (hitPlayer.Length == 0) {
            return false;
        }
        if (playerRight == faceRight)
        {
            hit = Physics2D.Raycast(
            new Vector2(observationCheck.position.x + 2f, observationCheck.position.y),
            bodyPlayerObject.position - observationCheck.position,
            findDistanceFace + 2f,
            _layerMaskForEnemy
            );
        }
        else
        {
            hit = Physics2D.Raycast(
            new Vector2(observationCheck.position.x - 2f, observationCheck.position.y),
            bodyPlayerObject.position - observationCheck.position,
            findDistanceBack + 2f,
            _layerMaskForEnemy
            );
        }

        if (hit.collider) {
            if (hit.collider.CompareTag(_playerObject.tag)) return true;
        }
        
        if (hitInEnemy.collider) {
            if (hitInEnemy.collider.CompareTag(_playerObject.tag))
                return true;
        }
        return false;
    }


    private void Idle()
    {
        if (isFlipX) IdleFlip();
        if (_idle) _idlePhase = 0;
        if (_idle && DetectionPlayer()){
            _idle = false;
            _idlePhase = 1;
        }
    }

    private void IdleFlip(){
        _time += Time.deltaTime;
        if (_time < timeIdle) return;
        transform.localScale *= new Vector2(-1, 1);
        faceRight = !faceRight;
        _time = 0f;
    }
    
    private void AttackIdle()
    {
        FlipWithPlayer();
        ComeBackToIdle();
        _attackTime += Time.deltaTime;

        var hit = Physics2D.Raycast(faceRight ? new Vector2(observationCheck.position.x - 1f, observationCheck.position.y) : new Vector2(observationCheck.position.x + 1f, observationCheck.position.y), bodyPlayerObject.position - observationCheck.position, attackDistance + 1f, _layerMaskForEnemy);

        if (hit.collider)
        {
            if (hit.collider.CompareTag(_playerObject.tag) && _attackTime >= _kdAttack)
            {
                _attackTime = 0;
                var timer = 0f;
                while (timer <= 1f) timer += Time.deltaTime;
                _animator.SetTrigger("Attack");
                _attack = true;
                _kdAttack = Random.Range(kdAttackConst - 0.5f, kdAttackConst + 0.5f);
            }
        }
    }

    private void ComeBackToIdle()
    {
        if (!DetectionPlayer()) _time += Time.deltaTime;
        else _time = 0;
        if (_time < timeIdle) return;
        _time = 0;
        _idle = true;
        _idlePhase = 0;
    }
    

    public float TakingDamage(float damageOnEnemy, Vector3 positionAttack, float pushPowerAttack)
    {
        if (death || _invincibility)
        {
            return 0;
        }
        _time = 0;
        float dmg;
        if (health >= damageOnEnemy) dmg = damageOnEnemy;
        else dmg = health;
        health -= damageOnEnemy;
        Death();
        
        if (!death) _soundEnemy.PlayHurtSound();
        if (_rb == null || pushPower == 0) return 0;

        if (_attack) return dmg;
        _animator.SetTrigger("Hurt");
        

        if (positionAttack.x >= transform.position.x) _rb.AddForce(-transform.right * pushPowerAttack, ForceMode2D.Impulse);
        else _rb.AddForce(transform.right * pushPowerAttack, ForceMode2D.Impulse);
        
        _rb.AddForce(transform.up * pushPowerAttack, ForceMode2D.Impulse);
        if (_idle) _idle = false;
        return dmg;
    }

    private void GoWithPlayer()
    {
        if (Mathf.Abs(transform.position.x - _playerObject.transform.position.x) > attackDistance)
        {
            if (_playerObject.position.x < transform.position.x - attackDistance && !_withWalls)
                _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            if (_playerObject.position.x > transform.position.x + attackDistance && !_withWalls)
                _rb.velocity = new Vector2(speed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    private void FlipWithPlayer()
    {
        if (transform.position.x > _playerObject.position.x && faceRight || 
            transform.position.x < _playerObject.position.x && !faceRight)
        {
            faceRight = !faceRight;
            transform.localScale *= new Vector2(-1, 1);
        } 
    }

    public void OnAttack()
    {
        /*var hitEnemies = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            enemy);*/
        var hitPlayers = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            player);
        
        /*_soundEnemy.PlayAttackSound(hitEnemies.Length != 0 || hitPlayers.Length != 0);

        foreach (var enemyHit in hitEnemies)
        {
            if (id != enemyHit.GetComponent<Enemy>().id)
            {
                enemyHit.GetComponent<Enemy>().TakingDamage(damage, transform.position, pushPower);
            }
        }*/
        _soundEnemy.PlayAttackSound(hitPlayers.Length == 0);
        foreach (var playerHit in hitPlayers)
        {
            playerHit.GetComponent<Player>().TakingDamage(damage, transform.position, pushPower);
        }
    }


    private void CheckGrounded()
    {
        var boolean = _onGround;
        _onPlatform = Physics2D.OverlapCircle(groundCheckRight.position, _groundCheckRadius, platform) ||
        Physics2D.OverlapCircle(groundCheckLeft.position, _groundCheckRadius, platform);
        _onGround = Physics2D.OverlapCircle(groundCheckRight.position, _groundCheckRadius, ground) ||
        Physics2D.OverlapCircle(groundCheckLeft.position, _groundCheckRadius, ground) || _onPlatform;
        _animator.SetBool("Grounded", _onGround);

        if (!boolean && _onGround) _soundEnemy.PlayLandingSound();
    }

    private void CheckWalls()
    {
        _withWalls = Physics2D.OverlapCircle(wallsCheckUp.position, _wallsCheckRadius, walls) ||
        Physics2D.OverlapCircle(wallsCheckDown.position, _wallsCheckRadius, walls);
    }


    public void SetNoAttack()
    {
        _attack = false;
    }

    public void SetId(int idChange)
    {
        id = idChange;
    }

    public void Regeneration()
    {
        _time += Time.deltaTime;
        if (_time < 20f) return;
        if (!regeneration)
        {
            Destroy(gameObject);
            enabled = false;
        }
        else {
            death = false;
            health = maxHealth;
            _animator.SetTrigger("Recover");
            healthBar.gameObject.SetActive(true);
            canExpRageAfterDeath = true;
        }
    }

    public void Invincibility()
    {
        _invincibility = true;
    }

    public void NoInvincibility()
    {
        _invincibility = false;
    }

    public void GiveCanExpRageAfterDeath()
    {
        canExpRageAfterDeath = false;
    }

    public void SetTrueWarningAttack(){
        warningAttack.SetActive(true);
    }

    public void SetFalseWarningAttack(){
        warningAttack.SetActive(false);
    }

    public void SetNoIdle()
    {
        _idle = false;
    }

    public void SetIdle()
    {
        _idle = true;
        _rb.velocity = new Vector2(0, _rb.velocity.y);
    }

    public bool GetIdle()
    {
        return _idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(observationCheck.position, radiusObservation);
        Physics2D.Raycast(
            observationCheck.position,
            Vector2.right,
            findDistanceFace
            );
    }
}
