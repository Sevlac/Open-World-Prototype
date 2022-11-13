using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public PlayerController PlayerControllerScript;
    private int selectItem = 0;
    private Transform myItemSlot;
    private Transform myToolBar;
    public GameObject Axe;
    public GameObject Pickaxe;
    public GameObject Hammer;

    void Awake()
    {
        Axe.SetActive(false);
        Pickaxe.SetActive(false);
        Hammer.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {

        myToolBar = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        ToolBar();
    }

    void ToolBar()
    {
        if (Time.timeScale == 1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                selectItem--;
                if (selectItem < 0)
                {
                    selectItem = 9;
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                selectItem++;
                if (selectItem > 9)
                {
                    selectItem = 0;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                myItemSlot = myToolBar.GetChild(i);
                if (i == selectItem)
                {
                    myItemSlot.GetComponent<Image>().color = new Color(coloNumberConversion(255f), coloNumberConversion(255f), coloNumberConversion(255f), coloNumberConversion(255f));
                }
                else
                {
                    myItemSlot.GetComponent<Image>().color = new Color(coloNumberConversion(255f), coloNumberConversion(255f), coloNumberConversion(255f), coloNumberConversion(100f));
                }
            }
            Axe.SetActive(false);
            Pickaxe.SetActive(false);
            Hammer.SetActive(false);
            PlayerControllerScript.axeInHand = false;
            PlayerControllerScript.pickaxeInHand = false;
            if (selectItem == 1)
            {
                Hammer.SetActive(true);
            }
            else if (selectItem == 2)
            {
                Axe.SetActive(true);
                PlayerControllerScript.axeInHand = true;
                PlayerControllerScript.pickaxeInHand = false;
            }
            else if (selectItem == 3)
            {
                Pickaxe.SetActive(true);
                PlayerControllerScript.axeInHand = false;
                PlayerControllerScript.pickaxeInHand = true;
            }
        }


    }


    private float coloNumberConversion(float num)
    {
        return (num / 255f);
    }
}
