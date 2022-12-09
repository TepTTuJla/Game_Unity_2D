using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum Type
    {
        Wooden = 0,
        Iron = 1,
        Silver = 2,
        Golden = 3
    }
    public Type type;

    private Player _player;
    private Animator _animator;

    private void Awake() {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    public void OpenChest(){
        _animator.SetTrigger("Open");
        ApplicationBuff();
    }

    private void ApplicationBuff()
    {
        switch (type)
        {
            case Type.Golden:
                GoldenChest();
                break;
            case Type.Silver:
                SilverChest();
                break;
            case Type.Iron:
                IronChest();
                break;
            default: 
                WoodenChest();
                break;
        }
    }

    private void GoldenChest()
    {
        var probability = RandomBuffInChest();
        switch (probability)
        {
            case <= 25:
                _player.SetIncreaseDamage(1.5f, true);
                break;
            case <= 30:
                _player.SetPowerPunch(1.35f);
                break;
            case <= 60:
                _player.SetRecoveryHealth(70f);
                break;
            case <= 80:
                _player.SetRecoveryRage(10f);
                break;
            default:
                _player.SetRecoveryAfterDeath();
                break;
        }
    }

    private void SilverChest()
    {
        var probability = RandomBuffInChest();
        switch (probability)
        {
            case <= 20:
                _player.SetIncreaseDamage(2f, false);
                break;
            case <= 35:
                _player.SetPowerPunch(1.2f);
                break;
            case <= 70:
                _player.SetRecoveryHealth(40f);
                break;
            case <= 95:
                _player.SetRecoveryRage(8f);
                break;
            default:
                _player.SetRecoveryAfterDeath();
                break;
        }
    }

    private void IronChest()
    {
        var probability = RandomBuffInChest();
        switch (probability)
        {
            case <= 30:
                _player.SetIncreaseDamage(1.7f, false);
                break;
            case <= 40:
                _player.SetPowerPunch(1.07f);
                break;
            case <= 75:
                _player.SetRecoveryHealth(30f);
                break;
            default:
                _player.SetRecoveryRage(6.5f);
                break;
        }
    }

    private void WoodenChest()
    {
        var probability = RandomBuffInChest();
        switch (probability)
        {
            case <= 30:
                _player.SetIncreaseDamage(1.5f, false);
                break;
            case <= 70:
                _player.SetRecoveryHealth(20f);
                break;
            default:
                _player.SetRecoveryRage(5f);
                break;
        }
    }

    private int RandomBuffInChest()
    {
        return Random.Range(0, 101);
    }
}
