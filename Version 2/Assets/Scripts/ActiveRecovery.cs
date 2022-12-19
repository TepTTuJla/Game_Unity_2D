using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveRecovery : MonoBehaviour
{
    public List<GameObject> listGameObjects;
    private List<Enemy> _listEnemy = new List<Enemy>();
    public Transform leftUpPoint;
    public Transform rightDownPoint;
    public GameObject box;
    public LayerMask player;
    private bool _start;
    private bool _end;
    private float _time;

    private void Start()
    {
        foreach (var enemy in listGameObjects)
        {
            _listEnemy.Add(enemy.GetComponent<Enemy>());
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        var hitPlayer = Physics2D.OverlapAreaAll(
            leftUpPoint.position,
            rightDownPoint.position,
            player
        );

        if (!_start)
        {
            if (hitPlayer.Length != 0)
            {
                _time = 0;
                ActivateEnemy();
                ActivateBox();
                _start = true;
            }
        }

        if (_start)
        {
            foreach (var enemy in _listEnemy)
            {
                enemy.SetNoIdle();
            }
            _end = CheckDeathEnemy();
        }

        if (_end && _time >= 6f)
        {
            DeactivateBox();
        }
    }

    private void ActivateEnemy()
    {
        foreach (var enemy in listGameObjects)
        {
            enemy.GetComponent<SetDeathEnemyFromStart>().RecoveryIfPlayerClose();
        }
    }

    private void ActivateBox()
    {
        box.GetComponent<BoxCollider2D>().isTrigger = false;
        box.layer = 6;
    }

    private void DeactivateBox()
    {
        box.GetComponent<BoxCollider2D>().isTrigger = true;
        box.layer = 0;
    }

    private bool CheckDeathEnemy()
    {
        var count = 0;
        foreach (var enemy in _listEnemy)
        {
            if (enemy.death) count++;
        }

        return count == _listEnemy.Count;
    }
}
