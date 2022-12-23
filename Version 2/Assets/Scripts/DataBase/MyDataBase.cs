using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;
using static System.Int32;

namespace DataBase
{
    public class MyDataBase
    {
        private static SqliteConnection _dbConnection;
        private const string _fileName = "data.bytes";
        private static string _DBPath;
        private static string _path;
        public static string check;
        private static SqliteCommand _command;

        static MyDataBase()
        {
            _DBPath = GetDatabasePath();
            _path = "Data Source=" + _DBPath;
            Debug.Log(Application.streamingAssetsPath);
        }

        private static string GetDatabasePath()
        {
            var str = "";
#if (UNITY_EDITOR)
            check = "Unity_Editor";
            str = Path.Combine(Application.streamingAssetsPath, _fileName);
#endif
            if (str != "") return str;
#if (UNITY_STANDALONE_WIN)
            check = "Windows";
            str = Path.Combine(Application.dataPath, _fileName);
            if(!File.Exists(str)) UnpackDatabase(str);
#endif
            return str;
        }

        private static void UnpackDatabase(string toPath)
        {
            string fromPath = Path.Combine(Application.streamingAssetsPath, _fileName);

            WWW reader = new WWW(fromPath);
            while (!reader.isDone)
            {
            }

            File.WriteAllBytes(toPath, reader.bytes);
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
        
        public static DataTable GetTable(string query)
        {
            OpenConnection();

            var adapter = new SqliteDataAdapter(query, _dbConnection);

            var ds = new DataSet();
            adapter.Fill(ds);
            adapter.Dispose();

            CloseConnection();

            return ds.Tables[0];
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

        //Проверяет, есть ли нулевое прохождение (нулевое прохождение - прохождение, где было сделано ничего)
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

        //Создаёт нулевое прохождение, если его нет
        public static void CreateTheFirstCompletion()
        {
            if (GetTheFirstCompletion()) return;
            ExecuteQueryWithoutAnswer(
                "INSERT INTO completions (incoming_damage, outcoming_damage, kill_enemy, count_chest, rating, time) " +
                "VALUES (0, 0, 0, 0, 0, '0');");
        }

        //Проверка, что игрок есть в таблице
        public static bool CheckPlayerInBd(string nickname)
        {
            var check = false;
            OpenConnection();

            var sqlCommand = "SELECT id_player FROM players WHERE nickname = @nickname;";
            SqliteParameter nick = new SqliteParameter("@nickname", nickname);
            _command.CommandText = sqlCommand;
            _command.Parameters.Add(nick);

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
            OpenConnection();
            var sqlCommand = "SELECT id_player FROM players WHERE nickname = @nickname;";
            SqliteParameter nick = new SqliteParameter("@nickname", nickname);
            _command.CommandText = sqlCommand;
            _command.Parameters.Add(nick);
            id = Parse(_command.ExecuteScalar().ToString());
            CloseConnection();

            return id;
        }

        //Регистрация игроков
        public static void RegisterPlayer(string nickname)
        {
            OpenConnection();
            var sqlCommand = "INSERT INTO players (nickname) " +
                             "VALUES (@nickname);";
            SqliteParameter idPara = new SqliteParameter("@nickname", nickname);
            _command.CommandText = sqlCommand;
            _command.Parameters.Add(idPara);
            _command.ExecuteNonQuery();
            CloseConnection();
            var id = GetIdPlayer(nickname);

            sqlCommand = 
                "INSERT INTO killer_list (id_player, white_bandit_count, black_bandit_count, boss_count) " +
                "VALUES (@id, 0, 0, 0);";
            OpenConnection();
            _command.CommandText = sqlCommand;
            SqliteParameter idParam = new SqliteParameter("@id", id);
            _command.Parameters.Add(idParam);
            _command.ExecuteNonQuery();
            CloseConnection();

            sqlCommand = 
                "INSERT INTO player_info (id_player, count_completions, incoming_damage, outcoming_damage) " +
                "VALUES (@id, 0, 0, 0);";
            OpenConnection();
            _command.CommandText = sqlCommand;
            idParam = new SqliteParameter("@id", id);
            _command.Parameters.Add(idParam);
            _command.ExecuteNonQuery();
            CloseConnection();

            sqlCommand = "INSERT INTO best_completion (id_player, id_completion) " +
                                      "VALUES (@id, 1);";
            OpenConnection();
            _command.CommandText = sqlCommand;
            idParam = new SqliteParameter("@id", id);
            _command.Parameters.Add(idParam);
            _command.ExecuteNonQuery();
            CloseConnection();
        }

        //Добавление информации о прохождении в таблицу
        public static void AddCompletionInBd(int idPlayer, int incomingDamage, int outcomingDamage, int killWhiteBandit,
            int killBlackBandit,
            int killBoss, int rating, int countChest, float time)
        {
            var killEnemy = killBlackBandit + killWhiteBandit + killBoss;
            if (incomingDamage < 0) incomingDamage = 0;
            if (outcomingDamage < 0) outcomingDamage = 0;
            if (killBlackBandit < 0) killBlackBandit = 0;
            if (killWhiteBandit < 0) killWhiteBandit = 0;
            if (killBoss < 0) killBoss = 0;
            if (time < 0) time = 0;
            if (rating < 0) rating = 0;
            if (countChest < 0) countChest = 0;
            var seconds = time % 60;
            var minutes = (time - seconds) / 60;
            var timeStr = String.Format(Math.Round(minutes, 0) + " Minute " + Math.Round(seconds, 0) + " Seconds");
            var sqlCommand = 
                "INSERT INTO completions (id_player, incoming_damage, outcoming_damage, kill_enemy, count_chest, rating, time) " +
                "VALUES (@id, @incoming, @outcoming, @killEnemy, @countChest, @rating, @time);";
            OpenConnection();
            _command.CommandText = sqlCommand;
            var parameter = new SqliteParameter("@id", idPlayer);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@incoming", incomingDamage);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@outcoming", outcomingDamage);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@killEnemy", killEnemy);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@countChest", countChest);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@rating", rating);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@time", timeStr);
            _command.Parameters.Add(parameter);
            _command.ExecuteNonQuery();
            CloseConnection();
            
