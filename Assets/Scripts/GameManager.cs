using System.ComponentModel.Design.Serialization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public GameObject player;
    public GameObject enemy;
    public GameWorld gw;
    public List<GameObject> hand;
    public List<GameObject> enemyHand;
    public int selectedButton = -1;
    public int selectedField = 0;
    public bool HasPlacedCard = false;
    public TextAsset databaseObj;
    public List<int> allCards = new List<int>();
    public List<Dictionary<string, string>> cards_stats = new List<Dictionary<string, string>>();
    private Dictionary<string, string> dic;

    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    //Methods
    void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<GameManager>();
        //Get the card from xml database an add them to a list
        GetXmlStats();
        hand = new List<GameObject>();
        enemyHand = new List<GameObject>();
    }
    /// <summary>
    /// Gets the all the cards from the xml doc and place them in a list
    /// </summary>
    void GetXmlStats()
    {
        XmlDocument xmlCardDoc = new XmlDocument(); //creates new xml doc.
        xmlCardDoc.LoadXml(databaseObj.text); //load the premade xml doc into the new xml doc.

        XmlNodeList cardsList = xmlCardDoc.GetElementsByTagName("card"); //list of card nodes.

        foreach (XmlNode cardInfo in cardsList)
        {
            //Make a xml node list which hold the child value of the card 
            XmlNodeList cardContent = cardInfo.ChildNodes;
            dic = new Dictionary<string, string>();// Create a new Dictionary to hold the values the card has
            foreach (XmlNode cardValue in cardContent)
            {
                if (cardValue.Name == "id")
                {
                    dic.Add("id", cardValue.InnerText); // put the value in dictionary
                }
                if (cardValue.Name == "name")
                {
                    dic.Add("name", cardValue.InnerText); // put the value in dictionary
                }
                if (cardValue.Name == "img")
                {
                    dic.Add("img", cardValue.InnerText); // put the value in dictionary
                }
                if (cardValue.Name == "hp")
                {
                    dic.Add("hp", cardValue.InnerText); // put the value in dictionary
                }
                if (cardValue.Name == "atk")
                {
                    dic.Add("atk", cardValue.InnerText); // put the value in dictionary
                }
                if (cardValue.Name == "waitTime")
                {
                    dic.Add("waitTime", cardValue.InnerText); // put the value in dictionary
                }
            }
            cards_stats.Add(dic); //add the dictionary to the cards list
        }
    }
}
