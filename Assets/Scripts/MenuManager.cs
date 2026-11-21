//Name: David Sargent
//Date: 11/21/26 [11/20/26 after midnight]
//Desc: Manages the Panels and GUIs for the MainMenu
//Attach: MainMenu(MenuManager)
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //Label; GameObjects[Panels]
    [Header("Label Elements")]
    [SerializeField]
    private TMP_Text ScoreList;
    [SerializeField]
    private GameObject HighScorePanel;
    [SerializeField]
    private GameObject HowToPanel;
    [SerializeField]
    private Button QuitButton;
    //Start is at initialization
    private void Start()
    {
        HighScorePanel.SetActive(false);
        HowToPanel.SetActive(false);
        //Quit Button was failing event, add listener
        QuitButton.onClick.AddListener(() => QuitGame());
    }
    //Menu Flow Functions
    public void PlayGame()
    {
        //Play Button makes the game in motion
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainGame");
    }
    public void ToggleHowTo()
    {
        //One-way enables button
        HowToPanel.SetActive(true);
    }
    public void QuitGame()
    {
        //Quit Button leaves the application
        Debug.Log("Quitting Application. . .");
        Application.Quit();
    }
    public void ToggleHighScore()
    {
        //One-way enables button
        OccupyScoreList();
        HighScorePanel.SetActive(true);        
    }
    private void OccupyScoreList()
    {
        //Going to locally load the text, make a list, check its count;
        //Parameter hierarchy goes if 0, elif < 5, else full list

        //Load the Text, make a list using ReadAll Text
        //Conglomerating the Load + Present
        string Path = Application.persistentDataPath + "/Highscores.txt";
        string LocalData = null;
        string LabelText = null;
        if(!File.Exists(Path))
        {
            //File does not exist, create it with 5 0s;
            File.WriteAllText(Path, "0,0,0,0,0");
        }
        else
        {
            //File does exist, read the text
            LocalData = File.ReadAllText(Path);
        }    
        //First run needs null-check
        if(LocalData == null)
        {
            LocalData = File.ReadAllText(Path);
        }
        // Use CPP Dual if-check
        if(LocalData != null)
        {
            
            string[] ScoreArray = LocalData.Split(',');
            for(int i = 0; i < ScoreArray.Length; i++)
            {
                //Assemble Label Text as payload, then we set the label itself
                LabelText += "#" + (i+1) + "| " + ScoreArray[i] + ";\n";
            }
        }
        //Null-check LabelText and put on ScoreList
        if(LabelText != null)
        {
            ScoreList.text = LabelText;
        }    
    }

}
