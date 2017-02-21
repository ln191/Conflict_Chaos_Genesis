using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    //Fields
    private int id;
    private string name;
    private List<int> cards;

    //Propeties
    public string Name
    {
        get { return name; }
    }
    public List<int> Cards
    {
        get { return cards; }
        set { cards = value; }
    }

    //Methods
    /// <summary>
    /// Shuffle the deck 
    /// </summary>
    public void ShuffleDeck()
    {
        // Generates a random number
        System.Random rnd = new System.Random();
        // Finds the index to change
        int n = cards.Count;
        // finds the random number we want to swarp with
        int j;
        // holds the random objct we want to swarp with
        int temp;
        while (--n > 1)
        {
            j = rnd.Next(0, n);
            temp = cards[j];
            cards[j] = cards[n];
            cards[n] = temp;
        }
    }
    /// <summary>
    /// Draw card first card in the deck
    /// </summary>
    /// <param name="toHand">The hands which draw card</param>
    /// <param name="isPlayer"></param>
    public void DrawCard(List<GameObject> toHand, bool isPlayer)
    {
        //if it is not the first round it will draw one card
        if (!GameManager.Instance.gw.FirstRound)
            for (int i = 0; i < toHand.Count; i++)
            {
                if (toHand[i].GetComponent<Card>().removedFromHand)
                {
                    int tmp = cards[0];
                    //remove first card from deck
                    cards.RemoveAt(0);
                    toHand[i].GetComponent<Image>().enabled = true;
                    // Retrieve the cards data from database
                    toHand[i].GetComponent<Card>().GetStats(tmp, isPlayer);
                    toHand[i].GetComponent<Card>().removedFromHand = false;
                    toHand[i].GetComponent<Image>().color = new Color(255, 255, 255, 1);
                }
            }
        //if it is the first round it will draw 3 card
        else
        {
            for (int i = 0; i < toHand.Count; i++)
            {
                int tmp = cards[0];
                cards.RemoveAt(0);
                toHand[i].GetComponent<Image>().enabled = true;
                toHand[i].GetComponent<Card>().GetStats(tmp, isPlayer);
                toHand[i].GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
        }
    }

}
