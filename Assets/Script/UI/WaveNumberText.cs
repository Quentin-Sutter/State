using TMPro;
using UnityEngine;

public class WaveNumberText : MonoBehaviour
{
    public TextMeshProUGUI text; 

    public void UpdateText (int current, int max)
    {
        text.text = "Wave  " + current + " / " + max;
    }

}
