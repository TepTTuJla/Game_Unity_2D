using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataBase
{
    public class SqlScript : MonoBehaviour
    {
        public static int idPlayer;
        public static string nicknamePlayer;
        public TextMeshProUGUI textNickname;
        public GameObject textAuthNickname;
        private TMP_InputField _nickname;

        public Button playButton;
        public Button ratingButton;

        private void Start()
        {
            _nickname = textAuthNickname.GetComponent<TMP_InputField>();
            MyDataBase.CreateTables();
            MyDataBase.CreateTheFirstCompletion();
            textNickname.text = MyDataBase.check;
        }

        public void Authentication()
        {
            var nickname = _nickname.text;

            if (!MyDataBase.CheckPlayerInBd(nickname))
            {
                MyDataBase.RegisterPlayer(nickname);
                Debug.Log("Регистрация прошла успешно");
            }
            else
            {
                Debug.Log("Вход выполнен");
            }
            idPlayer = MyDataBase.GetIdPlayer(nickname);

            playButton.interactable = true;
            ratingButton.interactable = true;
            textNickname.text = nickname;
            nicknamePlayer = nickname;
        }
    }
}