            sqlCommand = "UPDATE killer_list " +
                                      "SET white_bandit_count = white_bandit_count + @killWhite, " +
                                      "black_bandit_count = black_bandit_count + @killBlack, " +
                                      "boss_count = boss_count + @killBoss " +
                                      "WHERE id_player = @id;";
            OpenConnection();
            parameter = new SqliteParameter("@id", idPlayer);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@killWhite", killWhiteBandit);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@killBlack", killBlackBandit);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@killBoss", killBoss);
            _command.Parameters.Add(parameter);
            _command.CommandText = sqlCommand;
            _command.ExecuteNonQuery();
            CloseConnection();

            sqlCommand = "UPDATE player_info " +
                                      "SET count_completions = count_completions + 1, " +
                                      "incoming_damage = incoming_damage + @incoming, " +
                                      "outcoming_damage = outcoming_damage + @outcoming " +
                                      "WHERE id_player = @id;";
            OpenConnection();
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@id", idPlayer);
            _command.Parameters.Add(parameter);
            parameter = new SqliteParameter("@incoming", incomingDamage);
            _command.Parameters.Add(parameter);
            parameter = new SqliteParameter("@outcoming", outcomingDamage);
            _command.Parameters.Add(parameter);
            _command.ExecuteNonQuery();
            CloseConnection();

            sqlCommand = "SELECT id_completion FROM completions " +
                                                         "WHERE id_player = @id " +
                                                         "ORDER BY rating DESC " +
                                                         "LIMIT 1;";
            OpenConnection();
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@id", idPlayer);
            _command.Parameters.Add(parameter);
            var bestCompetition = Parse(_command.ExecuteScalar().ToString());
            CloseConnection();

            sqlCommand = "UPDATE best_completion " +
                                      "SET id_completion = @best " +
                                      "WHERE id_player = @id;";
            OpenConnection();
            _command.CommandText = sqlCommand;
            parameter = new SqliteParameter("@id", idPlayer);
            _command.Parameters.Add(parameter);
            parameter = new SqliteParameter("@best", bestCompetition);
            _command.Parameters.Add(parameter);
            _command.ExecuteNonQuery();
            CloseConnection();
        }

        //Получение информации об игроке
        public static Element GetInfoPlayer(int idPlayers)
        {
            
            OpenConnection();
            var sqlCommand2 = "SELECT count_completions FROM player_info " +
                              "WHERE id_player = @id;";
            _command.CommandText = sqlCommand2;
            SqliteParameter idPara = new SqliteParameter("@id", idPlayers);
            _command.Parameters.Add(idPara);
            
            var maxRating = 0;
            var countCompletion = Parse(_command.ExecuteScalar().ToString());
            CloseConnection();
            
            if (countCompletion != 0)
            {
                OpenConnection();
                var sqlCommand1 = "SELECT rating FROM completions " +
                                 "WHERE id_player = @id " +
                                 "ORDER BY rating DESC " +
                                 "LIMIT 1;";
                _command.CommandText = sqlCommand1;
                SqliteParameter idPar = new SqliteParameter("@id", idPlayers.ToString());
                _command.Parameters.Add(idPar);
                maxRating = Parse(_command.ExecuteScalar().ToString());
                CloseConnection();
            }

            var incomingDamage = 0;
            var nickname = "";
            var outcomingDamage = 0;
            var countWhiteBanditKill = 0;
            var countBlackBanditKill = 0;
            var countBossKill = 0;
            var countCompletions = 0;

            OpenConnection();

            var sqlCommand =
                "SELECT nickname, white_bandit_count, black_bandit_count, boss_count, count_completions, incoming_damage, outcoming_damage FROM players " +
                "JOIN killer_list ON players.id_player = killer_list.id_player " +
                "JOIN player_info ON player_info.id_player = players.id_player " +
                "WHERE players.id_player = @id;";
            _command.CommandText = sqlCommand;
            SqliteParameter idParametr = new SqliteParameter("@id", idPlayers.ToString());
            _command.Parameters.Add(idParametr);
            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    incomingDamage = Parse(reader["incoming_damage"].ToString());
                    nickname = reader["nickname"].ToString();
                    outcomingDamage = Parse(reader["outcoming_damage"].ToString());
                    countWhiteBanditKill = Parse(reader["white_bandit_count"].ToString());
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
