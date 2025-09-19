using TMPro;
using UnityEngine;

public class WaveNumberText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void UpdateText(int current, int max)
    {
        if (text == null)
        {
            return;
        }

        text.text = $"Wave {current} / {max}";
    }
}
