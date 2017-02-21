using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class Enemy : Summoner
{
    //Fields
    int rndNum;

    //Methods
    public void EnemyAI()
    {
        SelectRandomCardFromHand();
        StartCoroutine(InfoDelay());
    }
    /// <summary>
    /// Select a card randomly from the hand
    /// </summary>
    private void SelectRandomCardFromHand()
    {
        System.Random rnd = new System.Random();
        if (GameManager.Instance.enemyHand.Count != 0)
            rndNum = rnd.Next(1, GameManager.Instance.enemyHand.Count);
        for (int i = 0; i < GameManager.Instance.gw.enemyFields.Count; i++)
        {
            //if there is a not occupied field 
            if (!GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().Occupied)
            {
                //select a random card on the hand
                GameManager.Instance.enemyHand[rndNum].GetComponent<Card>().SelectCard();
                GameManager.Instance.gw.CardInfoImage.GetComponent<Image>().sprite = GameManager.Instance.enemyHand[rndNum].GetComponent<Card>().TmpSprite;
                //Show card info on screen
                GameManager.Instance.gw.CardInfo.SetActive(true);
                return;
            }
        }


    }
    /// <summary>
    /// Place the card opposite to a player card 
    /// if there is non it will be place in the first not occupied field
    /// </summary>
    private void PlaceSelectedCardOnField()
    {
        //Run througe the player fields and see if there is a card on it 
        for (int i = 0; i < GameManager.Instance.gw.playerFields.Count; i++)
        {
            //if there is a card on the field and if the opposite enemy field is not occupied
            //the card will placed 
            if (GameManager.Instance.gw.playerFields[i].GetComponent<Field>().Occupied && !GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().Occupied)
            {
                //place card on field
                Place(i);
                //exit the method
                return;
            }
        }
        //if there where non card to be place oppsite of 
        //it will run througe enemy fields and find a not occupied field
        for (int i = 0; i < GameManager.Instance.gw.enemyFields.Count; i++)
        {
            if (!GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().Occupied)
            {
                //place card on field
                Place(i);
                //exit method
                return;
            }

        }

    }
    /// <summary>
    /// Place the card on field
    /// </summary>
    /// <param name="i"></param>
    private void Place(int i)
    {
        //add a card script to field
        GameManager.Instance.gw.enemyFields[i].AddComponent<Card>();
        //copy the data from the card the hand to the card on the field
        GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().CopyClassValues(GameManager.Instance.enemyHand[rndNum].GetComponent<Card>(), GameManager.Instance.gw.enemyFields[i].GetComponent<Card>());
        // Moves the image
        GameManager.Instance.gw.enemyFields[i].GetComponent<Image>().sprite = GameManager.Instance.enemyHand[rndNum].GetComponent<Card>().TmpSprite;
        GameManager.Instance.gw.enemyFields[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 1);
        //set the field to be occupied
        GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().Occupied = true;
        GameManager.Instance.gw.enemyFields[i].GetComponent<Button>().interactable = true;
        // De-activate Image component
        GameManager.Instance.enemyHand[rndNum].GetComponent<Image>().enabled = false;
        // Sets the player has placed card to true so that the play can only place one card each round
        GameManager.Instance.HasPlacedCard = true;
        // De-activates the card info panel
        GameManager.Instance.gw.CardInfo.SetActive(false);
        // Removes the card component from the card on the players hand
        GameManager.Instance.enemyHand[rndNum].GetComponent<Card>().removedFromHand = true;

    }
    /// <summary>
    /// A enumerator to make a delay so the can see enemy cardinfo before it is placed
    /// </summary>
    /// <returns></returns>
    IEnumerator InfoDelay()
    {
        //wait 2 sec
        yield return new WaitForSeconds(2);
        // End of turn
        PlaceSelectedCardOnField();
    }
}