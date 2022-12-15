using TMPro;
using UnityEngine;

namespace DataBase
{
    public class SetCompletionInTable : MonoBehaviour
    {
        private CollectionInfoInBd _state;
        public static int id;
        public TextMeshProUGUI textId;

        private void Start()
        {
            _state = GameObject.FindWithTag("Player").GetComponent<CollectionInfoInBd>();
            id = SqlScript.idPlayer;
            textId.text = id.ToString();
        }

        public void Set()
        {
            _state.SetInBd();
        }
    }
}