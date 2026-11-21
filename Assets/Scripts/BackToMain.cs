//Name: David Sargent
//Date: 11-21-26
//Desc: Panel Buttons that add Event Listeners
//Attach: Nested Panel Buttons
using UnityEngine;
using UnityEngine.UI;

public class BackToMain : MonoBehaviour
{
    [SerializeField]
    private Button Button;
    [SerializeField]
    private GameObject CurrentPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button.onClick.AddListener( () => PanelToggle());
        
    }
    private void PanelToggle()
    {
       CurrentPanel.SetActive(false);
    }
}
