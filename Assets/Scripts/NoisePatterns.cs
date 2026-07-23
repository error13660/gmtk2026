using System;
using UnityEngine;

public class NoisePatterns : MonoBehaviour
{
    public static NoisePatterns Instance { get; private set; }

    [SerializeField] Texture2D pattern1;

    private void Awake()
    {
        Instance = this;

    }

    public float Pattern1(Vector2 pos)
    {
        pos = new Vector2(pos.x % pattern1.width, pos.y % pattern1.width);

        return pattern1.GetPixel((int)pos.x, (int)pos.y).r;
    }
}
