using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwap : MonoBehaviour
{

    public Selectable target;

    public SpriteState otherState;
    public Sprite otherSprite;

    SpriteState normalState;
    Sprite normalSprite;

    // Use this for initialization
    void Start()
    {
        normalState = target.spriteState;
        normalSprite = target.image.sprite;
    }

    public void SwapSprites()
    {
        SpriteState tempState = normalState;
        Sprite tempSprite = normalSprite;

        normalState = otherState;
        otherState = tempState;

        normalSprite = otherSprite;
        otherSprite = tempSprite;

        target.spriteState = normalState;
        target.image.sprite = normalSprite;
    }
}
