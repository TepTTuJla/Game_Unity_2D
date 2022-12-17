using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeactivateAttackIdleForEnemies : MonoBehaviour
{
    public Transform pointLeftDown;
    public Transform pointRightUp;
    private List<Enemy> _listEnemies;
    private Boss _boss;
    public LayerMask player;
    public Transform pointActiveBoss;
    private Transform _playerPosition;
    private bool _bossActive;

    public Image healthGreen;
    public Image healthRed;
    public GameObject healthBoss;
    private float _maxHealth = 200f;
    private float _currentHealth;
    public float speed = 0.1f;

    private void Awake()
    {
        healthBoss.gameObject.SetActive(false);
    }
    private void Start()
    {
        _listEnemies = new List<Enemy>();
        var listEnemiesGameObject = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in listEnemiesGameObject)
        {
            _listEnemies.Add(enemy.GetComponent<Enemy>());
        }
        _boss = GameObject.FindWithTag("Boss").GetComponent<Boss>();
        _playerPosition = GameObject.FindWithTag("Player").GetComponent<Transform>();

        _maxHealth = _boss.maxHealth;
    }
    
    private void Update()
    {
        if (CheckPlayerWithBoss())
        {
            DeactivateAttackIdle();
        }

        if (CanActiveBoss())
        {
            ActivateBoss();
        }

        if (_bossActive)
        {
            _currentHealth = _boss.health;
            if (_currentHealth < 0) _currentHealth = 0;
            
                healthRed.fillAmount = Mathf.Lerp(healthRed.fillAmount, _currentHealth / _maxHealth, speed);
               
            healthGreen.fillAmount = _currentHealth / _maxHealth;
        }
    }

    private bool CheckPlayerWithBoss()
    {
        var area = Physics2D.
            OverlapAreaAll(pointLeftDown.position, pointRightUp.position, player);
        return area.Length != 0;
    }

    private bool CanActiveBoss()
    {
        return Mathf.Abs(pointActiveBoss.position.x - _playerPosition.position.x) <= 1f;
    }

    private void DeactivateAttackIdle()
    {
        foreach (var enemy in _listEnemies)
        {
            if (enemy) enemy.SetIdle();
        }
    }

    private void ActivateBoss()
    {
        _boss.active = true;
        _bossActive = true;
        healthBoss.gameObject.SetActive(true);
    }
}
