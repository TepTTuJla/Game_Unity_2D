using System.Collections;
using System.Collections.Generic;
using DataBase;
using TMPro;
using UnityEngine;

public class AboutPlayer : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI countEnemies;
    public TextMeshProUGUI countWhite;
    public TextMeshProUGUI countBlack;
    public TextMeshProUGUI countBoss;
    public TextMeshProUGUI incomingDamage;
    public TextMeshProUGUI outcomingDamage;
    public TextMeshProUGUI countCompletions;
    public TextMeshProUGUI maxRating;

    public void Open()
    {
        Element el = MyDataBase.GetInfoPlayer(SqlScript.idPlayer);
        nickname.text = el.nickname;
        //countEnemies.text = el.killEnemy.ToString();
        countBlack.text = el.countBlackBanditKill.ToString();
        countWhite.text = el.countWhiteBanditKill.ToString();
        countBoss.text = el.countBossKill.ToString();
        incomingDamage.text = el.incomingDamage.ToString();
        outcomingDamage.text = el.outcomingDamage.ToString();
        countCompletions.text = el.countCompletions.ToString();
        maxRating.text = el.maxRating.ToString();
    }
}
