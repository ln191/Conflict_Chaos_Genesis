using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
public class Field : MonoBehaviour
{
    //Fields
    private bool highlighted;
    private bool occupied;
    private bool selected;
    [SerializeField]
    private int buttonId;
    // Properties
    public int ButtonId
    {
        get { return buttonId; }
        set { buttonId = value; }
    }
    public bool Highlighted
    {
        get { return highlighted; }
    }
    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }
    //Methods
    void Start()
    {
        this.gameObject.GetComponent<Button>().interactable = false;
    }
    /// <summary>
    /// Place selected card on this field
    /// </summary>
    public void PlaceOnField()
    {
        // If card has been placed and isn't occupied
        if (!GameManager.Instance.HasPlacedCard && !occupied)
        {
            // Runs through hand
            for (int i = 0; i < GameManager.Instance.hand.Count; i++)
            {
                // If it has a card componenet and a button is selected
                if (GameManager.Instance.hand[i].GetComponent<Card>() != null && GameManager.Instance.selectedButton != -1)
                    // If the button matches the ID of the card
                    if (GameManager.Instance.selectedButton == GameManager.Instance.hand[i].GetComponent<Card>().ButtonId)
                    {
                        // Add the Card script to the field component
                        this.gameObject.AddComponent<Card>();
                        // Copy the class values from card component on hand to field
                        CopyClassValues(GameManager.Instance.hand[i].GetComponent<Card>(), this.gameObject.GetComponent<Card>());
                        // Moves the image
                        this.gameObject.GetComponent<Image>().sprite = GameManager.Instance.hand[i].GetComponent<Image>().sprite;
                        // De-activate Image component
                        GameManager.Instance.hand[i].GetComponent<Image>().enabled = false;
                        // Sets the player has placed card to true so that the play can only place one card each round
                        GameManager.Instance.HasPlacedCard = true;
                        // Sets this field to occupied, so that the player cannot place a card oppon it
                        occupied = true;
                        // De-activates the card info panel
                        GameManager.Instance.gw.CardInfo.SetActive(false);
                        GameManager.Instance.selectedButton = -1;
                        GameManager.Instance.hand[i].GetComponent<Card>().Selected = false;
                        // Removes the card component from the card on the players hand
                        GameManager.Instance.hand[i].GetComponent<Card>().removedFromHand = true;
                        // De-activates all fields
                        GameManager.Instance.gw.HighlightFields(false);
                    }
            }
        }
    }
    /// <summary>
    /// Show the cards info
    /// </summary>
    public void ShowCardInfo()
    {
        GameManager.Instance.gw.DeselectCard();
        // Adds the informations to the card infobox
        GameManager.Instance.gw.CardInfoImage.GetComponent<Image>().sprite = this.gameObject.GetComponent<Image>().sprite;

        if (GameManager.Instance.selectedButton != buttonId && occupied)
        {
            GameManager.Instance.gw.CardInfo.SetActive(true);
            GameManager.Instance.selectedButton = buttonId;

        }
        else
        {
            GameManager.Instance.gw.CardInfo.SetActive(false);
            GameManager.Instance.selectedButton = -1;
        }

    }
    /// <summary>
    /// Copies a cards data to another card
    /// </summary>
    /// <param name="sourceComp">Card you copy from</param>
    /// <param name="targetComp">Card you copy to</param>
    public void CopyClassValues(Card sourceComp, Card targetComp)
    {
        // Finds all fields in the source component (public, private and instances)
        FieldInfo[] sourceFields = sourceComp.GetType().GetFields(BindingFlags.Public |
                                                         BindingFlags.NonPublic |
                                                         BindingFlags.Instance);
        // Runs throug the array
        for (int i = 0; i < sourceFields.Length; i++)
        {
            // Adds the current value to a var
            var value = sourceFields[i].GetValue(sourceComp);
            // Adds the value from the var to the target component
            sourceFields[i].SetValue(targetComp, value);
        }
    }
}
