using TMPro;

public class Element
{
    public int idPlayer;
    public string nickname;
    //public int killEnemy;
    public int incomingDamage;
    public int outcomingDamage;
    public int countCompletions;
    public int countWhiteBanditKill;
    public int countBlackBanditKill;
    public int countBossKill;
    public int maxRating;

    public Element(int idPlayer, string nickname, int incomingDamage, int outcomingDamage,
        int countCompletions,
        int countWhiteBanditKill, int countBlackBanditKill, int countBossKill, int maxRating)
    {
        this.nickname = nickname;
        this.countCompletions = countCompletions;
        this.idPlayer = idPlayer;
        this.incomingDamage = incomingDamage;
        //this.killEnemy = killEnemy;
        this.maxRating = maxRating;
        this.outcomingDamage = outcomingDamage;
        this.countBossKill = countBossKill;
        this.countBlackBanditKill = countBlackBanditKill;
        this.countWhiteBanditKill = countWhiteBanditKill;
    }
}
