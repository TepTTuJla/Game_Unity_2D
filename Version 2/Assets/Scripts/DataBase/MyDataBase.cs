using System;
using UnityEngine;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using static System.Int32;

namespace DataBase
{
    public class MyDataBase
    {
        private static SqliteConnection _dbConnection;
        private static string _path;
        private static SqliteCommand _command;

        static MyDataBase()
        {
            var dir = @"C:\DataBase";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _path = @"Data Source=C:\DataBase/data.db;Version=3";
        }

        private static void OpenConnection()
        {
            _dbConnection = new SqliteConnection(_path);
            _dbConnection.Open();
            _command = _dbConnection.CreateCommand();
        }

        //Закрывает подключение к БД.
        private static void CloseConnection()
        {
            _dbConnection.Close();
        }
        
        //Выполняет запрос
        public static void ExecuteQueryWithoutAnswer(string query)
        {
            OpenConnection();
            _command.CommandText = query;
            var i = _command.ExecuteNonQuery();
            CloseConnection();
        }

        //Возвращает строку - ответ на запрос
        public static string ExecuteQueryWithAnswer(string query)
        {
            try
            {
                CloseConnection();
                OpenConnection();
                _command.CommandText = query;
                var answer = _command.ExecuteScalar();
                CloseConnection();
                if (answer != null) return answer.ToString();
                return null;
            }
            catch (SqliteException)
            {
                CloseConnection();
                return null;
            }
        }

        //Создание таблиц
        public static void CreateTables()
        {
            ExecuteQueryWithoutAnswer("CREATE TABLE IF NOT EXISTS players (" +
                                      "id_player INTEGER PRIMARY KEY AUTOINCREMENT," +
                                      "nickname TEXT NOT NULL" +
                                      ");");

            ExecuteQueryWithoutAnswer("CREATE TABLE IF NOT EXISTS completions (" +
                                      "id_completion INTEGER PRIMARY KEY AUTOINCREMENT," +
                                      "id_player INTEGER," +
                                      "incoming_damage INTEGER DEFAULT 0," +
                                      "outcoming_damage INTEGER DEFAULT 0," +
                                      "kill_enemy INTEGER DEFAULT 0," +
                                      "count_chest INTEGER DEFAULT 0," +
                                      "rating INTEGER DEFAULT 0," +
                                      "count_completion INTEGER DEFAULT 0," +
                                      "time TEXT DEFAULT 0," +
                                      "FOREIGN KEY (id_player) REFERENCES players (id_player)" +
                                      ");");

            ExecuteQueryWithoutAnswer("CREATE TABLE IF NOT EXISTS killer_list (" +
                                      "id_player INTEGER," +
                                      "white_bandit_count INTEGER DEFAULT 0," +
                                      "black_bandit_count INTEGER DEFAULT 0," +
                                      "boss_count INTEGER DEFAULT 0," +
                                      "FOREIGN KEY (id_player) REFERENCES players (id_player)" +
                                      ");");

            ExecuteQueryWithoutAnswer("CREATE TABLE IF NOT EXISTS player_info (" +
                                      "id_player INTEGER," +
                                      "count_completions INTEGER," +
                                      "incoming_damage INTEGER," +
                                      "outcoming_damage INTEGER" +
                                      ");");

            ExecuteQueryWithoutAnswer("CREATE TABLE IF NOT EXISTS best_completion (" +
                                      "id_player INTEGER," +
                                      "id_completion INTEGER," +
                                      "FOREIGN KEY (id_player) REFERENCES players (id_player)," +
                                      "FOREIGN KEY (id_completion) REFERENCES completions (id_completion)" +
                                      ");");
        }

        public static bool GetTheFirstCompletion()
        {
            var check = false;
            OpenConnection();

            var sqlCommand = "SELECT rating FROM completions WHERE id_completion = 1;";
            
            _command.CommandText = sqlCommand;
            using (var reader = _command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    check = true;
                }

                reader.Close();
            }

