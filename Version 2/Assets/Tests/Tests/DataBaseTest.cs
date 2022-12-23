using System;
using System.Data;
using DataBase;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class DataBaseTest
{
    //Конечно, так лучше не делать, но тогда тесты получаются слишком большие
    private string _playerNickname = "testPlayer";
    private int _incomingDamage;
    private int _outcomingDamage;
    private int _killWhite;
    private int _killBlack;
    private int _killBoss;
    private int _rating;
    private int _countCompletions;
    
    public void DropAllTable()
    {
        MyDataBase.ExecuteQueryWithoutAnswer("DROP TABLE players");
        MyDataBase.ExecuteQueryWithoutAnswer("DROP TABLE completions");
        MyDataBase.ExecuteQueryWithoutAnswer("DROP TABLE killer_list");
        MyDataBase.ExecuteQueryWithoutAnswer("DROP TABLE player_info");
        MyDataBase.ExecuteQueryWithoutAnswer("DROP TABLE best_completion");
    }
    
    // Проверка, что таблицы правда сделались
    [Test]
    public void DataBaseTest1CreateTable()
    {
        DropAllTable();
        MyDataBase.CreateTables();
        var count1 = MyDataBase.ExecuteQueryWithAnswer("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='players';");
        var count2 = MyDataBase.ExecuteQueryWithAnswer("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='completions';");
        var count3 = MyDataBase.ExecuteQueryWithAnswer("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='killer_list';");
        var count4 = MyDataBase.ExecuteQueryWithAnswer("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='player_info';");
        var count5 = MyDataBase.ExecuteQueryWithAnswer("SELECT count(*) FROM sqlite_master WHERE type='table' AND name='best_completion';");
        Assert.AreEqual(Int32.Parse(count1), 1);
        Assert.AreEqual(Int32.Parse(count2), 1);
        Assert.AreEqual(Int32.Parse(count3), 1);
        Assert.AreEqual(Int32.Parse(count4), 1);
        Assert.AreEqual(Int32.Parse(count5), 1);
    }

    [Test]
    public void DataBaseTest2CheckNullCompletion()
    {
        var check = MyDataBase.GetTheFirstCompletion();
        Assert.IsFalse(check);
    }

    [Test]
    public void DataBaseTest3CheckCreateNullCompletion()
    {
        MyDataBase.CreateTheFirstCompletion();
        var checkNull = MyDataBase.GetTheFirstCompletion();
        var table = MyDataBase.GetTable("SELECT id_player, incoming_damage, outcoming_damage, kill_enemy, count_chest, rating, time FROM completions WHERE id_completion = 1");
        DataRow[] rows = table.Select();
        for(int i = 0; i < rows.Length ; i++)
        {
            Assert.AreEqual(rows[i]["id_player"].ToString(), "");
            Assert.AreEqual(Int32.Parse(rows[i]["incoming_damage"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rows[i]["outcoming_damage"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rows[i]["kill_enemy"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rows[i]["count_chest"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rows[i]["rating"].ToString()), 0);
            Assert.AreEqual(rows[i]["time"].ToString(), "0");
        }
        Assert.IsTrue(checkNull);
    }

    [Test]
    public void DataBaseTest4CheckPlayerInTable()
    {
        Assert.IsFalse(MyDataBase.CheckPlayerInBd("dsjindsjcndsc"));
    }

    [Test]
    public void DataBaseTest5RegisterAndCheckPlayerInTable()
    {
        MyDataBase.RegisterPlayer(_playerNickname);
        Assert.IsTrue(MyDataBase.CheckPlayerInBd(_playerNickname));

        var idPlayer = MyDataBase.GetIdPlayer(_playerNickname);
        
        var tableKiller =
            MyDataBase.GetTable("SELECT white_bandit_count, black_bandit_count, boss_count FROM killer_list WHERE id_player = " +
                                idPlayer);
        
        DataRow[] rowsKiller = tableKiller.Select();
        
        for (int i = 0; i < rowsKiller.Length ; i++)
        {
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["white_bandit_count"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["black_bandit_count"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["boss_count"].ToString()), 0);
        }
        
        
        var tableInfo =
            MyDataBase.GetTable(
                "SELECT count_completions, incoming_damage, outcoming_damage FROM player_info WHERE id_player = " +
                idPlayer);
        
        DataRow[] rowsInfo = tableInfo.Select();
        for(int i = 0; i < rowsInfo.Length ; i++)
        {
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["count_completions"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["incoming_damage"].ToString()), 0);
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["outcoming_damage"].ToString()), 0);
        }

        var idBestCompletion =
            MyDataBase.ExecuteQueryWithAnswer("SELECT id_completion FROM best_completion WHERE id_player = " +
                                              idPlayer);
        Assert.AreEqual(Int32.Parse(idBestCompletion), 1);
    }

    [Test]
    public void DataBaseTest6AddCompletionInTable()
    {
        var playerId = MyDataBase.GetIdPlayer(_playerNickname);
        
        _countCompletions = Random.Range(0, 10);
        for (var i = 0; i < _countCompletions; i++)
        {
            var inDam = Random.Range(0, 1000);
            var outDam = Random.Range(0, 1000);
            var kW = Random.Range(0, 10);
            var kB = Random.Range(0, 10);
            var kBoss = Random.Range(0, 2);
            var kChest = Random.Range(0, 10);
            var time = Random.Range(0, 100000);
            var rate = Random.Range(0, 1000);
            _incomingDamage += inDam;
            _outcomingDamage += outDam;
            _killWhite += kW;
            _killBlack += kB;
            _killBoss += kBoss;
            _rating = Math.Max(_rating, rate);
            var killEnemy = kB + kBoss + kW;

            MyDataBase.AddCompletionInBd(playerId, inDam, outDam, kW, kB, kBoss, rate, kChest, time);
            
            var count =
                MyDataBase.ExecuteQueryWithAnswer("SELECT COUNT(*) FROM completions WHERE (incoming_damage = " + inDam + ") AND " +
                                    "(outcoming_damage = " + outDam + ") AND (kill_enemy = " + killEnemy + ") AND (count_chest = " + kChest + ") AND " +
                                    "(id_player = " + playerId + ") AND (rating = " + rate + ");");
            
            Assert.AreEqual(Int32.Parse(count), 1);
        }
    }

    [Test]
    public void DataBaseTest7AddInKillerTable()
    {
        var playerId = MyDataBase.GetIdPlayer(_playerNickname);
        
        var tableKiller =
            MyDataBase.GetTable("SELECT white_bandit_count, black_bandit_count, boss_count FROM killer_list WHERE id_player = " +
                                playerId);
        DataRow[] rowsKiller = tableKiller.Select();
        
        for (int i = 0; i < rowsKiller.Length ; i++)
        {
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["white_bandit_count"].ToString()), _killWhite);
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["black_bandit_count"].ToString()), _killBlack);
            Assert.AreEqual(Int32.Parse(rowsKiller[i]["boss_count"].ToString()), _killBoss);
        }
    }

    [Test]
    public void DataBaseTest8AddInInfoTable()
    {
        var playerId = MyDataBase.GetIdPlayer(_playerNickname);
        
        var tableInfo =
            MyDataBase.GetTable(
                "SELECT count_completions, incoming_damage, outcoming_damage FROM player_info WHERE id_player = " +
                playerId);
        
        DataRow[] rowsInfo = tableInfo.Select();
        for (int i = 0; i < rowsInfo.Length ; i++)
        {
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["count_completions"].ToString()), _countCompletions);
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["incoming_damage"].ToString()), _incomingDamage);
            Assert.AreEqual(Int32.Parse(rowsInfo[i]["outcoming_damage"].ToString()), _outcomingDamage);
        }
    }

    [Test]
    public void DataBaseTest9CheckAboutPlayer()
    {
        var element = MyDataBase.GetInfoPlayer(MyDataBase.GetIdPlayer(_playerNickname));
        Assert.AreEqual(element.nickname, _playerNickname);
        Assert.AreEqual(element.countCompletions, _countCompletions);
        Assert.AreEqual(element.idPlayer, MyDataBase.GetIdPlayer(_playerNickname));
        Assert.AreEqual(element.incomingDamage, _incomingDamage);
        Assert.AreEqual(element.maxRating, _rating);
        Assert.AreEqual(element.outcomingDamage, _outcomingDamage);
        Assert.AreEqual(element.countBossKill, _killBoss);
        Assert.AreEqual(element.countBlackBanditKill, _killBlack);
        Assert.AreEqual(element.countWhiteBanditKill, _killWhite);
    }
}
