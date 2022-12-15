using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 3f;
    public float health = 100f;
    public float maxHealth = 100f;
    public float jumpForce = 2f;
    private bool _faceRight = true;
    private bool _invincibility;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    private bool _onGround = true;
    private float _groundCheckRadius;
    public LayerMask ground;
    
    private int _comboAttack = 1;
    private float _sinceAttack;
    public float kdAttack;
    public Transform attackCheckLeftUp;
    public Transform attackCheckRightDown;
    public LayerMask player;
    public float damage;
    public float pushPower = 2f;
    public float attackDistance;

    private Rigidbody2D _rb;
    private Animator _animator;

    public bool death;
    public bool deathAnimation;
    private bool _hurt;
    private bool _attack;
    private float _timeEndTakingDamage;
    public float kdAttackInCombo;
    private float _time;
    private bool _combo;
    private Transform _playerPosition;
    private Player _player;
    public bool active;
    public float lastTimeTakingDamage;
    private SoundForBoss _soundForBoss;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundCheckRadius = groundCheckLeft.GetComponent<CircleCollider2D>().radius;
        _playerPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _soundForBoss = GetComponent<SoundForBoss>();
    }

    private void Update()
    {
        if (!active) return;
        if (!death && !deathAnimation)
        {
            FlipX();
            CheckGrounded();
            AccessAnimator();
            _sinceAttack += Time.deltaTime;
            if (_timeEndTakingDamage >= 0) _timeEndTakingDamage = 0;
            else _timeEndTakingDamage += Time.deltaTime;
            
            Run();
            if (Mathf.Abs(_playerPosition.position.x - transform.position.x) <= attackDistance &&
                   _sinceAttack >= kdAttack && _timeEndTakingDamage >= 0)
            {
                if (Random.Range(1, 5) != 2 && !_combo)
                {
                    Attack();
                }
                else
                {
                    AttackCombo();
                }
            }
            else
            {
                if (_combo)
                {
                    speed /= 2;
                    AttackCombo();
                    speed *= 2;
                }
            }
            lastTimeTakingDamage += Time.deltaTime;
        }

        if (deathAnimation || death)
        {
            lastTimeTakingDamage = 1f;
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _animator.SetInteger("AnimState", 0);
        }
    }
    
    private void Run()
    {
        if (Mathf.Abs(_playerPosition.position.x - transform.position.x) > attackDistance && !_attack)
        {
            if (_playerPosition.position.x < transform.position.x - attackDistance)
                _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            if (_playerPosition.position.x > transform.position.x + attackDistance)
                _rb.velocity = new Vector2(speed, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    private void Attack()
    {
        _attack = true;
        var countHit = Random.Range(1, 4);
        _animator.SetTrigger("Attack" + countHit);
        _soundForBoss.PlayAttackSound();
        RelaxationAfterAttack(countHit);
        _sinceAttack = 0;
        Debug.Log("Атака");
    }

    private void AttackCombo()
    {
        if (_attack) return;
        if (!_combo)
        {
            _comboAttack = 1;
            _soundForBoss.PlayComboSound();
        }
        _combo = true;
        _time += Time.deltaTime;
        if (_time >= kdAttackInCombo)
        {
            _attack = true;
            _animator.SetTrigger("Attack" + _comboAttack);
            Debug.Log("Комбо " + _comboAttack);
            Invoke(nameof(ComboAttackPlus), 0.5f);
            _time = 0;
            _attack = true;
        }

        if (_comboAttack == 4)
        {
            _comboAttack = 1;
            RelaxationAfterAttack(4);
            _combo = false;
        }
    }

    private void ComboAttackPlus()
    {
        _comboAttack++;
    }
    
    public float TakingDamage(float damageOnEnemy, Vector3 positionAttack, float pushPowerAttack)
    {
        if (death || _invincibility)
        {
            return 0;
        }
        
        //if (!death) _soundEnemy.PlayHurtSound();
        if (_rb == null || pushPower == 0) return 0;

        //if (_sinceAttack < 1f || _timeEndTakingDamage < 0)
        //{
        float dmg;
        if (health >= damageOnEnemy) dmg = damageOnEnemy;
        else dmg = health;
        health -= damageOnEnemy;
        lastTimeTakingDamage = 0;
        if (!death && !deathAnimation) Death();
        if (positionAttack.x >= transform.position.x)
        {
            _rb.AddForce(-transform.right * pushPowerAttack, ForceMode2D.Impulse);
        }
        else
        {
            _rb.AddForce(transform.right * pushPowerAttack, ForceMode2D.Impulse);
        }

        if (!_attack)
        {
            _animator.SetTrigger("Hurt");
            _soundForBoss.PlayHurtSound();
        }

        return dmg;
        //}
    }

    private void Death()
    {
        if (health > 0) return;
        //_soundEnemy.PlayDeathSound();
        _animator.SetTrigger("Death");
        deathAnimation = true;
        //healthBar.gameObject.SetActive(false);
    }

    private void RelaxationAfterAttack(int countHit)
    {
        switch (countHit)
        {
            case 1:
                _timeEndTakingDamage -= 1.5f;
                break;
            case 2:
                _timeEndTakingDamage -= 1.7f;
                break;
            case 3:
                _timeEndTakingDamage -= 1.9f;
                break;
            case 4:
                _timeEndTakingDamage -= 3f;
                break;
        }
    }

    private void AccessAnimator()
    {
        _animator.SetFloat("SpeedY", _rb.velocity.y);
        _animator.SetFloat("SpeedX", _rb.velocity.x);
        _animator.SetBool("Grounded", _onGround);
        
        if (_rb.velocity.x != 0) _animator.SetInteger("AnimState", 1);
        else _animator.SetInteger("AnimState", 0);
    }
    
    private void FlipX()
    {
        if (transform.position.x >= _playerPosition.position.x && !_faceRight ||
            transform.position.x <= _playerPosition.position.x && _faceRight) return;
        transform.localScale *= new Vector2(-1, 1);
        _faceRight = !_faceRight;
    }

    private void CheckGrounded()
    {
        //var change = _onGround;
        _onGround = Physics2D.OverlapCircle(groundCheckLeft.position, _groundCheckRadius, ground) || 
                    Physics2D.OverlapCircle(groundCheckRight.position, _groundCheckRadius, ground);

        //if (!change && _onGround) _soundForPlayer.PlayLanding();
    }
    
    public void SetNoAttack()
    {
        _attack = false;
    }
    
    public void SetDeath()
    {
        Invoke(nameof(SetDeathInvoke), 1f);
    }

    private void SetDeathInvoke(){
        death = true;
    }

    public void OnAttack()
    {
        var hitPlayers = Physics2D.OverlapAreaAll(
            attackCheckLeftUp.position,
            attackCheckRightDown.position,
            player);
        var i = 1;
        
        foreach (var playerHit in hitPlayers)
        {
            playerHit.GetComponent<Player>().TakingDamage(damage, transform.position, pushPower);
            if (_player.GetOnBlock()) i = 3;
            else
            {
                if (!_player.GetInvincibility()) i = 2;
            }
        }
        _soundForBoss.PlaySwordSound(i);
    }
}
