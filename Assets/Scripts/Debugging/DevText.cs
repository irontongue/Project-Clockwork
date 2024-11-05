using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;
public class DevText : MonoBehaviour
{

    static public bool Debugging;
    static Dictionary<string, TextMeshProUGUI> debugUI = new Dictionary<string, TextMeshProUGUI>(); 
    static Dictionary<string, bool> debugGroup = new Dictionary<string, bool>();

    [SerializeField] GameObject spawnableDebugUI;
    [SerializeField] GameObject canvasObject;

    static GameObject debugUIGO;

    static GameObject canvas;
    static int positionOffset = 280;

    float fpsUpdateTime = 0.2f;
    float timer;

    static List<GameObject> freeUI = new List<GameObject>();



    private void Start()
    {
        debugUI.Clear();
        debugUIGO = spawnableDebugUI;
        canvas = canvasObject;
        positionOffset = 380;


        for(int i =0; i < 50; i++) // pre initilize all of the debug UI
        {
            GameObject uiOBJ = Instantiate(debugUIGO);
            uiOBJ.name = i.ToString();
            uiOBJ.GetComponent<TextMeshProUGUI>().rectTransform.localPosition = new Vector3(-858, positionOffset, 0);
            uiOBJ.GetComponent<TextMeshProUGUI>().text = "";
            uiOBJ.transform.SetParent(canvas.transform,false);
            freeUI.Add(uiOBJ);
            positionOffset -= 40;

        }
        
    }
    bool uiActive;
    [SerializeField] TextMeshProUGUI groupSelector;
    bool wasPressedThisFrame;
    private void Update()
    {
        //fps counter
        timer -= Time.deltaTime;
       
        if (timer <= 0)
        {
            timer = fpsUpdateTime;
            DisplayInfo("FPS", "FPS: " + ((int)(1.0f / Time.deltaTime)).ToString(), "Basic");
        }

        if(Input.GetKeyDown(KeyCode.F1) && !wasPressedThisFrame)
        {
            Debugging = true;
            wasPressedThisFrame = true;
            print("a");
            if (uiActive)
                uiActive = false;
            else
                uiActive = true;
            print(uiActive);
            if(uiActive)
            {
                print("b");
                groupSelector.gameObject.SetActive(true);
                groupSelector.text = "Debug Groups \n";
                int i = 1;
                foreach (KeyValuePair<string, bool> group in debugGroup)
                {
                    groupSelector.text += i + ". " + group.Key + ": " + (group.Value == false ? "Inactive" : "Active") + "\n";
                    i++;
                }
            }
            else
            {
                groupSelector.gameObject.SetActive(false);
            }
        }

      
       
      
    }
    private void LateUpdate()
    {
        wasPressedThisFrame = false;
    }
    void OnGUI()
    {
        if (!uiActive)
            return;

        Event e = Event.current;
        if (e.isKey)
        {
            int i = 1;
            foreach (KeyValuePair<string, bool> group in debugGroup)
            {
                if (e.character.ToString() == i.ToString())
                {
                    debugGroup[group.Key] = !group.Value;
                    uiActive = false;
                    groupSelector.gameObject.SetActive(false);

                   
                    return;
                }
                i++; 
            }
        }
    }

    public static void DisplayInfo(string identifyer, string message,string group)
    {
        

        if(!debugGroup.ContainsKey(group))
        {
            debugGroup.Add(group, false);
        }
        if (debugGroup[group] == false) // if the group has been deactivated dont continue, remove it from the pool.
        {
          
            if(debugUI.ContainsKey(identifyer)) // if the ui exists, empty it and return it to the free ui pool
            {
                debugUI[identifyer].text = "";
                freeUI.Add(debugUI[identifyer].gameObject);
               
                debugUI.Remove(identifyer);
                
              
            }
                
            return;
        }
            

        if (debugUI.ContainsKey(identifyer)) // if this debug allready exists dont worry about instaniating it
        {
            debugUI[identifyer].text = message;
        }
        else // if we havent seen it assign it a UI
        {
            GameObject ui;
            ui = freeUI[0];
            foreach(GameObject g in freeUI) // find the lowest number UI element
            {
                
                if (Int32.Parse(g.name) < Int32.Parse(ui.name))
                    ui = g;
            }
         
             freeUI.Remove(ui);
    
            TextMeshProUGUI tmpro = ui.GetComponent<TextMeshProUGUI>();
            debugUI.Add(identifyer, tmpro);
 
        }
    }
 
   
}
