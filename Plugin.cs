using BepInEx;
using UnityEngine;
using TMPro;

namespace ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        Canvas canvas;
        TextMeshProUGUI popup_text;
        TextMeshProUGUI command_text;
        string command_string = "";
        int popup_timer = 0;
        Vector2 player_pos;
        Rigidbody2D rb;
        bool selected = false;
        bool godmode = false;
        bool xdamage = false;
        bool flight = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} by Daltonyx is loaded!");
        }

        public void AddGeo(int amount)
        {
            if (amount < 0)
            {
                Log("You can't choose a negative value!");
                return;
            }

            GameManager.instance.playerData.geo += amount;
            GeoCounter geoCounter = FindObjectOfType<GeoCounter>();
            int counterCurrent;
            counterCurrent = GameManager.instance.playerData.geo;
            geoCounter.geoTextMesh.text = counterCurrent.ToString();
            //i know it can be cleaned up but eh
            //GameManager.instance.playerData.geo += amount;
            Log($"Added {amount} Geo!\nGeo: {GameManager.instance.playerData.geo}");
            
            
        }

        public void AddCharm(int charmNum)
        {
            if (charmNum < 0 || charmNum > 44)
            {
                Log("That number doesn't exist.");
                return;
            }
            
            
            GameManager.instance.playerData.equippedCharms.Add(charmNum);
            
            
        }

        public void AddToMPReserve(int amount)
        {
            GameManager.instance.playerData.MPReserveMax += amount;
            GameManager.instance.playerData.MPReserveCap += amount;
            GameManager.instance.playerData.MPReserve += amount;
            GameManager.instance.playerData.AddToMaxMPReserve(amount);
            
            Log($"Added {amount} to MP Reserve\nMP = {GameManager.instance.playerData.MPReserve}\nMaxMPreserve = {GameManager.instance.playerData.MPReserveMax}");
        }

        public void AddMP(int amount)
        {
            HeroController.instance.AddMPCharge(amount);
            Log($"Added {amount} to MP!");
        }

        public void AddToHealthReserve(int amount)
        {
            GameManager.instance.playerData.maxHealthBase += amount;
            GameManager.instance.playerData.maxHealth += amount;
            GameManager.instance.playerData.AddHealth(amount);
            GameManager.instance.playerData.AddToMaxHealth(amount);
            Log($"Added {amount} to Health Reserve\nHealth = {GameManager.instance.playerData.health}\nMaxHealth = {GameManager.instance.playerData.maxHealth}");
        }

        public void UnlockAllAchievements()
        {
            Log("Unlocking All Achievements\nRate the mod on nexus if you enjoy! (;");
            //find current achievement handler
            AchievementHandler achievementHandler = FindObjectOfType<AchievementHandler>();

            //I'm not sure which one actually worked, but I'm leaving both in because I don't wanna test it. It works, and that's all that matters. (it gave me all 63 achievements <3)
            achievementHandler.AwardAllAchievements();

            foreach (Achievement achievement in achievementHandler.achievementsList.achievements)
            {
                achievementHandler.AwardAchievementToPlayer(achievement.key);
            }
        }

        public void Log(string message)
        {
            Logger.LogInfo(message);
            if (canvas != null)
            {
                popup_text.text = message;
                popup_timer = 400;
            }
        }

        public void Update()
        {
            //actually do a title check, so we aren't caught with our pants down :>
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString() == "Menu_Title" || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString() == "Pre_Menu_Intro")
            {
                return;
            }            
            if (canvas == null)
            {
                //create a new canvas
                canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                //create popup_text and set it's parent to canvas
                popup_text = new GameObject("Popup Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                popup_text.transform.SetParent(canvas.transform);
                //set the text to be in the center of the screen
                popup_text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                popup_text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                popup_text.rectTransform.anchoredPosition = new Vector2(0, 0);
                
                popup_text.alignment = TextAlignmentOptions.Center;
                popup_text.fontSize = .5f;
                popup_text.color = Color.white;
                popup_text.text = "";

                //do the same for command_text, but it's position should be -13.1818 -8.1 0
                command_text = new GameObject("Command Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                command_text.transform.SetParent(canvas.transform);
                command_text.rectTransform.anchorMin = new Vector2(0, 0);
                command_text.rectTransform.anchorMax = new Vector2(0, 0);
                command_text.rectTransform.anchoredPosition = new Vector2(0,0);
                command_text.alignment = TextAlignmentOptions.Left;
                command_text.fontSize = .5f;
                command_text.color = Color.white;
                command_text.text = "Press / for commands";
                selected = false;
                command_string = "";
                //set position to (-12,8)
                command_text.rectTransform.position = new Vector3(35.5f, -8, 0);

                //bring it to the front of the 2d screen
                canvas.planeDistance = 1;
                //set it to the 'UI' layer
                canvas.gameObject.layer = 5;
            }
            if (canvas != null)
            {
                if (godmode)
                {
                    GameManager.instance.playerData.health = 999;
                    GameManager.instance.playerData.maxHealth = 999;
                    GameManager.instance.playerData.MPCharge = 999;
                }
                if (xdamage)
                {
                    GameManager.instance.playerData.nailDamage = 999;
                    GameManager.instance.playerData.beamDamage = 999;
                }
                if (flight)
                {
                    HeroController instance = HeroController.instance;
                    if (instance != null)
                    {
                        //rb = instance.
                    }
                    //player_pos = HeroController.instance.
                }


                //if canvas isn't null, we can attempt to access values and stuff, but it's still unsafe in title
                if (popup_timer > 0)
                {
                    popup_timer--;
                    if (popup_timer == 0)
                    {
                        popup_text.text = "";
                    }
                }

                if (Input.GetKeyDown(KeyCode.Slash))
                {
                    if (selected)
                    {
                        selected = false;
                        command_text.text = "Press / for commands";
                        command_string = "";
                    }
                    else
                    {
                        selected = true;
                        command_text.text = "/";
                        command_string = "/";
                    }
                }

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (selected)
                    {
                        if (command_string.Length > 0)
                        {
                            command_string = command_string.Substring(0, command_string.Length - 1);
                            command_text.text = command_string;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (selected)
                    {
                        selected = false;
                        command_text.text = "Press / for commands";
                        command_string = "";
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {

                    if (command_string.StartsWith("/addhealth"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);

                        if (choice < 0)
                        {
                            Log("You can't add negative health! (;");
                        }
                        else
                        {
                            AddToHealthReserve(choice);
                        }
                    }

                    else if (command_string.StartsWith("/addmp"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);

                        if (choice < 0)
                        {
                            Log("You can't add negative MP! (;");
                        }
                        else
                        {
                            AddMP(choice);
                        }
                    }

                    else if (command_string.StartsWith("/addmpreserve"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);

                        if (choice < 0)
                        {
                            Log("You can't add negative MP! (;");
                        }
                        else
                        {
                            AddToMPReserve(choice);
                        }
                    }

                    else if (command_string.StartsWith("/achget"))
                    {
                        UnlockAllAchievements();
                    }

                    else if (command_string.StartsWith("/godmode"))
                    {
                        godmode = true;
                        Log("Godmode enabled!");
                    }

                    else if (command_string.StartsWith("/xdamage"))
                    {
                        xdamage = true;
                        Log("Extra Damage (instakill) enabled!");
                    }

                    else if (command_string.StartsWith("/flight"))
                    {
                        string[] strings = command_text.text.Split();
                        bool choice = bool.Parse(strings[1]);
                        flight = choice;
                        Log($"Flight set to {choice} NOTE: THIS COMMAND IS INACTIVE RN");
                    }

                    else if (command_string.StartsWith("/stags"))
                    {


                        Log("Unlocking all stag stations!");
                        GameManager.instance.playerData.openedTown = true;
                        GameManager.instance.playerData.openedTownBuilding = true;
                        GameManager.instance.playerData.openedCrossroads = true;
                        GameManager.instance.playerData.openedGreenpath = true;
                        GameManager.instance.playerData.openedRuins1 = true;
                        GameManager.instance.playerData.openedRuins2 = true;
                        GameManager.instance.playerData.openedFungalWastes = true;
                        GameManager.instance.playerData.openedRoyalGardens = true;
                        GameManager.instance.playerData.openedRestingGrounds = true;
                        GameManager.instance.playerData.openedDeepnest = true;
                        GameManager.instance.playerData.openedStagNest = true;
                        GameManager.instance.playerData.openedHiddenStation = true;
                        GameManager.instance.playerData.gladeDoorOpened = true;
                        GameManager.instance.playerData.troupeInTown = true;
                        
                    }

                    else if (command_string.StartsWith("/addmoney"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        if (choice < 0)
                        {
                            choice = 0;
                            Log("You can't add a negative value!");
                        }
                        GameManager.instance.playerData.bankerBalance += choice;
                        //GameManager.instance.playerData.bankerBalance += choice;
                        Log($"Added {choice} to Banker Balance\nBanker Balance: {GameManager.instance.playerData.bankerBalance}");
                    }

                    else if (command_string.StartsWith("/addgeo"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        
                        AddGeo(choice);
                        

                    }

                    else if (command_string.StartsWith("/addcharm"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        AddCharm(choice);
                    }




                    selected = false;
                    command_text.text = "Press / for commands";
                    command_string = "";
                }

                if (selected)
                {
                    //I've tried like four different ways to do this without an if/else block for the _entire alphabet and all 10 numbers_, it's already taken too long so fine I guess here we go-
                    
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        command_string += "a";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.B))
                    {
                        command_string += "b";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.C))
                    {
                        command_string += "c";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        command_string += "d";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        command_string += "e";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.F))
                    {
                        command_string += "f";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.G))
                    {
                        command_string += "g";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.H))
                    {
                        command_string += "h";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.I))
                    {
                        command_string += "i";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.J))
                    {
                        command_string += "j";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.K))
                    {
                        command_string += "k";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.L))
                    {
                        command_string += "l";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.M))
                    {
                        command_string += "m";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.N))
                    {
                        command_string += "n";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.O))
                    {
                        command_string += "o";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.P))
                    {
                        command_string += "p";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Q))
                    {
                        command_string += "q";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.R))
                    {
                        command_string += "r";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.S))
                    {
                        command_string += "s";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.T))
                    {
                        command_string += "t";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.U))
                    {
                        command_string += "u";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.V))
                    {
                        command_string += "v";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        command_string += "w";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        command_string += "x";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Y))
                    {
                        command_string += "y";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Z))
                    {
                        command_string += "z";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        command_string += "0";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        command_string += "1";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        command_string += "2";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        command_string += "3";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        command_string += "4";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        command_string += "5";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        command_string += "6";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        command_string += "7";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha8))
                    {
                        command_string += "8";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha9))
                    {
                        command_string += "9";
                        command_text.text = command_string;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        command_string += " ";
                        command_text.text = command_string;
                    }

                }
            }
        }
    }
}
