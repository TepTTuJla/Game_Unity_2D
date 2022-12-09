using UnityEngine;
using UnityEngine.UI;

public class HeathPlayer : MonoBehaviour
{
    public Image healthGreen;
    public Image healthRed;

    public Image rageBar;
    
    private float _maxHealth = 100f;
    private float _currentHealth;

    private float _maxRage = 30f;
    private float _currentRage;

    public float speed = 0.1f;
    public float timeChange = 2f;

    private Player _player;

    private void Update()
    {
        _currentHealth = _player.health;
        if (_currentHealth < 0) _currentHealth = 0;
        ChangeValue();
        healthGreen.fillAmount = _currentHealth / _maxHealth;

        _currentRage = _player.rage;
        rageBar.fillAmount = _currentRage / _maxRage;

        ActiveRage();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _maxHealth = _player.maxHealth;
        _maxRage = _player.maxRage;
    }

    private void ChangeValue()
    {
        //if (_player.lastTimeTakingDamage >= timeChange || _player.deathAnimation)
        //{
            healthRed.fillAmount = Mathf.Lerp(healthRed.fillAmount, _currentHealth / _maxHealth, speed);
        //}
    }

    private void ActiveRage(){
        if (_player.onRage){
            rageBar.color = new Color(245f, 0f, 45f);
        }
        else rageBar.color = new Color(255f, 255f, 255f);
    }
}
