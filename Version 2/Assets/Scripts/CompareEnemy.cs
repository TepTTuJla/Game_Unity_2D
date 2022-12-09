using UnityEngine;

public class CompareEnemy : MonoBehaviour
{
    private void Start()
    {
        var idCurrent = 1;
        var arrayEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in arrayEnemies)
        {
            enemy.GetComponent<Enemy>().SetId(idCurrent);
            idCurrent++;
        }
    }
}
