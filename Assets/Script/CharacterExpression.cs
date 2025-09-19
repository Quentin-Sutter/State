using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterExpression : MonoBehaviour
{
    public enum Expression
    {
        Hurt,
        Happy,
        Angry,
        Sad,
        Surprised,
        Sunglass,
        Love,
        Sleep,
        Stun
    }

    [Serializable]
    public struct ExpressionSprite
    {
        public Expression expression;
        public Sprite sprite;
    }

    [Header("References")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Expressions")]
    [SerializeField] private List<ExpressionSprite> expressions = new List<ExpressionSprite>();

    private Dictionary<Expression, Sprite> lookup;

    void Awake()
    {
        // Si le SpriteRenderer n’est pas assigné, on prend celui du GameObject
        if (!targetRenderer) targetRenderer = GetComponent<SpriteRenderer>();

        lookup = new Dictionary<Expression, Sprite>();
        foreach (var e in expressions)
        {
            if (!lookup.ContainsKey(e.expression))
                lookup.Add(e.expression, e.sprite);
        }
    }

    /// <summary>
    /// Change l’expression du personnage en mettant le sprite correspondant.
    /// </summary>
    public void SetExpression(Expression expr)
    {
        if (lookup.TryGetValue(expr, out var sprite))
        {
            targetRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned for expression {expr}");
        }
    }
}


