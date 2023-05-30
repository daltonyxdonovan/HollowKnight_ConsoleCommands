using BepInEx;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using unity's scene management
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

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
        bool selected = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void AddToMPReserve(int amount)
        {
            PlayerData.instance.MPReserveMax += amount;
            PlayerData.instance.MPReserveCap += amount;
            PlayerData.instance.MPReserve += amount;
            PlayerData.instance.AddToMaxMPReserve(amount);
            Log($"Added {amount} to MP Reserve\nMP = {PlayerData.instance.MPReserve}\nMaxMPreserve = {PlayerData.instance.MPReserveMax}");
        }

        public void AddToHealthReserve(int amount)
        {
            PlayerData.instance.maxHealthBase += amount;
            PlayerData.instance.maxHealth += amount;
            PlayerData.instance.AddHealth(amount);
            PlayerData.instance.AddToMaxHealth(amount);
            Log($"Added {amount} to Health Reserve\nHealth = {PlayerData.instance.health}\nMaxHealth = {PlayerData.instance.maxHealth}");
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
                command_text.alignment = TextAlignmentOptions.Center;
                command_text.fontSize = .5f;
                command_text.color = Color.white;
                command_text.text = "Press / for commands";
                selected = false;
                command_string = "";
                //set position to (-12,8)
                command_text.rectTransform.position = new Vector3(-12.5f, -8, 0);

                //bring it to the front of the 2d screen
                canvas.planeDistance = 1;
                //set it to the 'UI' layer
                canvas.gameObject.layer = 5;
            }
            if (canvas != null)
            {
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
                    }
                }

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (selected)
                    {
                        if (command_string.Length > 0)
                        {
                            command_string = command_string.Substring(0, command_string.Length - 1);
                            command_text.text = "/" + command_string;
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


                if (Input.GetKey(KeyCode.Return))
                {

                    if (command_string.StartsWith("/health"))
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

                    else if (command_string.StartsWith("/mp"))
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

                    else if (command_string.StartsWith("/ach_get"))
                    {
                        UnlockAllAchievements();
                    }





                }

                

                
            }
        }
    }
}
