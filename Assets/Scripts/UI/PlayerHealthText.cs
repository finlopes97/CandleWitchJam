using UnityEngine;
using TMPro;

namespace UI
{
    public class PlayerHealthText : MonoBehaviour
    {
        public TextMeshProUGUI healthText;

        private void Awake()
        {
            SetHealth(100);
        }

        private void SetHealth(int health)
        {
            healthText.text = health.ToString();
        }

        public void UpdateHealth(Component sender, object data)
        {
            if (data is int)
            {
                int amount = (int)data;
                SetHealth(amount);
            }
        }
    }
}