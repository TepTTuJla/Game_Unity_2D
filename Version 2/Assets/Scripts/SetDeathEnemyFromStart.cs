using UnityEngine;

public class SetDeathEnemyFromStart : MonoBehaviour
{
    private Enemy _enemy;
    public bool isDead;
    private void Awake()
    {
        if (isDead)
        {
            _enemy = GetComponent<Enemy>();
        }
    }

    private void Start()
    {
        _enemy.TakingDamage(_enemy.health, _enemy.transform.position, 0.1f);
    }

    public void RecoveryIfPlayerClose()
    {
        isDead = false;
        _enemy.Regeneration();
    }
}
