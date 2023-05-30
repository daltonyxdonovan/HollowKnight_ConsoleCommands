using BepInEx;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using unity's scene management
using UnityEngine.SceneManagement;
using TMPro;

namespace ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        Canvas canvas;
        TextMeshProUGUI popup_text;
        int popup_timer = 0;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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

                //bring it to the front of the 2d screen
                canvas.planeDistance = 1;
                //set it to the 'uGUI' layer
                canvas.gameObject.layer = 5;

                //create an input_field and set it's parent to canvas
                TMP_InputField input_field = new GameObject("Input Field", typeof(TMP_InputField)).GetComponent<TMP_InputField>();
                input_field.transform.SetParent(canvas.transform);
                //get size of screen
                float width = Screen.width;
                float height = Screen.height;
                //set the input_field to be in the bottom left corner of the screen
                input_field.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                input_field.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                input_field.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                //set the size of the input_field to be 1/4 of the screen and 1/8 of the screen, respectively
                input_field.GetComponent<RectTransform>().sizeDelta = new Vector2(width / 4, height / 8);
                //set the text to be in the left of the input_field
                input_field.textComponent.alignment = TextAlignmentOptions.Left;
                //set the font size to be 1/4 of the input_field's height
                input_field.textComponent.fontSize = input_field.GetComponent<RectTransform>().sizeDelta.y / 4;
                //set the font color to be white
                input_field.textComponent.color = Color.white;
                






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


                if (Input.GetKey(KeyCode.Return))
                {
                    Log("Added to max MP reserve.");
                    PlayerData.instance.MPReserveMax += 10;
                    PlayerData.instance.MPReserveCap += 10;
                    PlayerData.instance.MPReserve += 10;
                    PlayerData.instance.AddToMaxMPReserve(10);
                    Log($"MP = {PlayerData.instance.MPReserve}\nMaxMPreserve = {PlayerData.instance.MPReserveMax}");
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    Log("added to health");
                    PlayerData.instance.maxHealthBase += 10;
                    PlayerData.instance.maxHealth += 10;
                    PlayerData.instance.AddHealth(10);
                    PlayerData.instance.AddToMaxHealth(10);
                    Log($"Health = {PlayerData.instance.health}\nMaxHealth = {PlayerData.instance.maxHealth}");
                }

                if (Input.GetKey(KeyCode.F1))
                {
                    Log("Unlocking All Achievements");
                    //find current achievement handler
                    AchievementHandler achievementHandler = FindObjectOfType<AchievementHandler>();

                    //I'm not sure which one actually worked, but I'm leaving both in because I don't wanna test it. It works, and that's all that matters.
                    achievementHandler.AwardAllAchievements();

                    foreach (Achievement achievement in achievementHandler.achievementsList.achievements)
                    {
                        achievementHandler.AwardAchievementToPlayer(achievement.key);
                    }
                }
            }
        }
    }
}
