using System.Collections.Generic;
using UnityEngine;

public class ActiveAttackIdleForEnemy : MonoBehaviour
{
    public List<GameObject> list;
    private Enemy _enemy;
    
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }
    private void Update()
    {
        if (!_enemy.GetIdle())
        {
            foreach (var enemy in list)
            {
                if (enemy) enemy.GetComponent<Enemy>().SetNoIdle();
            }
        }
        else
        {
            foreach (var enemy in list)
            {
                if (enemy) enemy.GetComponent<Enemy>().SetIdle();
            }
        }
    }
}
