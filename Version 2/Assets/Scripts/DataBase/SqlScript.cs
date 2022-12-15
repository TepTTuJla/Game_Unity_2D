using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataBase
{
    public class SqlScript : MonoBehaviour
    {
        public static int idPlayer;
        public TextMeshProUGUI textId;
        public GameObject textAuthNickname;
        private TMP_InputField _nickname;
        public GameObject textAuthPassword;
        private TMP_InputField _password;

        public GameObject textRegNickname;
        private TMP_InputField _nicknameReg;
        public GameObject textRegPassword;
        private TMP_InputField _passwordReg;

        public Button playButton;
        public Button ratingButton;

        private void Start()
        {
            _nickname = textAuthNickname.GetComponent<TMP_InputField>();
            _password = textAuthPassword.GetComponent<TMP_InputField>();
            _nicknameReg = textRegNickname.GetComponent<TMP_InputField>();
            _passwordReg = textRegPassword.GetComponent<TMP_InputField>();
            MyDataBase.CreateTables();
            MyDataBase.CreateTheFirstCompletion();
        }

        public void Authentication()
        {
            var nickname = _nickname.text;
            var password = _password.text;

            if (CheckSpace(nickname) || CheckSpace(password))
            {
                Debug.Log("В строке есть пробелы");
                return;
            }

            if (!MyDataBase.CheckPlayerInBd(nickname))
            {
                Debug.Log("Такого игрока нет в БД");
                return;
            }

            if (MyDataBase.CheckPasswordPlayer(nickname, password))
            {
                Debug.Log("Вход выполнен");
                idPlayer = MyDataBase.GetIdPlayer(nickname);
                textId.text = idPlayer.ToString();
                playButton.interactable = true;
                ratingButton.interactable = true;
            }
            else
            {
                Debug.Log("Ошибка пароля");
            }
        }

        private bool CheckSpace(string str)
        {
            var pattern = @"^\S+\s";
            return Regex.IsMatch(str, pattern);
        }

        public void Registration()
        {
            var nickname = _nicknameReg.text;
            var password = _passwordReg.text;

            if (CheckSpace(nickname) || CheckSpace(password))
            {
                Debug.Log("В строке есть пробелы");
                return;
            }

            if (MyDataBase.CheckPlayerInBd(nickname))
            {
                Debug.Log("Игрок с таким ником уже есть");
                return;
            }

            Debug.Log(MyDataBase._dbConnection.State);
            MyDataBase.RegisterPlayer(nickname, password);
            Debug.Log("Регистрация прошла успешно");
            idPlayer = MyDataBase.GetIdPlayer(nickname);
            textId.text = idPlayer.ToString();
            playButton.interactable = true;
            ratingButton.interactable = true;
        }
    }
}
