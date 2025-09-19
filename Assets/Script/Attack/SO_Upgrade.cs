using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Upgrade", menuName = "Scriptable Objects/SO_Upgrade")]
public class SO_Upgrade : ScriptableObject
{
    public string upgradeName;

    [Header("Weapon")]
    public float weaponSpeedPercent; //
    public float weaponDamagePercent; // 
    public float weaponSizePercent;
    public float pushPowerPercent; //
    public float pushDurationPercent; //

    [Header("Character")]
    public float moveSpeedPercent; //
    public float pushResistance; //
    public float invulnerabilityTime; //

    [Header("Dodge")]
    public float dodgeDuration;//
    public float dodgeSpeed;//

    [Header("Parry")]
    public float parryDuration; //
    public float parryDamage;//
    public float parryRetaliationSpeed;//

    /// <summary>
    /// Construit une description multi-lignes pour l’UI.
    /// </summary>
    public string BuildDescription(bool richText = true, int percentDecimals = 0, int unitDecimals = 2)
    {
        var sb = new StringBuilder();
        const float EPS = 1e-6f;

        // helpers
        string ColorWrap(string s, bool positive)
            => richText ? $"<color={(positive ? "#7CFC7C" : "#FF6B6B")}>{s}</color>" : s;

        string Pct(float v, bool inverse = false)
        {
            if (inverse) v *= -1;
            string s = $"{(v > 0 ? "+" : "")}{System.Math.Round(v, percentDecimals)}%";
            return ColorWrap(s, v >= 0);
        }

        string Unit(float v, string unit)
        {
            string s = $"{(v > 0 ? "+" : "")}{System.Math.Round(v, unitDecimals)}{unit}";
            return ColorWrap(s, v >= 0);
        }

        void Line(float v, string label, string formatted)
        {
            if (Mathf.Abs(v) < EPS) return;
            sb.Append(label).Append(" ").Append(formatted).AppendLine().AppendLine();
        }

        // --- Weapon ---
        Line(weaponSpeedPercent, "Attack Speed", Pct(weaponSpeedPercent, true));
        Line(weaponDamagePercent, "Damage", Pct(weaponDamagePercent));
        Line(weaponSizePercent, "Hitbox Size", Pct(weaponSizePercent));
        Line(pushPowerPercent, "Knockback Power", Pct(pushPowerPercent));
        Line(pushDurationPercent, "Knockback Duration", Pct(pushDurationPercent));

        // --- Character ---
        Line(moveSpeedPercent, "Move Speed", Pct(moveSpeedPercent));
        Line(pushResistance, "Knockback Resistance", Pct(pushResistance));
        Line(invulnerabilityTime, "Invulnerability", Pct(invulnerabilityTime));

        // --- Dodge ---
        Line(dodgeDuration, "Dodge Duration", Pct(dodgeDuration));
        Line(dodgeSpeed, "Dodge Distance", Pct(dodgeSpeed));

        // --- Parry ---
        Line(parryDuration, "Parry Window", Pct(parryDuration));
        Line(parryDamage, "Parry Damage", Pct(parryDamage));              // adapte si additif
        Line(parryRetaliationSpeed, "Parry Retaliation", Pct(parryRetaliationSpeed));

        return sb.Length == 0 ? (richText ? "<i>No changes</i>" : "No changes") : sb.ToString().TrimEnd();
    }
}

 
