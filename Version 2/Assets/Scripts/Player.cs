using DataBase;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float jumpForce = 2f;
    private bool _faceRight = true;
    private bool _invincibility;

    public float rage;
    public float maxRage = 30f;
    public bool onRage;
    public float speedRage;
    public float speedHillOnRage;
    public float timerRage;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    private bool _onGround = true;
    private bool _onPlatform;
    private float _groundCheckRadius;
    public LayerMask ground;
    public LayerMask platform;

    private bool _onBlock;
    private float _blockTime;
    public float blockParryTime = 0.1f;
    
    private bool _onRoll;
    public float speedRoll = 5f;

    private int _comboAttack = 1;
    private float _sinceAttack;
    public Transform attackCheckLeftUp;
    public Transform attackCheckRightDown;
    public LayerMask enemy;
    public LayerMask chest;
    public LayerMask boss;
    public float damage;
    public float pushPower = 2f;

    public Transform wallsOnCheck;
    private float _wallsOnCheckRadius;
    public Transform wallsDownCheck;
    private float _wallsDownCheckRadius;
    private bool _onWalls;
    public LayerMask walls;

    private Rigidbody2D _rb;
    private Vector2 _moveVector;
    private Animator _animator;

    public GameObject slideDust;
    public GameObject smallBlockEffect;
    public Transform blockPos;

    public bool death;
    public bool recoveryAfterDeath = true;
    public bool deathAnimation;
    private bool _hurt;
    private bool _menu;
    private bool _attack;
    public float lastTimeTakingDamage;
    private SoundForPlayer _soundForPlayer;
    private CollectionInfoInBd _collection;

    private void Update()
    {
        if (!deathAnimation && !death && !_menu)
        {
            _animator.SetFloat("AirSpeedY", _rb.velocity.y);

            FlipX();

            Run();

            CheckGrounded();

            Jump();

            Block();

            Roll();

            _sinceAttack += Time.deltaTime;
            Attack();

            Rage();

            CheckWalls();

            SlideOnWalls();

            lastTimeTakingDamage += Time.deltaTime;
        }
        if (deathAnimation) lastTimeTakingDamage = 1f;
        if (death || deathAnimation) _rb.velocity = new Vector2(0, _rb.velocity.y);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundCheckRadius = groundCheckLeft.GetComponent<CircleCollider2D>().radius;
        _wallsOnCheckRadius = wallsOnCheck.GetComponent<CircleCollider2D>().radius;
        _wallsDownCheckRadius = wallsDownCheck.GetComponent<CircleCollider2D>().radius;
        _soundForPlayer = GetComponent<SoundForPlayer>();
        _collection = GetComponent<CollectionInfoInBd>();
    }

    
    private void Run()
    {
        if (!_hurt && !_onRoll && !_onBlock && !_hurt && !_attack)
        {
            if (Input.GetButton("Horizontal"))
            {
                _moveVector.x = Input.GetAxis("Horizontal");
                _animator.SetInteger("AnimState", 1);
                _rb.velocity = new Vector2(_moveVector.x * speed, _rb.velocity.y);
            }
            else
            {
                _animator.SetInteger("AnimState", 0);
                _rb.velocity = new Vector2(0, _rb.velocity.y);
            }
        }
        else
        {
            _animator.SetInteger("AnimState", 0);
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Down") && _onPlatform && !_onBlock && !_onRoll && !_hurt && !_attack){
            _onPlatform = false;
            Physics2D.IgnoreLayerCollision(9, 11, true);
            Physics2D.IgnoreLayerCollision(3, 11, true);
            _onPlatform = false;
            Invoke(nameof(IgnorePlatformOff), 0.3f);
            Invoke(nameof(IgnorePlatformCheckPoint), 0.3f);
        }

        if (Input.GetButtonDown("Jump") && _onGround && !_onBlock && !_onRoll && !_hurt && !_attack)
        {
            if (_onPlatform) _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _animator.SetTrigger("Jump");
            _rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            _animator.SetBool("Grounded", _onGround);
            _animator.SetInteger("AnimState", 0);
        }
    }

    private void IgnorePlatformOff(){
        Physics2D.IgnoreLayerCollision(9, 11, false);
    }

    private void IgnorePlatformCheckPoint(){
        Physics2D.IgnoreLayerCollision(3, 11, false);
    }

    private void Roll()
    {
        if (Input.GetButtonDown("Roll") && _onGround && !_onBlock && !_onRoll && !_hurt && !_attack)
        {
            _onRoll = true;
            _animator.SetTrigger("Roll");
        }

        if (_onRoll && _onGround)
        {
            if (_faceRight) _rb.velocity = new Vector2(speedRoll, _rb.velocity.y);
            else _rb.velocity = new Vector2(-speedRoll, _rb.velocity.y);
        }
    }

    private void Rage()
    {
        if (onRage)
        {
            timerRage += Time.deltaTime;
            if (rage > 0 && timerRage >= 1.5f) rage -= speedRage;
            if (health < maxHealth) health += speedHillOnRage;

            if (rage <= 0)
            {
                rage = 0;
                onRage = false;
            }
        }

        if (Input.GetButtonDown("Rage"))
        {
            if (onRage && timerRage >= 1.5f)
            {
                onRage = false;
                damage /= 1.5f;
                timerRage = 0;
                return;
            }
            if (!onRage && rage >= 3f)
            {
                onRage = true;
                damage *= 1.5f;
                _soundForPlayer.PlayRageReplica();
            }
        }
    }

    private void PlusRage(float exp)
    {
        if (onRage || rage >= maxRage) return;
        if (rage + exp >= maxRage) rage = maxRage;
        else rage += exp;
    }

    private void MinusRage(float exp)
    {
        if (onRage || rage <= 0) return;
        if (rage - exp < 0) rage = 0;
        else rage -= exp;
    }
    
    private void Attack()
    {
        if (_sinceAttack < 0.35f || _onBlock || _onRoll || _hurt || !_onGround) return;
        if (Input.GetButtonDown("Attack"))
        {
            _attack = true;
            _comboAttack++;
            if (_comboAttack > 3) _comboAttack = 1;

            if (_sinceAttack > 1.2f) _comboAttack = 1;

            _animator.SetTrigger("Attack" + _comboAttack);

            _sinceAttack = 0;
            
        }
    }

    public void OnAttack()
    {
        var hitEnemies = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            enemy
        );

        var count = 0;

        foreach (var enemyCollider in hitEnemies)
        {
            var enemyObject = enemyCollider.GetComponent<Enemy>();
            var dmg = enemyObject.TakingDamage(damage, transform.position, pushPower);
            if (enemyObject.canExpRageAfterDeath)
            {
                if (enemyObject.death)
                {
                    var exp = enemyObject.expRage;
                    PlusRage(exp);
                    switch (exp)
                    {
                        case 4f:
                            _collection.KillBlackBandit();
                            break;
                        case 8.5f:
                            _collection.KillWhiteBandit();
                            break;
                    }
                    enemyObject.GiveCanExpRageAfterDeath();
                    _soundForPlayer.DeathEnemyReplica();
                }
                else
                {
                    PlusRage(0.1f);
                }
                _collection.Attack(dmg);
                count++;
            }
        }
        
        var hitBoss = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            boss
        );

        foreach (var bossCollider in hitBoss)
        {
            var bossObject = bossCollider.GetComponent<Boss>();
            var dmg = bossObject.TakingDamage(damage, transform.position, pushPower);
            _collection.Attack(dmg);
            PlusRage(0.1f);
        }

        var listChest = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            chest
        );

        foreach (var chestCollider in listChest)
        {
            var chestObject = chestCollider.GetComponent<Chest>();
            chestObject.OpenChest();
            _soundForPlayer.OpenChestReplica();
            _collection.OpenChest();
        }

        _soundForPlayer.enemy = count > 0;
    }

    private void SlideOnWalls()
    {
        if (_onWalls && !_onGround && !_onBlock && !_onRoll && !_hurt && !_attack && lastTimeTakingDamage > 0.5f)
        {
            _animator.SetTrigger("WallSlide");
        }
    }

    private void Block()
    {
        if (!_onGround || _onRoll || _hurt || _attack) return;
        if (Input.GetButton("Block"))
        {
            _animator.SetInteger("AnimState", 0);
            _animator.SetInteger("AnimState2", 1);
            _rb.velocity = new Vector2(0, _rb.velocity.y);

            _blockTime += Time.deltaTime;
            _onBlock = true;
        }
        else
        {
            _animator.SetInteger("AnimState2", 0);
            _blockTime = 0;
            _onBlock = false;
        }
    }

    private void ParryBlock()
    {
        _animator.SetTrigger("Block");
        _animator.SetInteger("AnimState2", 0);
        PlusRage(0.7f);
    }

    public void TakingDamage(float damageEnemy, Vector3 positionAttack, float pushPowerAttack)
    {
        if (_invincibility) return;
        lastTimeTakingDamage = 0;
        var enemyRight = positionAttack.x > transform.position.x;
        if (_onBlock && _blockTime <= blockParryTime && enemyRight == _faceRight) ParryBlock();
        else
        {
            if (_onBlock && enemyRight == _faceRight)
            {
                MinusRage(0.8f);
                if (rage == 0)
                {
                    if (health >= damageEnemy / 2) _collection.TakingDamage(damageEnemy / 2);
                    else _collection.TakingDamage(health);
                    health -= damageEnemy / 2;
                    
                }
                
                Death();
                SmallBlockEvent();
            }
            else
            {
                if (health >= damageEnemy) _collection.TakingDamage(damageEnemy);
                else _collection.TakingDamage(health);
                health -= damageEnemy;
                
                MinusRage(0.05f);

                Death();
                _hurt = true;
                if (!deathAnimation) _animator.SetTrigger("Hurt");
                _onRoll = false;
                _attack = false;
            }
        }
        
        if (enemyRight)
            _rb.AddForce(-transform.right * pushPowerAttack, ForceMode2D.Impulse);
        else
            _rb.AddForce(transform.right * pushPowerAttack, ForceMode2D.Impulse);
    }

    private void Death()
    {
        if (health > 0 || deathAnimation || death) return; 

        _animator.SetInteger("AnimState", 0);
        _animator.SetInteger("AnimState2", 0);
        _animator.SetBool("noBlood", false);
        _collection.SetDeath();
        deathAnimation = true;
        _animator.SetTrigger("Death");
    }

    public void Invincibility()
    {
        _invincibility = true;
    }

    public void NoInvincibility()
    {
        _invincibility = false;
    }

    public void NoHurt()
    {
        _onRoll = false;
        _hurt = false;
        _attack = false;
    }

    public void NoRoll()
    {
        _onRoll = false;
    }

    public void NoAttack()
    {
        _attack = false;
    }

    public void SetNoDeath()
    {
        death = false;
    }

    public void SetDeath()
    {
        Invoke(nameof(SetDeathInvoke), 1f);
    }

    private void SetDeathInvoke(){
        if (recoveryAfterDeath){
            recoveryAfterDeath = false;
            deathAnimation = false;
            death = false;
            _onBlock = false;
            _attack = false;
            _onRoll = false;
            onRage = false;
            _hurt = false;

            health = maxHealth;
            _soundForPlayer.PlayRecoveryReplica();
            _animator.SetTrigger("Recovery");

            _invincibility = true;
            Invoke(nameof(NoInvincibility), 0.7f);
        }
        else death = true;
    }

    public void SetMenu()
    {
        _menu = !_menu;
    }

    public void Error(){
        _attack = false;
        _onBlock = false;
        _onRoll = false;
        _hurt = false;
        _invincibility = false;
    }

    private void FlipX()
    {
        if ((_moveVector.x >= 0 && _faceRight) || (_moveVector.x <= 0 && !_faceRight)) return;
        transform.localScale *= new Vector2(-1, 1);
        _faceRight = !_faceRight;
    }

    private void CheckGrounded()
    {
        _onPlatform = Physics2D.OverlapCircle(groundCheckLeft.position, _groundCheckRadius, platform) || 
        Physics2D.OverlapCircle(groundCheckRight.position, _groundCheckRadius, platform);
        
        var change = _onGround;
        _onGround = Physics2D.OverlapCircle(groundCheckLeft.position, _groundCheckRadius, ground) || 
        Physics2D.OverlapCircle(groundCheckRight.position, _groundCheckRadius, ground) || _onPlatform;
        _animator.SetBool("Grounded", _onGround);
        
        if (!change && _onGround) _soundForPlayer.PlayLanding();
    }

    private void CheckWalls()
    {
        _onWalls = Physics2D.OverlapCircle(wallsOnCheck.position, _wallsOnCheckRadius, walls) &&
            Physics2D.OverlapCircle(wallsDownCheck.position, _wallsDownCheckRadius, walls);
        _animator.SetBool("WallSlide", _onWalls);
    }

    public void AE_SlideDust()
    {
        var spawnPosition = wallsDownCheck.transform.position;

        if (slideDust != null)
        {
            Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation);
        }
    }

    private void SmallBlockEvent()
    {
        if (smallBlockEffect != null && blockPos != null)
        {
            var spawnPosition = blockPos.transform.position;
            Instantiate(smallBlockEffect, spawnPosition, gameObject.transform.localRotation);
        }
    }

    public void SetRecoveryHealth(float recoveryHealth){
        if (health >= maxHealth) return;
        if (health + recoveryHealth >= maxHealth) health = maxHealth;
        else health += recoveryHealth;
    }

    public void SetRecoveryRage(float recoveryRage){
        if (rage >= maxRage) return;
        if (rage + recoveryRage >= maxRage) rage = maxRage;
        else rage += recoveryRage;
    }

    public void SetIncreaseDamage(float currentDamage, bool multiply){
        if (multiply) damage *= currentDamage;
        else damage += currentDamage;
    }

    public void SetPowerPunch(float currentPunch)
    {
        pushPower *= currentPunch;
    }

    public void SetRecoveryAfterDeath(){
        recoveryAfterDeath = true;
    }

    public bool GetOnBlock()
    {
        return _onBlock;
    }

    public bool GetInvincibility()
    {
        return _invincibility;
    }
}
