using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class GameWorld : MonoBehaviour
{
    //Fields
    private bool playerTurn = true;
    private List<int> cardsInDeck2;
    private List<int> cardsInDeck1;
    private Deck deck1;
    private Deck deck2;
    private GameObject playerHero;
    private GameObject enemyHero;
    private Sprite defaultSprite;
    private float turnTime;
    private bool playerWin;
    private List<Deck> possibleDecks;
    private bool firstRound = true;
    private GameObject battle;
    private GameObject cardInfo;
    private GameObject cardInfoImage;
    public List<GameObject> playerFields;
    public List<GameObject> enemyFields;
    public List<GameObject> cardInfoText;
    public GameObject WinPanel;
    public GameObject LosePanel;

    //Propeties
    public bool FirstRound
    {
        get { return firstRound; }
        set { firstRound = value; }
    }
    public GameObject CardInfoImage
    {
        get { return cardInfoImage; }
        set { cardInfoImage = value; }
    }
    public bool PlayerTurn
    {
        get { return playerTurn; }
    }
    public GameObject CardInfo
    {
        get { return cardInfo; }
        set { cardInfo = value; }
    }
    //Methods
    void Awake()
    {
        cardsInDeck1 = new List<int>();
        cardsInDeck2 = new List<int>();

        Instantiate(Resources.Load("Prefab/Enemy"));
        Instantiate(Resources.Load("Prefab/Player"));

        deck1 = new Deck();
        deck2 = new Deck();

    }

    void Start()
    {
        defaultSprite = Resources.Load("Texture/FIeld") as Sprite;
        playerHero = GameObject.Find("PlayerHero");
        enemyHero = GameObject.Find("EnemyHero");
        GameManager.Instance.enemy = GameObject.FindGameObjectWithTag("Enemy");
        GameManager.Instance.player = GameObject.FindGameObjectWithTag("Player");
        cardInfo = GameObject.Find("CardInfoPanel");
        cardInfoImage = GameObject.Find("CardInfo");
        cardInfo.SetActive(false);
        GameManager.Instance.hand.AddRange(GameObject.FindGameObjectsWithTag("Card"));
        GameManager.Instance.enemyHand.AddRange(GameObject.FindGameObjectsWithTag("EnemyCard"));
        battle = GameObject.Find("Battle");
        ApplyIDs();
        GenerateDeck();
        deck1.Cards = cardsInDeck1;
        deck2.Cards = cardsInDeck2;

        ActivateBoard(false);
    }

    /// <summary>
    /// Starts Player turn
    /// </summary>
    private void UserTurn()
    {
        // Ensures that the player can act, place card and cards are drawn
        SetUpTurn(true, playerFields);
        GameManager.Instance.HasPlacedCard = false;
        //Player draw card
        GameManager.Instance.player.GetComponent<Player>().Deck.DrawCard(GameManager.Instance.hand, true);
    }
    /// <summary>
    /// Starts Enemy turn
    /// </summary>
    private void EnemyTurn()
    {
        // Start of turn
        SetUpTurn(false, enemyFields);
        GameManager.Instance.HasPlacedCard = false;
        //Enemy draw card
        GameManager.Instance.enemy.GetComponent<Enemy>().Deck.DrawCard(GameManager.Instance.enemyHand, false);
        //Run enemy ai
        GameManager.Instance.enemy.GetComponent<Enemy>().EnemyAI();
        //Start battle
        BattleLogic(false);
        //Wait 2 sec before switch to player turn
        StartCoroutine(DelayPlayerTurn());
    }
    /// <summary>
    /// A IEnumerator to make delay before switch to player turn
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayPlayerTurn()
    {
        yield return new WaitForSeconds(2);
        // End of turn
        UserTurn();
    }
    /// <summary>
    /// Set up the Turn for either player or enemy
    /// </summary>
    /// <param name="enablePlayer"></param>
    /// <param name="fieldList"> This defines the list of fields for the summoner, whom's turn it is</param>
    private void SetUpTurn(bool enablePlayer, List<GameObject> fieldList)
    {
        // All fields' interactablity is changed, depending on the given bool
        for (int i = 0; i < fieldList.Count; i++)
        {
            if (!fieldList[i].GetComponent<Field>().Occupied)
                fieldList[i].GetComponent<Button>().interactable = false;
        }
        // Button is equally changed
        battle.GetComponent<Button>().interactable = enablePlayer;
        playerTurn = enablePlayer;
    }
    /// <summary>
    /// The functionally of the battle button
    /// </summary>
    public void Battle()
    {
        DeselectCard();
        GameManager.Instance.selectedButton = -1;
        SetUpTurn(false, playerFields);
        BattleLogic(true);
        // Set the turn to enemy turn
        EnemyTurn();
        firstRound = false;
    }
    /// <summary>
    /// Highlight the fields 
    /// </summary>
    /// <param name="highLight"></param>
    public void HighlightFields(bool highLight)
    {
        // Runs through all the playerfields
        for (int i = 0; i < GameManager.Instance.gw.playerFields.Count; i++)
        {
            // If it isn't occupied it should be highlighed
            if (!GameManager.Instance.gw.playerFields[i].GetComponent<Field>().Occupied)
            {
                GameManager.Instance.gw.playerFields[i].GetComponent<Button>().interactable = highLight;
            }
            //If it is occupied, it shouldn't be highlighted
            else
            {
                GameManager.Instance.gw.playerFields[i].GetComponent<Button>().interactable = !highLight;
            }
        }
        for (int i = 0; i < GameManager.Instance.gw.enemyFields.Count; i++)
        {
            if (GameManager.Instance.gw.enemyFields[i].GetComponent<Field>().Occupied)
            {
                GameManager.Instance.gw.enemyFields[i].GetComponent<Button>().interactable = !highLight;
            }
        }
    }
    /// <summary>
    /// Deselect a card
    /// </summary>
    public void DeselectCard()
    {
        for (int i = 0; i < GameManager.Instance.hand.Count; i++)
        {
            // If the card isn't null, it checks if the cards name differ
            if (GameManager.Instance.hand[i].GetComponent<Card>().ButtonId == GameManager.Instance.selectedButton)
            {
                // If the names are different, it must be another card, which means previous card is deselected
                if (GameManager.Instance.hand[i].GetComponent<Image>().IsActive())
                {
                    GameManager.Instance.hand[i].GetComponent<Card>().Selected = false;
                    GameManager.Instance.hand[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 1);
                }
            }
        }
    }
    /// <summary>
    /// Win - lose  
    /// </summary>
    /// <param name="playerWin"></param>
    private void WinLose(bool playerWin)
    {
        if (playerWin)
        {
            WinPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            LosePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    /// <summary>
    /// Generate two Decks
    /// </summary>
    private void GenerateDeck()
    {
        for (int i = 1; i < 21; i++)
        {
            GameManager.Instance.allCards.Add(i);
        }
        // Generate deck 1
        for (int i = 1; i < GameManager.Instance.allCards.Count * 0.5f + 1; i++)
        {
            // Adds the first 10 cards to the deck
            cardsInDeck1.Add(i);
        }
        // Dublicate deck one
        int temp;
        for (int i = 0; i < 10; i++)
        {
            temp = cardsInDeck1[i];
            cardsInDeck1.Add(temp);
        }

        // Generate deck two
        for (int i = GameManager.Instance.allCards.Count; i > GameManager.Instance.allCards.Count * 0.5f; i--)
        {
            cardsInDeck2.Add(i);
        }
        // Duclicates deck two
        for (int i = 0; i < 10; i++)
        {
            temp = cardsInDeck2[i];
            cardsInDeck2.Add(temp);
        }
    }
    /// <summary>
    /// Select the deck the player want to use
    /// </summary>
    /// <param name="deckPicked"></param>
    public void DeckSelection(int deckPicked)
    {
        // Deck one is picked and assigned to player, other is assinged to enemy
        if (deckPicked == 1)
        {
            GameManager.Instance.player.GetComponent<Player>().Deck = deck1;
            GameManager.Instance.enemy.GetComponent<Enemy>().Deck = deck2;
        }
        // Deck two is picked and assigned to player, other is assinged to enemy
        if (deckPicked == 2)
        {
            GameManager.Instance.player.GetComponent<Player>().Deck = deck2;
            GameManager.Instance.enemy.GetComponent<Enemy>().Deck = deck1;
        }
        // Shuffles Decks
        GameManager.Instance.player.GetComponent<Player>().Deck.ShuffleDeck();
        GameManager.Instance.enemy.GetComponent<Enemy>().Deck.ShuffleDeck();

        // The deck selection panel is hidden
        GameObject selection = GameObject.Find("DeckSelectionPanel");
        selection.SetActive(false);
        // Board is activated, so actions can be taken
        ActivateBoard(true);

        // Card is drawn and card can be planed
        GameManager.Instance.HasPlacedCard = false;
        GameManager.Instance.player.GetComponent<Player>().Deck.DrawCard(GameManager.Instance.hand, true);


        // Adds the name to the hero
        // Finds the input field
        GameObject playerName = selection.gameObject.transform.FindChild("PlayerName").gameObject;
        if (playerName.gameObject.transform.FindChild("Text").GetComponent<Text>().text != string.Empty)
            playerHero.GetComponent<Hero>().AddHeroInfo(playerName.gameObject.transform.FindChild("Text").GetComponent<Text>().text);
        else
            playerHero.GetComponent<Hero>().AddHeroInfo("Unknown player");
    }
    /// <summary>
    /// Set the player side of the board to be active or not active
    /// So the player can´t interact with gui
    /// </summary>
    /// <param name="activate"></param>
    private void ActivateBoard(bool activate)
    {
        // Sets the battle buttons interactable to false, so the player can't battle the opponent with out a deck
        battle.GetComponent<Button>().interactable = activate;
        // Sets the card on hand buttonss interactable to false
        for (int i = 0; i < GameManager.Instance.hand.Count; i++)
        {
            // Deactivates the button
            GameManager.Instance.hand[i].GetComponent<Button>().interactable = activate;
            // deables the image component on the gameObject
            GameManager.Instance.hand[i].GetComponent<Image>().enabled = activate;
        }

        // Fields are all deactivated, so that when a card is selected, they will be interactable
        for (int i = 0; i < 5; i++)
        {
            enemyFields[i].GetComponent<Button>().interactable = false;
            playerFields[i].GetComponent<Button>().interactable = false;
        }
    }
    /// <summary>
    /// Contorls the flow of the battle 
    /// </summary>
    /// <param name="pt"></param>
    private void BattleLogic(bool pt)
    {
        // loops through all of the fields
        for (int i = 0; i < playerFields.Count; i++)
        {
            if (pt)
            {
                // Checks if the field has a card script
                if (playerFields[i].GetComponent<Card>() && enemyFields[i].GetComponent<Card>() && playerFields[i].GetComponent<Card>().WaitTime == 0)
                {
                    // Takes damage from the card below
                    if (enemyFields[i].GetComponent<Card>().Hp > 0)
                    {
                        enemyFields[i].GetComponent<Card>().Hp -= playerFields[i].GetComponent<Card>().Atk;
                    }

                    // Checks if the cards hp is zero or below zero
                    if (enemyFields[i].GetComponent<Card>().Hp <= 0)
                    {
                        Debug.Log("Enemy is killed");
                        enemyFields[i].GetComponent<Card>().Atk = 0;
                        Destroy(enemyFields[i].GetComponent<Card>());
                        enemyFields[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/FIeld");
                        enemyFields[i].GetComponent<Field>().Occupied = false;
                    }

                }
                // Checks if the hero should take damage
                else if (playerFields[i].GetComponent<Card>() && !enemyFields[i].GetComponent<Card>() && playerFields[i].GetComponent<Card>().WaitTime == 0)
                {
                    // Enemy hero takes damage if the hp is above zero
                    if (enemyHero.GetComponent<Hero>().Hp > 0)
                    {
                        enemyHero.GetComponent<Hero>().TakeDamage(playerFields[i].GetComponent<Card>().Atk);
                    }

                    // Checks if the heros HP is zero or below zero
                    if (enemyHero.GetComponent<Hero>().Hp <= 0)
                    {
                        // Sets the player bool
                        WinLose(true);
                        // Stops the game
                        //Time.timeScale = 0;
                    }
                }
                // Checks if the field have a card component and if the wait time os above zero
                if (playerFields[i].GetComponent<Card>() && playerFields[i].GetComponent<Card>().WaitTime > 0)
                {

                    // Minus one from the cards wait time
                    playerFields[i].GetComponent<Card>().WaitTime--;
                }

            }
            else
            { // Same as above just for the enemy
                if (playerFields[i].GetComponent<Card>() && enemyFields[i].GetComponent<Card>() && enemyFields[i].GetComponent<Card>().WaitTime == 0)
                {
                    if (playerFields[i].GetComponent<Card>().Hp > 0)
                    {
                        playerFields[i].GetComponent<Card>().Hp -= enemyFields[i].GetComponent<Card>().Atk;
                    }


                    if (playerFields[i].GetComponent<Card>().Hp <= 0)
                    {
                        CardDead(true);
                        enemyFields[i].GetComponent<Card>().Atk = 0;
                        Destroy(playerFields[i].GetComponent<Card>());
                        playerFields[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/FIeld");
                        playerFields[i].GetComponent<Field>().Occupied = false;
                        playerFields[i].GetComponent<Button>().interactable = false;
                        Debug.Log("Player is killed");
                    }

                }
                else if (enemyFields[i].GetComponent<Card>() && !playerFields[i].GetComponent<Card>() && enemyFields[i].GetComponent<Card>().WaitTime == 0)
                {
                    if (playerHero.GetComponent<Hero>().Hp > 0)
                    {

                        playerHero.GetComponent<Hero>().TakeDamage(enemyFields[i].GetComponent<Card>().Atk);
                    }

                    if (playerHero.GetComponent<Hero>().Hp <= 0)
                    {
                        // Sets the player won bool
                        WinLose(false);
                    }
                }
                if (enemyFields[i].GetComponent<Card>() && enemyFields[i].GetComponent<Card>().WaitTime > 0)
                {
                    enemyFields[i].GetComponent<Card>().WaitTime--;
                }
            }
        }
    }
    /// <summary>
    /// Clear the text on the card on the hand
    /// </summary>
    public void CardCleanUp()
    {
        //run througe hand 
        for (int i = 0; i < GameManager.Instance.hand.Count; i++)
        {
            //if the card has been remove from hand  clean the card text
            if (GameManager.Instance.hand[i].GetComponent<Card>().removedFromHand)
            {
                GameManager.Instance.hand[i].GetComponent<Card>().DrawText = false;
                GameManager.Instance.hand[i].GetComponent<Card>().TextUI[0].GetComponent<Text>().text = "";
                GameManager.Instance.hand[i].GetComponent<Card>().TextUI[1].GetComponent<Text>().text = "";
                GameManager.Instance.hand[i].GetComponent<Card>().TextUI[2].GetComponent<Text>().text = "";
            }
        }
    }
    /// <summary>
    /// Clean card text on the dead on the field
    /// </summary>
    /// <param name="playerCard"></param>
    public void CardDead(bool playerCard)
    {
        for (int i = 0; i < playerFields.Count; i++)
        {
            if (playerCard)
            {
                if (playerFields[i].GetComponent<Card>() && playerFields[i].GetComponent<Card>().Hp <= 0)
                {
                    playerFields[i].GetComponent<Card>().TextUI[0].GetComponent<Text>().text = "";
                    playerFields[i].GetComponent<Card>().TextUI[1].GetComponent<Text>().text = "";
                    playerFields[i].GetComponent<Card>().TextUI[2].GetComponent<Text>().text = "";
                    break;
                }
            }
        }
    }
    /// <summary>
    /// Give every button a id 
    /// </summary>
    private void ApplyIDs()
    {
        int uniqeID = 0;
        //Player fields
        for (int i = 0; i < playerFields.Count; i++)
        {
            playerFields[i].GetComponent<Field>().ButtonId = i;
            playerFields[i].GetComponent<Field>().Occupied = false;
            playerFields[i].GetComponent<Button>().interactable = false;
            uniqeID++;
        }
        // Enemy fields 
        for (int i = 0; i < enemyFields.Count; i++)
        {
            enemyFields[i].GetComponent<Field>().ButtonId = uniqeID + i;
            enemyFields[i].GetComponent<Field>().Occupied = false;
            enemyFields[i].GetComponent<Button>().interactable = false;
        }

        uniqeID += enemyFields.Count;
        // Player Hand
        for (int i = 0; i < GameManager.Instance.hand.Count; i++)
        {
            GameManager.Instance.hand[i].GetComponent<Card>().ButtonId = uniqeID + i;
        }
    }
}
