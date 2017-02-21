using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    //Fields
    private int waitTime;
    private int hp;
    private int atk;
    [SerializeField]
    private int buttonId;
    private Sprite tmpSprite;
    private bool drawText;
    private List<GameObject> textUI;
    private string name;
    private Sprite image;
    private bool selected;

    //Propeties
    public List<GameObject> TextUI
    {
        get { return textUI; }
        set { textUI = value; }
    }
    public bool DrawText
    {
        get { return drawText; }
        set { drawText = value; }
    }
    public Sprite TmpSprite
    {
        get { return tmpSprite; }
        set { tmpSprite = value; }
    }
    public int ButtonId
    {
        get { return buttonId; }
        set { buttonId = value; }
    }
    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }
    public bool removedFromHand;
    public int WaitTime
    {
        get { return waitTime; }
        set { waitTime = value; }
    }
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public int Atk
    {
        get { return atk; }
        set { atk = value; }
    }
    public string Name
    {
        get { return name; }
    }

    //Methods
    private void Start()
    {
        selected = false;

        textUI = new List<GameObject>();

        textUI.Add(transform.FindChild("HP").gameObject);
        textUI.Add(transform.FindChild("Atk").gameObject);
        textUI.Add(transform.FindChild("WaitTime").gameObject);

        if (gameObject.tag == "PlayerCardField")
        {
            textUI[0].GetComponent<Text>().text = hp.ToString();
            textUI[1].GetComponent<Text>().text = atk.ToString();
            textUI[2].GetComponent<Text>().text = waitTime.ToString();
        }
    }
    private void Update()
    {
        if (drawText && (gameObject.tag == "Card" || gameObject.tag == "PlayerFieldCard" || gameObject.tag == "EnemyFieldCard"))
        {
            textUI[0].GetComponent<Text>().text = hp.ToString();
            textUI[1].GetComponent<Text>().text = atk.ToString();
            textUI[2].GetComponent<Text>().text = waitTime.ToString();
        }
    }
    /// <summary>
    /// This functon is run when it is a card is clicked
    /// </summary>
    public void SelectCard()
    {
        // Adds the informations to the card infobox
        GameManager.Instance.gw.CardInfoImage.GetComponent<Image>().sprite = this.gameObject.GetComponent<Image>().sprite;
        // If it's the playr's turn, the function will check if a card is selected.
        if (GameManager.Instance.gw.PlayerTurn)
        {
            if (GameManager.Instance.selectedButton != buttonId && !selected)
            {
                if (GameManager.Instance.selectedButton != -1)
                    GameManager.Instance.gw.DeselectCard();
                if (!GameManager.Instance.HasPlacedCard)
                    GameManager.Instance.gw.HighlightFields(true);
                this.gameObject.GetComponent<Image>().color = new Color(99f, 99f, 99f, .5f);
                GameManager.Instance.gw.CardInfo.SetActive(true);
                GameManager.Instance.gw.cardInfoText[0].GetComponent<Text>().text = hp.ToString();
                GameManager.Instance.gw.cardInfoText[1].GetComponent<Text>().text = atk.ToString();
                GameManager.Instance.gw.cardInfoText[2].GetComponent<Text>().text = waitTime.ToString();
                // The id of the card is selected, which is used on the fields       
                GameManager.Instance.selectedButton = buttonId;
                selected = true;
            }
            // Fields are not highlighted
            else
            {
                GameManager.Instance.gw.HighlightFields(false);
                GameManager.Instance.gw.CardInfo.SetActive(false);
                GameManager.Instance.selectedButton = -1;
                this.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1);
                selected = false;
            }
        }
    }
    /// <summary>
    /// Get the stats for the card with the given id
    /// </summary>
    /// <param name="cardId">The id of the card</param>
    public void GetStats(int cardId, bool isPLayer)
    {
        string idTemp = "";
        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("id", out idTemp);
        cardId = Convert.ToInt32(idTemp);//Convert from string to int

        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("name", out name);

        string hpTemp = "";
        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("hp", out hpTemp);
        hp = Convert.ToInt32(hpTemp);

        string atkTemp = "";
        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("atk", out atkTemp);
        atk = Convert.ToInt32(atkTemp);

        string waitTimeTemp = "";
        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("waitTime", out waitTimeTemp);
        waitTime = Convert.ToInt32(waitTimeTemp);

        string imgTemp = "";
        GameManager.Instance.cards_stats[cardId - 1].TryGetValue("img", out imgTemp);
        image = Resources.Load<Sprite>("Card_Images/" + imgTemp);

        if (isPLayer)
        {
            this.gameObject.GetComponent<Image>().sprite = image;
        }
        else
        {
            tmpSprite = image;
        }
        drawText = true;
    }
}
