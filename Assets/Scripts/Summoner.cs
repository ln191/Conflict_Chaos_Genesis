using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Summoner : MonoBehaviour
{
    //Fields
    private Hero hero;
    private Deck deck;
    //Propeties
    public Deck Deck
    {
        get { return deck; }
        set { deck = value; }
    }
    public Hero Hero
    {
        get { return hero; }
        set { hero = value; }
    }

}
