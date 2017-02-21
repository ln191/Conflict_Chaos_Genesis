using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    //fields
    private string name;
    private int hp;
    private List<GameObject> textUI;
   //Propeties
    public List<GameObject> TextUI
    {
        get { return textUI; }
        set { textUI = value; }
    }
    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    //Methods
    private void Start()
    {
        textUI = new List<GameObject>();
        hp = 20;
        textUI.Add(gameObject.transform.FindChild("Name").gameObject);
        textUI.Add(gameObject.transform.FindChild("HP").gameObject);
        if (gameObject.tag == "EnemyHero")
        {
            name = "Generic Turd";
            textUI[0].GetComponent<Text>().text = name;
        }
        textUI[1].GetComponent<Text>().text = hp.ToString();
    }
    /// <summary>
    /// Add hero info
    /// </summary>
    /// <param name="name"></param>
    public void AddHeroInfo(string name)
    {
        textUI[0].GetComponent<Text>().text = name;
    }
    /// <summary>
    /// Hero take damage
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        hp -= amount;
        textUI[1].GetComponent<Text>().text = "";
        textUI[1].GetComponent<Text>().text = hp.ToString();
    }
}
