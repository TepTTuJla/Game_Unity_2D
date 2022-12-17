using TMPro;
using UnityEngine;

namespace DataBase
{
    public class SetCompletionInTable : MonoBehaviour
    {
        private CollectionInfoInBd _state;
        public static int id;
        public string nickname;
        public TextMeshProUGUI nicknameText;

        private void Start()
        {
            _state = GameObject.FindWithTag("Player").GetComponent<CollectionInfoInBd>();
            id = SqlScript.idPlayer;
            nickname = SqlScript.nicknamePlayer;
            nicknameText.text = nickname;
        }

        public void Set()
        {
            _state.SetInBd();
        }
    }
}