            CloseConnection();

            return check;
        }

        public static void CreateTheFirstCompletion()
        {
            if (GetTheFirstCompletion()) return;
            ExecuteQueryWithoutAnswer("INSERT INTO completions (incoming_damage, outcoming_damage, kill_enemy, count_chest, rating, time) " +
                                      "VALUES (0, 0, 0, 0, 0, '0');");
        }
        
        //Проверка, что игрок есть в таблице
        public static bool CheckPlayerInBd(string nickname)
        {
            var check = false;
            OpenConnection();

            var sqlCommand = "SELECT id_player FROM players WHERE nickname = '" + nickname + "';";
            
            _command.CommandText = sqlCommand;
            using (var reader = _command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    check = true;
                }

                reader.Close();
            }

            CloseConnection();

            return check;
        }

        //Вовзвращает id игрока
        public static int GetIdPlayer(string nickname)
        {
            var id = 0;
            using (var connection = new SqliteConnection(_path))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id_player FROM players WHERE nickname = '" + nickname + "';";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = Parse(reader["id_player"].ToString());
                        }

                        reader.Close();
                    }
                }

                connection.Close();
            }

            return id;
        }

        //Регистрация игроков
        public static void RegisterPlayer(string nickname)
        {
            ExecuteQueryWithoutAnswer("INSERT INTO players (nickname) " +
                                      "VALUES ('" + nickname + "');");
            
            var id = GetIdPlayer(nickname);
            
            ExecuteQueryWithoutAnswer("INSERT INTO killer_list (id_player, white_bandit_count, black_bandit_count, boss_count) " +
                                      "VALUES (" + id + ", 0, 0, 0);");
            
            ExecuteQueryWithoutAnswer("INSERT INTO player_info (id_player, count_completions, incoming_damage, outcoming_damage) " +
                                      "VALUES (" + id + ", 0, 0, 0);");
            
            ExecuteQueryWithoutAnswer("INSERT INTO best_completion (id_player, id_completion) " +
                                      "VALUES (" + id + ", 1);");
        }
        
        //Добавление информации о прохождении в таблицу
        public static void AddCompletionInBd(int idPlayer, int incomingDamage, int outcomingDamage, int killWhiteBandit, int killBlackBandit,
            int killBoss, int rating, int countChest, float time)
        {
            var killEnemy = killBlackBandit + killWhiteBandit + killBoss;
            var seconds = time % 60;
            var minutes = (time - seconds) / 60;
            var timeStr = String.Format(Math.Round(minutes, 0) + " Minute " + Math.Round(seconds, 0) + " Seconds");
            ExecuteQueryWithoutAnswer("INSERT INTO completions (id_player, incoming_damage, outcoming_damage, kill_enemy, count_chest, rating, time) " +
                                      "VALUES ( " + idPlayer + ", " + incomingDamage +", " + outcomingDamage + ", " + killEnemy + ", " + countChest + ", " + rating + ", '" + timeStr + "');");

            ExecuteQueryWithoutAnswer("UPDATE killer_list " +
                                      "SET white_bandit_count = white_bandit_count + " + killWhiteBandit + ", " +
                                      "black_bandit_count = black_bandit_count + " + killBlackBandit + ", " +
                                      "boss_count = boss_count + " + killBoss + " " +
                                      "WHERE id_player = " + idPlayer + "; ");

            ExecuteQueryWithoutAnswer("UPDATE player_info " +
                                      "SET count_completions = count_completions + 1, " +
                                      "incoming_damage = incoming_damage + " + incomingDamage + ", " +
                                      "outcoming_damage = outcoming_damage + " + outcomingDamage + " " +
                                      "WHERE id_player = " + idPlayer + "; ");

            var bestCompetition = ExecuteQueryWithAnswer("SELECT id_completion FROM completions " +
                                                         "WHERE id_player = " + idPlayer + " " +
                                                         "ORDER BY rating DESC " +
                                                         "LIMIT 1;");
            
            var best = Parse(bestCompetition);
            ExecuteQueryWithoutAnswer("UPDATE best_completion " +
                                      "SET id_completion = " + best + " " +
                                      "WHERE id_player = " + idPlayer + ";");
        }
        
        //Получение списка топ 7 игроков и данного
        // public static List<Element> GetList(int idPlayer)
        // {
        //     //List<Element> list = new List<Element>();
        //
        //     var rating = Parse(ExecuteQueryWithAnswer(
        //         "SELECT id_player, nickname, time, kill_enemy, rating FROM players " +
        //         "JOIN best_completion ON players.id_player = best_completion.id_player " +
        //         "JOIN completions ON completions.id_completion = best_completion.id_completion "
        //         ));
        //     
        //     OpenConnection();
        //     
        //     var sqlCommand = "SELECT id_player, nickname, time, kill_enemy, rating FROM players " +
        //                      "JOIN best_completion ON players.id_player = best_completion.id_player, " +
        //                      "JOIN completions ON completions.id_completion = best_completion.id_completion " +
        //                      "GROUP BY rating DESC " +
        //                      "LIMIT 7;" ;
        //     _command.CommandText = sqlCommand;
        //     using (var reader = _command.ExecuteReader())
        //     {
        //         while (reader.Read())
        //         {
        //             /*var el = new Element();
        //             el.id = Parse(reader["id_player"].ToString());
        //             el.nickname = reader["nickname"].ToString();
        //             el.rating = Parse(reader["rating"].ToString());
        //             el.time = reader["time"].ToString();
        //             el.killEnemy = Parse(reader["kill_enemy"].ToString());
        //             list.Add(el);*/
        //         }
        //         reader.Close();
        //     }
        //
        //     //return list;
        // }

        //Получение информации об игроке
        public static Element GetInfoPlayer(int idPlayers)
        {
            var countCompletion = Parse(ExecuteQueryWithAnswer(
                "SELECT count_completions FROM player_info " +
                "WHERE id_player = " + idPlayers + " ;"));
            var maxRating = 0;
            if (countCompletion != 0) maxRating = Parse(ExecuteQueryWithAnswer(
                "SELECT rating FROM completions " +
                "WHERE id_player = " + idPlayers + " " +
                "ORDER BY rating DESC " +
                "LIMIT 1;"
            ));
            var incomingDamage = 0;
            var nickname = "";
            var outcomingDamage = 0;
            var countWhiteBanditKill = 0;
            //var killEnemy = 0;
            var countBlackBanditKill = 0;
            var countBossKill = 0;
            var countCompletions = 0;
            
            OpenConnection();

            var sqlCommand =
                "SELECT nickname, white_bandit_count, black_bandit_count, boss_count, count_completions, incoming_damage, outcoming_damage FROM players " +
                "JOIN killer_list ON players.id_player = killer_list.id_player " +
                "JOIN player_info ON player_info.id_player = players.id_player;";
            _command.CommandText = sqlCommand;
            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    incomingDamage = Parse(reader["incoming_damage"].ToString());
                    nickname = reader["nickname"].ToString();
                    outcomingDamage = Parse(reader["outcoming_damage"].ToString());
                    countWhiteBanditKill = Parse(reader["white_bandit_count"].ToString());
                    //killEnemy = Parse(reader["kill_enemy"].ToString());
                    countBlackBanditKill = Parse(reader["black_bandit_count"].ToString());
                    countBossKill = Parse(reader["boss_count"].ToString());
                    countCompletions = Parse(reader["count_completions"].ToString());
                }
                reader.Close();
            }
            CloseConnection();
            var el = new Element(idPlayers, nickname, incomingDamage, outcomingDamage,
                countCompletions, countWhiteBanditKill, countBlackBanditKill, countBossKill, maxRating);
            return el;
        }
    }
}
