using BepInEx;
using UnityEngine;
using TMPro;
using System.Xml.Serialization;
using GlobalEnums;
using System.Collections.Generic;

namespace ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        Canvas canvas;
        TextMeshProUGUI popup_text;
        TextMeshProUGUI command_text;
        TextMeshProUGUI debugText;
        string command_string = "";
        int popup_timer = 0;
        bool selected = false;
        bool godmode = false;
        bool xdamage = false;
        bool flight = false;
        //bool permadeath = false;
        bool showDebug = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} by Daltonyx is loaded!");
        }

        public string GetDebugText()
        {
            return $"Nail DMG: {GameManager.instance.playerData.nailDamage}\n" +
                   $"Beam DMG: {GameManager.instance.playerData.beamDamage}\n" +
                   $"Geo AMNT: {GameManager.instance.playerData.geo}\n" +
                   $"Health: {GameManager.instance.playerData.health}\n" +
                   $"maxHealth: {GameManager.instance.playerData.maxHealth}\n" +
                   $"infJump: {flight}\n" +
                   $"xDamage: {xdamage}\n" +
                   $"godMode: {godmode}";
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
            if (charmNum < 0 || charmNum > 39)
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

        public void Heal()
        {
            GameManager.instance.playerData.health = GameManager.instance.playerData.maxHealth;
            GameManager.instance.playerData.prevHealth = GameManager.instance.playerData.maxHealth;
        }

        public void AddToHealthReserve(int amount)
        {
            GameManager.instance.playerData.maxHealthBase += amount/2;
            GameManager.instance.playerData.maxHealth += amount/2;
            GameManager.instance.playerData.AddHealth(amount/2);
            GameManager.instance.playerData.AddToMaxHealth(amount/2);
            GameManager.instance.playerData.prevHealth += amount/2;
            GameManager.instance.playerData.health += amount/2;
            //GameManager.instance.playerData.blockerHits = 999;
            GameManager.instance.playerData.UpdateBlueHealth();
            Log($"Added {amount} to Health Reserve\nHealth = {GameManager.instance.playerData.health}\nMaxHealth = {GameManager.instance.playerData.maxHealth}");
        }

        public void SetHealth(int amount)
        {
            if (amount < 1) return;
            GameManager.instance.playerData.health = amount;
            GameManager.instance.playerData.prevHealth = amount;
            GameManager.instance.playerData.maxHealth = amount;
            GameManager.instance.playerData.maxHealthBase = amount;
            GameManager.instance.playerData.maxHealthCap = amount;
            
            GameManager.instance.playerData.UpdateBlueHealth();
            Log($"Set Health to {amount}");
        }

        public void Reset()
        {
            godmode = false;
            xdamage = false;
            flight = false;
            MidReset();
            GameManager.instance.playerData.maxHealth = 5;
            GameManager.instance.playerData.health = 5;
            GameManager.instance.playerData.prevHealth = 5;
            GameManager.instance.playerData.MPCharge = 10;
            GameManager.instance.playerData.blockerHits = 0;
            GameManager.instance.playerData.isInvincible = false;
            GameManager.instance.playerData.infiniteAirJump = false;
            GameManager.instance.playerData.UpdateBlueHealth();
        }

        public void UpdateValues()
        {
            
            GameManager.instance.playerData.UpdateBlueHealth();


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

        public void MidReset()
        {
            GameManager.instance.playerData.health = 5;
            GameManager.instance.playerData.maxHealth = 5;
            GameManager.instance.playerData.maxHealthBase = 5;
            GameManager.instance.playerData.healthBlue = 0;
            GameManager.instance.playerData.joniHealthBlue = 0;
            GameManager.instance.playerData.maxHealthCap = 9;
            GameManager.instance.playerData.prevHealth = GameManager.instance.playerData.health;
            GameManager.instance.playerData.blockerHits = 4;
            GameManager.instance.playerData.maxMP = 99;
            GameManager.instance.playerData.MPCharge = 0;
            GameManager.instance.playerData.MPReserve = 0;
            GameManager.instance.playerData.MPReserveMax = 0;
            GameManager.instance.playerData.nailDamage = 5;
            GameManager.instance.playerData.nailRange = 0;
            GameManager.instance.playerData.beamDamage = 0;
        }

        public void HardReset()
        {
            GameManager.instance.playerData.openingCreditsPlayed = false;
            GameManager.instance.playerData.playTime = 0f;
            GameManager.instance.playerData.completionPercent = 0f;
            GameManager.instance.playerData.permadeathMode = 0;
            GameManager.instance.playerData.version = "1.5.78.11833";
            GameManager.instance.playerData.awardAllAchievements = false;
            GameManager.instance.playerData.health = 5;
            GameManager.instance.playerData.maxHealth = 5;
            GameManager.instance.playerData.maxHealthBase = 5;
            GameManager.instance.playerData.healthBlue = 0;
            GameManager.instance.playerData.joniHealthBlue = 0;
            GameManager.instance.playerData.damagedBlue = false;
            GameManager.instance.playerData.heartPieces = 0;
            GameManager.instance.playerData.heartPieceCollected = false;
            GameManager.instance.playerData.maxHealthCap = 9;
            GameManager.instance.playerData.heartPieceMax = false;
            GameManager.instance.playerData.prevHealth = GameManager.instance.playerData.health;
            GameManager.instance.playerData.blockerHits = 4;
            GameManager.instance.playerData.firstGeo = false;
            GameManager.instance.playerData.geo = 0;
            GameManager.instance.playerData.maxMP = 99;
            GameManager.instance.playerData.MPCharge = 0;
            GameManager.instance.playerData.MPReserve = 0;
            GameManager.instance.playerData.MPReserveMax = 0;
            GameManager.instance.playerData.soulLimited = false;
            GameManager.instance.playerData.focusMP_amount = 33;
            GameManager.instance.playerData.vesselFragments = 0;
            GameManager.instance.playerData.vesselFragmentCollected = false;
            GameManager.instance.playerData.MPReserveCap = 99;
            GameManager.instance.playerData.vesselFragmentMax = false;
            GameManager.instance.playerData.atBench = false;
            GameManager.instance.playerData.respawnScene = "Tutorial_01";
            GameManager.instance.playerData.respawnMarkerName = "Death Respawn Marker";
            GameManager.instance.playerData.mapZone = MapZone.KINGS_PASS;
            GameManager.instance.playerData.respawnType = 0;
            GameManager.instance.playerData.respawnFacingRight = false;
            GameManager.instance.playerData.hazardRespawnFacingRight = false;
            GameManager.instance.playerData.shadeScene = "None";
            GameManager.instance.playerData.shadeMapZone = "";
            GameManager.instance.playerData.shadePositionX = -999f;
            GameManager.instance.playerData.shadePositionY = -999f;
            GameManager.instance.playerData.shadeHealth = 0;
            GameManager.instance.playerData.shadeMP = 0;
            GameManager.instance.playerData.shadeFireballLevel = 0;
            GameManager.instance.playerData.shadeQuakeLevel = 0;
            GameManager.instance.playerData.shadeScreamLevel = 0;
            GameManager.instance.playerData.shadeSpecialType = 0;
            GameManager.instance.playerData.shadeMapPos = new Vector3(0f, 0f, 0f);
            GameManager.instance.playerData.dreamgateMapPos = new Vector3(0f, 0f, 0f);
            GameManager.instance.playerData.geoPool = 0;
            GameManager.instance.playerData.nailDamage = 5;
            GameManager.instance.playerData.nailRange = 0;
            GameManager.instance.playerData.beamDamage = 0;
            GameManager.instance.playerData.canDash = false;
            GameManager.instance.playerData.canBackDash = false;
            GameManager.instance.playerData.canWallJump = false;
            GameManager.instance.playerData.canSuperDash = false;
            GameManager.instance.playerData.canShadowDash = false;
            GameManager.instance.playerData.hasSpell = false;
            GameManager.instance.playerData.fireballLevel = 0;
            GameManager.instance.playerData.quakeLevel = 0;
            GameManager.instance.playerData.screamLevel = 0;
            GameManager.instance.playerData.hasNailArt = false;
            GameManager.instance.playerData.hasCyclone = false;
            GameManager.instance.playerData.hasDashSlash = false;
            GameManager.instance.playerData.hasUpwardSlash = false;
            GameManager.instance.playerData.hasAllNailArts = false;
            GameManager.instance.playerData.hasDreamNail = false;
            GameManager.instance.playerData.hasDreamGate = false;
            GameManager.instance.playerData.dreamNailUpgraded = false;
            GameManager.instance.playerData.dreamOrbs = 0;
            GameManager.instance.playerData.dreamOrbsSpent = 0;
            GameManager.instance.playerData.dreamGateScene = "";
            GameManager.instance.playerData.dreamGateX = 0f;
            GameManager.instance.playerData.dreamGateY = 0f;
            GameManager.instance.playerData.hasDash = false;
            GameManager.instance.playerData.hasWalljump = false;
            GameManager.instance.playerData.hasSuperDash = false;
            GameManager.instance.playerData.hasShadowDash = false;
            GameManager.instance.playerData.hasAcidArmour = false;
            GameManager.instance.playerData.hasDoubleJump = false;
            GameManager.instance.playerData.hasLantern = false;
            GameManager.instance.playerData.hasTramPass = false;
            GameManager.instance.playerData.hasQuill = false;
            GameManager.instance.playerData.hasCityKey = false;
            GameManager.instance.playerData.hasSlykey = false;
            GameManager.instance.playerData.gaveSlykey = false;
            GameManager.instance.playerData.hasWhiteKey = false;
            GameManager.instance.playerData.usedWhiteKey = false;
            GameManager.instance.playerData.hasMenderKey = false;
            GameManager.instance.playerData.hasWaterwaysKey = false;
            GameManager.instance.playerData.hasSpaKey = false;
            GameManager.instance.playerData.hasLoveKey = false;
            GameManager.instance.playerData.hasKingsBrand = false;
            GameManager.instance.playerData.hasXunFlower = false;
            GameManager.instance.playerData.ghostCoins = 0;
            GameManager.instance.playerData.ore = 0;
            GameManager.instance.playerData.foundGhostCoin = false;
            GameManager.instance.playerData.trinket1 = 0;
            GameManager.instance.playerData.foundTrinket1 = false;
            GameManager.instance.playerData.trinket2 = 0;
            GameManager.instance.playerData.foundTrinket2 = false;
            GameManager.instance.playerData.trinket3 = 0;
            GameManager.instance.playerData.foundTrinket3 = false;
            GameManager.instance.playerData.trinket4 = 0;
            GameManager.instance.playerData.foundTrinket4 = false;
            GameManager.instance.playerData.noTrinket1 = false;
            GameManager.instance.playerData.noTrinket2 = false;
            GameManager.instance.playerData.noTrinket3 = false;
            GameManager.instance.playerData.noTrinket4 = false;
            GameManager.instance.playerData.soldTrinket1 = 0;
            GameManager.instance.playerData.soldTrinket2 = 0;
            GameManager.instance.playerData.soldTrinket3 = 0;
            GameManager.instance.playerData.soldTrinket4 = 0;
            GameManager.instance.playerData.simpleKeys = 0;
            GameManager.instance.playerData.rancidEggs = 0;
            GameManager.instance.playerData.notchShroomOgres = false;
            GameManager.instance.playerData.notchFogCanyon = false;
            GameManager.instance.playerData.gotLurkerKey = false;
            GameManager.instance.playerData.gMap_doorX = 0f;
            GameManager.instance.playerData.gMap_doorY = 0f;
            GameManager.instance.playerData.gMap_doorScene = "";
            GameManager.instance.playerData.gMap_doorMapZone = "";
            GameManager.instance.playerData.gMap_doorOriginOffsetX = 0f;
            GameManager.instance.playerData.gMap_doorOriginOffsetY = 0f;
            GameManager.instance.playerData.gMap_doorSceneWidth = 0f;
            GameManager.instance.playerData.gMap_doorSceneHeight = 0f;
            GameManager.instance.playerData.guardiansDefeated = 0;
            GameManager.instance.playerData.lurienDefeated = false;
            GameManager.instance.playerData.hegemolDefeated = false;
            GameManager.instance.playerData.monomonDefeated = false;
            GameManager.instance.playerData.maskBrokenLurien = false;
            GameManager.instance.playerData.maskBrokenHegemol = false;
            GameManager.instance.playerData.maskBrokenMonomon = false;
            GameManager.instance.playerData.maskToBreak = 0;
            GameManager.instance.playerData.elderbug = 0;
            GameManager.instance.playerData.metElderbug = false;
            GameManager.instance.playerData.elderbugReintro = false;
            GameManager.instance.playerData.elderbugHistory = 0;
            GameManager.instance.playerData.elderbugHistory1 = false;
            GameManager.instance.playerData.elderbugHistory2 = false;
            GameManager.instance.playerData.elderbugHistory3 = false;
            GameManager.instance.playerData.elderbugSpeechSly = false;
            GameManager.instance.playerData.elderbugSpeechStation = false;
            GameManager.instance.playerData.elderbugSpeechEggTemple = false;
            GameManager.instance.playerData.elderbugSpeechMapShop = false;
            GameManager.instance.playerData.elderbugSpeechBretta = false;
            GameManager.instance.playerData.elderbugSpeechJiji = false;
            GameManager.instance.playerData.elderbugSpeechMinesLift = false;
            GameManager.instance.playerData.elderbugSpeechKingsPass = false;
            GameManager.instance.playerData.elderbugSpeechInfectedCrossroads = false;
            GameManager.instance.playerData.elderbugSpeechFinalBossDoor = false;
            GameManager.instance.playerData.elderbugRequestedFlower = false;
            GameManager.instance.playerData.elderbugGaveFlower = false;
            GameManager.instance.playerData.elderbugFirstCall = false;
            GameManager.instance.playerData.metQuirrel = false;
            GameManager.instance.playerData.quirrelEggTemple = 0;
            GameManager.instance.playerData.quirrelLeftEggTemple = false;
            GameManager.instance.playerData.quirrelSlugShrine = 0;
            GameManager.instance.playerData.quirrelRuins = 0;
            GameManager.instance.playerData.quirrelMines = 0;
            GameManager.instance.playerData.quirrelLeftStation = false;
            GameManager.instance.playerData.quirrelCityEncountered = false;
            GameManager.instance.playerData.quirrelCityLeft = false;
            GameManager.instance.playerData.quirrelMinesEncountered = false;
            GameManager.instance.playerData.quirrelMinesLeft = false;
            GameManager.instance.playerData.visitedDeepnestSpa = false;
            GameManager.instance.playerData.quirrelSpaReady = false;
            GameManager.instance.playerData.quirrelSpaEncountered = false;
            GameManager.instance.playerData.quirrelArchiveEncountered = false;
            GameManager.instance.playerData.quirrelEpilogueCompleted = false;
            GameManager.instance.playerData.quirrelMantisEncountered = false;
            GameManager.instance.playerData.enteredMantisLordArea = false;
            GameManager.instance.playerData.metRelicDealer = false;
            GameManager.instance.playerData.metRelicDealerShop = false;
            GameManager.instance.playerData.marmOutside = false;
            GameManager.instance.playerData.marmOutsideConvo = false;
            GameManager.instance.playerData.marmConvo1 = false;
            GameManager.instance.playerData.marmConvo2 = false;
            GameManager.instance.playerData.marmConvo3 = false;
            GameManager.instance.playerData.marmConvoNailsmith = false;
            GameManager.instance.playerData.cornifer = 0;
            GameManager.instance.playerData.metCornifer = false;
            GameManager.instance.playerData.corniferIntroduced = false;
            GameManager.instance.playerData.corniferAtHome = false;
            GameManager.instance.playerData.corn_crossroadsEncountered = false;
            GameManager.instance.playerData.corn_crossroadsLeft = false;
            GameManager.instance.playerData.corn_greenpathEncountered = false;
            GameManager.instance.playerData.corn_greenpathLeft = false;
            GameManager.instance.playerData.corn_fogCanyonEncountered = false;
            GameManager.instance.playerData.corn_fogCanyonLeft = false;
            GameManager.instance.playerData.corn_fungalWastesEncountered = false;
            GameManager.instance.playerData.corn_fungalWastesLeft = false;
            GameManager.instance.playerData.corn_cityEncountered = false;
            GameManager.instance.playerData.corn_cityLeft = false;
            GameManager.instance.playerData.corn_waterwaysEncountered = false;
            GameManager.instance.playerData.corn_waterwaysLeft = false;
            GameManager.instance.playerData.corn_minesEncountered = false;
            GameManager.instance.playerData.corn_minesLeft = false;
            GameManager.instance.playerData.corn_cliffsEncountered = false;
            GameManager.instance.playerData.corn_cliffsLeft = false;
            GameManager.instance.playerData.corn_deepnestEncountered = false;
            GameManager.instance.playerData.corn_deepnestLeft = false;
            GameManager.instance.playerData.corn_deepnestMet1 = false;
            GameManager.instance.playerData.corn_deepnestMet2 = false;
            GameManager.instance.playerData.corn_outskirtsEncountered = false;
            GameManager.instance.playerData.corn_outskirtsLeft = false;
            GameManager.instance.playerData.corn_royalGardensEncountered = false;
            GameManager.instance.playerData.corn_royalGardensLeft = false;
            GameManager.instance.playerData.corn_abyssEncountered = false;
            GameManager.instance.playerData.corn_abyssLeft = false;
            GameManager.instance.playerData.metIselda = false;
            GameManager.instance.playerData.iseldaCorniferHomeConvo = false;
            GameManager.instance.playerData.iseldaConvo1 = false;
            GameManager.instance.playerData.brettaRescued = false;
            GameManager.instance.playerData.brettaPosition = 0;
            GameManager.instance.playerData.brettaState = 0;
            GameManager.instance.playerData.brettaSeenBench = false;
            GameManager.instance.playerData.brettaSeenBed = false;
            GameManager.instance.playerData.brettaSeenBenchDiary = false;
            GameManager.instance.playerData.brettaSeenBedDiary = false;
            GameManager.instance.playerData.brettaLeftTown = false;
            GameManager.instance.playerData.slyRescued = false;
            GameManager.instance.playerData.slyBeta = false;
            GameManager.instance.playerData.metSlyShop = false;
            GameManager.instance.playerData.gotSlyCharm = false;
            GameManager.instance.playerData.slyShellFrag1 = false;
            GameManager.instance.playerData.slyShellFrag2 = false;
            GameManager.instance.playerData.slyShellFrag3 = false;
            GameManager.instance.playerData.slyShellFrag4 = false;
            GameManager.instance.playerData.slyVesselFrag1 = false;
            GameManager.instance.playerData.slyVesselFrag2 = false;
            GameManager.instance.playerData.slyVesselFrag3 = false;
            GameManager.instance.playerData.slyVesselFrag4 = false;
            GameManager.instance.playerData.slyNotch1 = false;
            GameManager.instance.playerData.slyNotch2 = false;
            GameManager.instance.playerData.slySimpleKey = false;
            GameManager.instance.playerData.slyRancidEgg = false;
            GameManager.instance.playerData.slyConvoNailArt = false;
            GameManager.instance.playerData.slyConvoMapper = false;
            GameManager.instance.playerData.slyConvoNailHoned = false;
            GameManager.instance.playerData.jijiDoorUnlocked = false;
            GameManager.instance.playerData.jijiMet = false;
            GameManager.instance.playerData.jijiShadeOffered = false;
            GameManager.instance.playerData.jijiShadeCharmConvo = false;
            GameManager.instance.playerData.metJinn = false;
            GameManager.instance.playerData.jinnConvo1 = false;
            GameManager.instance.playerData.jinnConvo2 = false;
            GameManager.instance.playerData.jinnConvo3 = false;
            GameManager.instance.playerData.jinnConvoKingBrand = false;
            GameManager.instance.playerData.jinnConvoShadeCharm = false;
            GameManager.instance.playerData.jinnEggsSold = 0;
            GameManager.instance.playerData.zote = 0;
            GameManager.instance.playerData.zoteDead = false;
            GameManager.instance.playerData.zoteDeathPos = 0;
            GameManager.instance.playerData.zoteRescuedBuzzer = false;
            GameManager.instance.playerData.zoteSpokenCity = false;
            GameManager.instance.playerData.zoteLeftCity = false;
            GameManager.instance.playerData.zoteRescuedDeepnest = false;
            GameManager.instance.playerData.zoteDefeated = false;
            GameManager.instance.playerData.zoteTrappedDeepnest = false;
            GameManager.instance.playerData.zoteSpokenColosseum = false;
            GameManager.instance.playerData.zotePrecept = 1;
            GameManager.instance.playerData.zoteTownConvo = 0;
            GameManager.instance.playerData.shaman = 0;
            GameManager.instance.playerData.shamanScreamConvo = false;
            GameManager.instance.playerData.shamanQuakeConvo = false;
            GameManager.instance.playerData.shamanFireball2Convo = false;
            GameManager.instance.playerData.shamanScream2Convo = false;
            GameManager.instance.playerData.shamanQuake2Convo = false;
            GameManager.instance.playerData.metMiner = false;
            GameManager.instance.playerData.miner = 0;
            GameManager.instance.playerData.minerEarly = 0;
            GameManager.instance.playerData.hornetGreenpath = 0;
            GameManager.instance.playerData.hornetFung = 0;
            GameManager.instance.playerData.hornet_f19 = false;
            GameManager.instance.playerData.hornetFountainEncounter = false;
            GameManager.instance.playerData.hornetCityBridge_ready = false;
            GameManager.instance.playerData.hornetCityBridge_completed = false;
            GameManager.instance.playerData.hornetAbyssEncounter = false;
            GameManager.instance.playerData.hornetDenEncounter = false;
            GameManager.instance.playerData.metMoth = false;
            GameManager.instance.playerData.ignoredMoth = false;
            GameManager.instance.playerData.gladeDoorOpened = false;
            GameManager.instance.playerData.mothDeparted = false;
            GameManager.instance.playerData.completedRGDreamPlant = false;
            GameManager.instance.playerData.dreamReward1 = false;
            GameManager.instance.playerData.dreamReward2 = false;
            GameManager.instance.playerData.dreamReward3 = false;
            GameManager.instance.playerData.dreamReward4 = false;
            GameManager.instance.playerData.dreamReward5 = false;
            GameManager.instance.playerData.dreamReward5b = false;
            GameManager.instance.playerData.dreamReward6 = false;
            GameManager.instance.playerData.dreamReward7 = false;
            GameManager.instance.playerData.dreamReward8 = false;
            GameManager.instance.playerData.dreamReward9 = false;
            GameManager.instance.playerData.dreamMothConvo1 = false;
            GameManager.instance.playerData.bankerAccountPurchased = false;
            GameManager.instance.playerData.metBanker = false;
            GameManager.instance.playerData.bankerBalance = 0;
            GameManager.instance.playerData.bankerDeclined = false;
            GameManager.instance.playerData.bankerTheftCheck = false;
            GameManager.instance.playerData.bankerTheft = 0;
            GameManager.instance.playerData.bankerSpaMet = false;
            GameManager.instance.playerData.metGiraffe = false;
            GameManager.instance.playerData.metCharmSlug = false;
            GameManager.instance.playerData.salubraNotch1 = false;
            GameManager.instance.playerData.salubraNotch2 = false;
            GameManager.instance.playerData.salubraNotch3 = false;
            GameManager.instance.playerData.salubraNotch4 = false;
            GameManager.instance.playerData.salubraBlessing = false;
            GameManager.instance.playerData.salubraConvoCombo = false;
            GameManager.instance.playerData.salubraConvoOvercharm = false;
            GameManager.instance.playerData.salubraConvoTruth = false;
            GameManager.instance.playerData.cultistTransformed = false;
            GameManager.instance.playerData.metNailsmith = false;
            GameManager.instance.playerData.honedNail = false;
            GameManager.instance.playerData.nailSmithUpgrades = 0;
            GameManager.instance.playerData.nailsmithCliff = false;
            GameManager.instance.playerData.nailsmithKilled = false;
            GameManager.instance.playerData.nailsmithSpared = false;
            GameManager.instance.playerData.nailsmithKillSpeech = false;
            GameManager.instance.playerData.nailsmithSheo = false;
            GameManager.instance.playerData.nailsmithConvoArt = false;
            GameManager.instance.playerData.metNailmasterMato = false;
            GameManager.instance.playerData.metNailmasterSheo = false;
            GameManager.instance.playerData.metNailmasterOro = false;
            GameManager.instance.playerData.matoConvoSheo = false;
            GameManager.instance.playerData.matoConvoOro = false;
            GameManager.instance.playerData.matoConvoSly = false;
            GameManager.instance.playerData.sheoConvoMato = false;
            GameManager.instance.playerData.sheoConvoOro = false;
            GameManager.instance.playerData.sheoConvoSly = false;
            GameManager.instance.playerData.sheoConvoNailsmith = false;
            GameManager.instance.playerData.oroConvoSheo = false;
            GameManager.instance.playerData.oroConvoMato = false;
            GameManager.instance.playerData.oroConvoSly = false;
            GameManager.instance.playerData.hunterRoared = false;
            GameManager.instance.playerData.metHunter = false;
            GameManager.instance.playerData.hunterRewardOffered = false;
            GameManager.instance.playerData.huntersMarkOffered = false;
            GameManager.instance.playerData.hasHuntersMark = false;
            GameManager.instance.playerData.metLegEater = false;
            GameManager.instance.playerData.paidLegEater = false;
            GameManager.instance.playerData.refusedLegEater = false;
            GameManager.instance.playerData.legEaterBrokenConvo = false;
            GameManager.instance.playerData.legEaterDungConvo = false;
            GameManager.instance.playerData.legEaterInfectedCrossroadConvo = false;
            GameManager.instance.playerData.legEaterBoughtConvo = false;
            GameManager.instance.playerData.legEaterConvo1 = false;
            GameManager.instance.playerData.legEaterConvo2 = false;
            GameManager.instance.playerData.legEaterConvo3 = false;
            GameManager.instance.playerData.legEaterGoldConvo = false;
            GameManager.instance.playerData.legEaterLeft = false;
            GameManager.instance.playerData.tukMet = false;
            GameManager.instance.playerData.tukEggPrice = 0;
            GameManager.instance.playerData.tukDungEgg = false;
            GameManager.instance.playerData.metEmilitia = false;
            GameManager.instance.playerData.emilitiaKingsBrandConvo = false;
            GameManager.instance.playerData.metCloth = false;
            GameManager.instance.playerData.clothEnteredTramRoom = false;
            GameManager.instance.playerData.savedCloth = false;
            GameManager.instance.playerData.clothEncounteredQueensGarden = false;
            GameManager.instance.playerData.clothKilled = false;
            GameManager.instance.playerData.clothInTown = false;
            GameManager.instance.playerData.clothLeftTown = false;
            GameManager.instance.playerData.clothGhostSpoken = false;
            GameManager.instance.playerData.bigCatHitTail = false;
            GameManager.instance.playerData.bigCatHitTailConvo = false;
            GameManager.instance.playerData.bigCatMeet = false;
            GameManager.instance.playerData.bigCatTalk1 = false;
            GameManager.instance.playerData.bigCatTalk2 = false;
            GameManager.instance.playerData.bigCatTalk3 = false;
            GameManager.instance.playerData.bigCatKingsBrandConvo = false;
            GameManager.instance.playerData.bigCatShadeConvo = false;
            GameManager.instance.playerData.tisoEncounteredTown = false;
            GameManager.instance.playerData.tisoEncounteredBench = false;
            GameManager.instance.playerData.tisoEncounteredLake = false;
            GameManager.instance.playerData.tisoEncounteredColosseum = false;
            GameManager.instance.playerData.tisoShieldConvo = false;
            GameManager.instance.playerData.tisoDead = false;
            GameManager.instance.playerData.mossCultist = 0;
            GameManager.instance.playerData.maskmakerMet = false;
            GameManager.instance.playerData.maskmakerConvo1 = false;
            GameManager.instance.playerData.maskmakerConvo2 = false;
            GameManager.instance.playerData.maskmakerUnmasked1 = false;
            GameManager.instance.playerData.maskmakerUnmasked2 = false;
            GameManager.instance.playerData.maskmakerShadowDash = false;
            GameManager.instance.playerData.maskmakerKingsBrand = false;
            GameManager.instance.playerData.dungDefenderConvo1 = false;
            GameManager.instance.playerData.dungDefenderConvo2 = false;
            GameManager.instance.playerData.dungDefenderConvo3 = false;
            GameManager.instance.playerData.dungDefenderCharmConvo = false;
            GameManager.instance.playerData.dungDefenderIsmaConvo = false;
            GameManager.instance.playerData.dungDefenderAwakeConvo = false;
            GameManager.instance.playerData.dungDefenderAwoken = false;
            GameManager.instance.playerData.dungDefenderLeft = false;
            GameManager.instance.playerData.midwifeMet = false;
            GameManager.instance.playerData.midwifeConvo1 = false;
            GameManager.instance.playerData.midwifeConvo2 = false;
            GameManager.instance.playerData.metQueen = false;
            GameManager.instance.playerData.queenTalk1 = false;
            GameManager.instance.playerData.queenTalk2 = false;
            GameManager.instance.playerData.queenDung1 = false;
            GameManager.instance.playerData.queenDung2 = false;
            GameManager.instance.playerData.queenHornet = false;
            GameManager.instance.playerData.queenTalkExtra = false;
            GameManager.instance.playerData.gotQueenFragment = false;
            GameManager.instance.playerData.gotKingFragment = false;
            GameManager.instance.playerData.metXun = false;
            GameManager.instance.playerData.xunFlowerBroken = false;
            GameManager.instance.playerData.xunFlowerBrokeTimes = 0;
            GameManager.instance.playerData.xunFlowerGiven = false;
            GameManager.instance.playerData.xunRewardGiven = false;
            GameManager.instance.playerData.xunFailedConvo1 = false;
            GameManager.instance.playerData.xunFailedConvo2 = true;
            GameManager.instance.playerData.menderState = 0;
            GameManager.instance.playerData.menderSignBroken = false;
            GameManager.instance.playerData.allBelieverTabletsDestroyed = false;
            GameManager.instance.playerData.mrMushroomState = 0;
            GameManager.instance.playerData.openedMapperShop = false;
            GameManager.instance.playerData.openedSlyShop = false;
            GameManager.instance.playerData.metStag = false;
            GameManager.instance.playerData.travelling = false;
            GameManager.instance.playerData.stagPosition = -1;
            GameManager.instance.playerData.stationsOpened = 0;
            GameManager.instance.playerData.stagConvoTram = false;
            GameManager.instance.playerData.stagConvoTiso = false;
            GameManager.instance.playerData.stagRemember1 = false;
            GameManager.instance.playerData.stagRemember2 = false;
            GameManager.instance.playerData.stagRemember3 = false;
            GameManager.instance.playerData.stagEggInspected = false;
            GameManager.instance.playerData.stagHopeConvo = false;
            GameManager.instance.playerData.nextScene = "";
            GameManager.instance.playerData.littleFoolMet = false;
            GameManager.instance.playerData.ranAway = false;
            GameManager.instance.playerData.seenColosseumTitle = false;
            GameManager.instance.playerData.colosseumBronzeOpened = false;
            GameManager.instance.playerData.colosseumBronzeCompleted = false;
            GameManager.instance.playerData.colosseumSilverOpened = false;
            GameManager.instance.playerData.colosseumSilverCompleted = false;
            GameManager.instance.playerData.colosseumGoldOpened = false;
            GameManager.instance.playerData.colosseumGoldCompleted = false;
            GameManager.instance.playerData.openedTown = true;
            GameManager.instance.playerData.openedTownBuilding = false;
            GameManager.instance.playerData.openedCrossroads = false;
            GameManager.instance.playerData.openedGreenpath = false;
            GameManager.instance.playerData.openedFungalWastes = false;
            GameManager.instance.playerData.openedRuins1 = false;
            GameManager.instance.playerData.openedRuins2 = false;
            GameManager.instance.playerData.openedRoyalGardens = false;
            GameManager.instance.playerData.openedRestingGrounds = false;
            GameManager.instance.playerData.openedDeepnest = false;
            GameManager.instance.playerData.openedStagNest = false;
            GameManager.instance.playerData.openedHiddenStation = false;
            GameManager.instance.playerData.dreamReturnScene = "Dream_NailCollection";
            GameManager.instance.playerData.charmSlots = 3;
            GameManager.instance.playerData.charmSlotsFilled = 0;
            GameManager.instance.playerData.hasCharm = false;
            GameManager.instance.playerData.equippedCharms = new List<int>();
            GameManager.instance.playerData.charmBenchMsg = false;
            GameManager.instance.playerData.charmsOwned = 0;
            GameManager.instance.playerData.canOvercharm = false;
            GameManager.instance.playerData.overcharmed = false;
            GameManager.instance.playerData.gotCharm_1 = false;
            GameManager.instance.playerData.equippedCharm_1 = false;
            GameManager.instance.playerData.charmCost_1 = 1;
            GameManager.instance.playerData.newCharm_1 = true;
            GameManager.instance.playerData.gotCharm_2 = false;
            GameManager.instance.playerData.equippedCharm_2 = false;
            GameManager.instance.playerData.charmCost_2 = 1;
            GameManager.instance.playerData.newCharm_2 = true;
            GameManager.instance.playerData.gotCharm_3 = false;
            GameManager.instance.playerData.equippedCharm_3 = false;
            GameManager.instance.playerData.charmCost_3 = 1;
            GameManager.instance.playerData.newCharm_3 = true;
            GameManager.instance.playerData.gotCharm_4 = false;
            GameManager.instance.playerData.equippedCharm_4 = false;
            GameManager.instance.playerData.charmCost_4 = 2;
            GameManager.instance.playerData.newCharm_4 = true;
            GameManager.instance.playerData.gotCharm_5 = false;
            GameManager.instance.playerData.equippedCharm_5 = false;
            GameManager.instance.playerData.charmCost_5 = 2;
            GameManager.instance.playerData.newCharm_5 = true;
            GameManager.instance.playerData.gotCharm_6 = false;
            GameManager.instance.playerData.equippedCharm_6 = false;
            GameManager.instance.playerData.charmCost_6 = 2;
            GameManager.instance.playerData.newCharm_6 = true;
            GameManager.instance.playerData.gotCharm_7 = false;
            GameManager.instance.playerData.equippedCharm_7 = false;
            GameManager.instance.playerData.charmCost_7 = 3;
            GameManager.instance.playerData.newCharm_7 = true;
            GameManager.instance.playerData.gotCharm_8 = false;
            GameManager.instance.playerData.equippedCharm_8 = false;
            GameManager.instance.playerData.charmCost_8 = 2;
            GameManager.instance.playerData.newCharm_8 = true;
            GameManager.instance.playerData.gotCharm_9 = false;
            GameManager.instance.playerData.equippedCharm_9 = false;
            GameManager.instance.playerData.charmCost_9 = 3;
            GameManager.instance.playerData.newCharm_9 = true;
            GameManager.instance.playerData.gotCharm_10 = false;
            GameManager.instance.playerData.equippedCharm_10 = false;
            GameManager.instance.playerData.charmCost_10 = 1;
            GameManager.instance.playerData.newCharm_10 = true;
            GameManager.instance.playerData.gotCharm_11 = false;
            GameManager.instance.playerData.equippedCharm_11 = false;
            GameManager.instance.playerData.charmCost_11 = 3;
            GameManager.instance.playerData.newCharm_11 = true;
            GameManager.instance.playerData.gotCharm_12 = false;
            GameManager.instance.playerData.equippedCharm_12 = false;
            GameManager.instance.playerData.charmCost_12 = 1;
            GameManager.instance.playerData.newCharm_12 = true;
            GameManager.instance.playerData.gotCharm_13 = false;
            GameManager.instance.playerData.equippedCharm_13 = false;
            GameManager.instance.playerData.charmCost_13 = 3;
            GameManager.instance.playerData.newCharm_13 = true;
            GameManager.instance.playerData.gotCharm_14 = false;
            GameManager.instance.playerData.equippedCharm_14 = false;
            GameManager.instance.playerData.charmCost_14 = 1;
            GameManager.instance.playerData.newCharm_14 = true;
            GameManager.instance.playerData.gotCharm_15 = false;
            GameManager.instance.playerData.equippedCharm_15 = false;
            GameManager.instance.playerData.charmCost_15 = 2;
            GameManager.instance.playerData.newCharm_15 = true;
            GameManager.instance.playerData.gotCharm_16 = false;
            GameManager.instance.playerData.equippedCharm_16 = false;
            GameManager.instance.playerData.charmCost_16 = 2;
            GameManager.instance.playerData.newCharm_16 = true;
            GameManager.instance.playerData.gotCharm_17 = false;
            GameManager.instance.playerData.equippedCharm_17 = false;
            GameManager.instance.playerData.charmCost_17 = 1;
            GameManager.instance.playerData.newCharm_17 = true;
            GameManager.instance.playerData.gotCharm_18 = false;
            GameManager.instance.playerData.equippedCharm_18 = false;
            GameManager.instance.playerData.charmCost_18 = 2;
            GameManager.instance.playerData.newCharm_18 = true;
            GameManager.instance.playerData.gotCharm_19 = false;
            GameManager.instance.playerData.equippedCharm_19 = false;
            GameManager.instance.playerData.charmCost_19 = 3;
            GameManager.instance.playerData.newCharm_19 = true;
            GameManager.instance.playerData.gotCharm_20 = false;
            GameManager.instance.playerData.equippedCharm_20 = false;
            GameManager.instance.playerData.charmCost_20 = 2;
            GameManager.instance.playerData.newCharm_20 = true;
            GameManager.instance.playerData.gotCharm_21 = false;
            GameManager.instance.playerData.equippedCharm_21 = false;
            GameManager.instance.playerData.charmCost_21 = 4;
            GameManager.instance.playerData.newCharm_21 = true;
            GameManager.instance.playerData.gotCharm_22 = false;
            GameManager.instance.playerData.equippedCharm_22 = false;
            GameManager.instance.playerData.charmCost_22 = 2;
            GameManager.instance.playerData.newCharm_22 = true;
            GameManager.instance.playerData.gotCharm_23 = false;
            GameManager.instance.playerData.equippedCharm_23 = false;
            GameManager.instance.playerData.brokenCharm_23 = false;
            GameManager.instance.playerData.charmCost_23 = 2;
            GameManager.instance.playerData.newCharm_23 = true;
            GameManager.instance.playerData.gotCharm_24 = false;
            GameManager.instance.playerData.equippedCharm_24 = false;
            GameManager.instance.playerData.brokenCharm_24 = false;
            GameManager.instance.playerData.charmCost_24 = 2;
            GameManager.instance.playerData.newCharm_24 = true;
            GameManager.instance.playerData.gotCharm_25 = false;
            GameManager.instance.playerData.equippedCharm_25 = false;
            GameManager.instance.playerData.brokenCharm_25 = false;
            GameManager.instance.playerData.charmCost_25 = 3;
            GameManager.instance.playerData.newCharm_25 = true;
            GameManager.instance.playerData.gotCharm_26 = false;
            GameManager.instance.playerData.equippedCharm_26 = false;
            GameManager.instance.playerData.charmCost_26 = 1;
            GameManager.instance.playerData.newCharm_26 = true;
            GameManager.instance.playerData.gotCharm_27 = false;
            GameManager.instance.playerData.equippedCharm_27 = false;
            GameManager.instance.playerData.charmCost_27 = 4;
            GameManager.instance.playerData.newCharm_27 = true;
            GameManager.instance.playerData.gotCharm_28 = false;
            GameManager.instance.playerData.equippedCharm_28 = false;
            GameManager.instance.playerData.charmCost_28 = 2;
            GameManager.instance.playerData.newCharm_28 = true;
            GameManager.instance.playerData.gotCharm_29 = false;
            GameManager.instance.playerData.equippedCharm_29 = false;
            GameManager.instance.playerData.charmCost_29 = 4;
            GameManager.instance.playerData.newCharm_29 = true;
            GameManager.instance.playerData.gotCharm_30 = false;
            GameManager.instance.playerData.equippedCharm_30 = false;
            GameManager.instance.playerData.charmCost_30 = 1;
            GameManager.instance.playerData.newCharm_30 = true;
            GameManager.instance.playerData.gotCharm_31 = false;
            GameManager.instance.playerData.equippedCharm_31 = false;
            GameManager.instance.playerData.charmCost_31 = 2;
            GameManager.instance.playerData.newCharm_31 = true;
            GameManager.instance.playerData.gotCharm_32 = false;
            GameManager.instance.playerData.equippedCharm_32 = false;
            GameManager.instance.playerData.charmCost_32 = 3;
            GameManager.instance.playerData.newCharm_32 = true;
            GameManager.instance.playerData.gotCharm_33 = false;
            GameManager.instance.playerData.equippedCharm_33 = false;
            GameManager.instance.playerData.charmCost_33 = 2;
            GameManager.instance.playerData.newCharm_33 = true;
            GameManager.instance.playerData.gotCharm_34 = false;
            GameManager.instance.playerData.equippedCharm_34 = false;
            GameManager.instance.playerData.charmCost_34 = 4;
            GameManager.instance.playerData.newCharm_34 = true;
            GameManager.instance.playerData.gotCharm_35 = false;
            GameManager.instance.playerData.equippedCharm_35 = false;
            GameManager.instance.playerData.charmCost_35 = 3;
            GameManager.instance.playerData.newCharm_35 = true;
            GameManager.instance.playerData.gotCharm_36 = false;
            GameManager.instance.playerData.equippedCharm_36 = false;
            GameManager.instance.playerData.charmCost_36 = 5;
            GameManager.instance.playerData.newCharm_36 = true;
            GameManager.instance.playerData.gotCharm_37 = false;
            GameManager.instance.playerData.equippedCharm_37 = false;
            GameManager.instance.playerData.charmCost_37 = 1;
            GameManager.instance.playerData.newCharm_37 = false;
            GameManager.instance.playerData.gotCharm_38 = false;
            GameManager.instance.playerData.equippedCharm_38 = false;
            GameManager.instance.playerData.charmCost_38 = 3;
            GameManager.instance.playerData.newCharm_38 = false;
            GameManager.instance.playerData.gotCharm_39 = false;
            GameManager.instance.playerData.equippedCharm_39 = false;
            GameManager.instance.playerData.charmCost_39 = 2;
            GameManager.instance.playerData.newCharm_39 = false;
            GameManager.instance.playerData.gotCharm_40 = false;
            GameManager.instance.playerData.equippedCharm_40 = false;
            GameManager.instance.playerData.charmCost_40 = 2;
            GameManager.instance.playerData.newCharm_40 = false;
            GameManager.instance.playerData.fragileHealth_unbreakable = false;
            GameManager.instance.playerData.fragileGreed_unbreakable = false;
            GameManager.instance.playerData.fragileStrength_unbreakable = false;
            GameManager.instance.playerData.royalCharmState = 0;
            GameManager.instance.playerData.hasJournal = false;
            GameManager.instance.playerData.lastJournalItem = 0;
            GameManager.instance.playerData.killedDummy = false;
            GameManager.instance.playerData.killsDummy = 0;
            GameManager.instance.playerData.newDataDummy = false;
            GameManager.instance.playerData.seenJournalMsg = false;
            GameManager.instance.playerData.seenHunterMsg = false;
            GameManager.instance.playerData.fillJournal = false;
            GameManager.instance.playerData.journalEntriesCompleted = 0;
            GameManager.instance.playerData.journalNotesCompleted = 0;
            GameManager.instance.playerData.journalEntriesTotal = 0;
            GameManager.instance.playerData.killedCrawler = true;
            GameManager.instance.playerData.killsCrawler = 0;
            GameManager.instance.playerData.newDataCrawler = false;
            GameManager.instance.playerData.killedBuzzer = false;
            GameManager.instance.playerData.killsBuzzer = 45;
            GameManager.instance.playerData.newDataBuzzer = false;
            GameManager.instance.playerData.killedBouncer = false;
            GameManager.instance.playerData.killsBouncer = 25;
            GameManager.instance.playerData.newDataBouncer = false;
            GameManager.instance.playerData.killedClimber = false;
            GameManager.instance.playerData.killsClimber = 30;
            GameManager.instance.playerData.newDataClimber = false;
            GameManager.instance.playerData.killedHopper = false;
            GameManager.instance.playerData.killsHopper = 25;
            GameManager.instance.playerData.newDataHopper = false;
            GameManager.instance.playerData.killedWorm = false;
            GameManager.instance.playerData.killsWorm = 10;
            GameManager.instance.playerData.newDataWorm = false;
            GameManager.instance.playerData.killedSpitter = false;
            GameManager.instance.playerData.killsSpitter = 20;
            GameManager.instance.playerData.newDataSpitter = false;
            GameManager.instance.playerData.killedHatcher = false;
            GameManager.instance.playerData.killsHatcher = 15;
            GameManager.instance.playerData.newDataHatcher = false;
            GameManager.instance.playerData.killedHatchling = false;
            GameManager.instance.playerData.killsHatchling = 30;
            GameManager.instance.playerData.newDataHatchling = false;
            GameManager.instance.playerData.killedZombieRunner = false;
            GameManager.instance.playerData.killsZombieRunner = 35;
            GameManager.instance.playerData.newDataZombieRunner = false;
            GameManager.instance.playerData.killedZombieHornhead = false;
            GameManager.instance.playerData.killsZombieHornhead = 35;
            GameManager.instance.playerData.newDataZombieHornhead = false;
            GameManager.instance.playerData.killedZombieLeaper = false;
            GameManager.instance.playerData.killsZombieLeaper = 35;
            GameManager.instance.playerData.newDataZombieLeaper = false;
            GameManager.instance.playerData.killedZombieBarger = false;
            GameManager.instance.playerData.killsZombieBarger = 35;
            GameManager.instance.playerData.newDataZombieBarger = false;
            GameManager.instance.playerData.killedZombieShield = false;
            GameManager.instance.playerData.killsZombieShield = 10;
            GameManager.instance.playerData.newDataZombieShield = false;
            GameManager.instance.playerData.killedZombieGuard = false;
            GameManager.instance.playerData.killsZombieGuard = 6;
            GameManager.instance.playerData.newDataZombieGuard = false;
            GameManager.instance.playerData.killedBigBuzzer = false;
            GameManager.instance.playerData.killsBigBuzzer = 2;
            GameManager.instance.playerData.newDataBigBuzzer = false;
            GameManager.instance.playerData.killedBigFly = false;
            GameManager.instance.playerData.killsBigFly = 3;
            GameManager.instance.playerData.newDataBigFly = false;
            GameManager.instance.playerData.killedMawlek = false;
            GameManager.instance.playerData.killsMawlek = 1;
            GameManager.instance.playerData.newDataMawlek = false;
            GameManager.instance.playerData.killedFalseKnight = false;
            GameManager.instance.playerData.killsFalseKnight = 1;
            GameManager.instance.playerData.newDataFalseKnight = false;
            GameManager.instance.playerData.killedRoller = false;
            GameManager.instance.playerData.killsRoller = 20;
            GameManager.instance.playerData.newDataRoller = false;
            GameManager.instance.playerData.killedBlocker = false;
            GameManager.instance.playerData.killsBlocker = 1;
            GameManager.instance.playerData.newDataBlocker = false;
            GameManager.instance.playerData.killedPrayerSlug = false;
            GameManager.instance.playerData.killsPrayerSlug = 2;
            GameManager.instance.playerData.newDataPrayerSlug = false;
            GameManager.instance.playerData.killedMenderBug = false;
            GameManager.instance.playerData.killsMenderBug = 1;
            GameManager.instance.playerData.newDataMenderBug = false;
            GameManager.instance.playerData.killedMossmanRunner = false;
            GameManager.instance.playerData.killsMossmanRunner = 25;
            GameManager.instance.playerData.newDataMossmanRunner = false;
            GameManager.instance.playerData.killedMossmanShaker = false;
            GameManager.instance.playerData.killsMossmanShaker = 25;
            GameManager.instance.playerData.newDataMossmanShaker = false;
            GameManager.instance.playerData.killedMosquito = false;
            GameManager.instance.playerData.killsMosquito = 25;
            GameManager.instance.playerData.newDataMosquito = false;
            GameManager.instance.playerData.killedBlobFlyer = false;
            GameManager.instance.playerData.killsBlobFlyer = 20;
            GameManager.instance.playerData.newDataBlobFlyer = false;
            GameManager.instance.playerData.killedFungifiedZombie = false;
            GameManager.instance.playerData.killsFungifiedZombie = 10;
            GameManager.instance.playerData.newDataFungifiedZombie = false;
            GameManager.instance.playerData.killedPlantShooter = false;
            GameManager.instance.playerData.killsPlantShooter = 15;
            GameManager.instance.playerData.newDataPlantShooter = false;
            GameManager.instance.playerData.killedMossCharger = false;
            GameManager.instance.playerData.killsMossCharger = 15;
            GameManager.instance.playerData.newDataMossCharger = false;
            GameManager.instance.playerData.killedMegaMossCharger = false;
            GameManager.instance.playerData.killsMegaMossCharger = 1;
            GameManager.instance.playerData.newDataMegaMossCharger = false;
            GameManager.instance.playerData.killedSnapperTrap = false;
            GameManager.instance.playerData.killsSnapperTrap = 15;
            GameManager.instance.playerData.newDataSnapperTrap = false;
            GameManager.instance.playerData.killedMossKnight = false;
            GameManager.instance.playerData.killsMossKnight = 8;
            GameManager.instance.playerData.newDataMossKnight = false;
            GameManager.instance.playerData.killedGrassHopper = false;
            GameManager.instance.playerData.killsGrassHopper = 15;
            GameManager.instance.playerData.newDataGrassHopper = false;
            GameManager.instance.playerData.killedAcidFlyer = false;
            GameManager.instance.playerData.killsAcidFlyer = 8;
            GameManager.instance.playerData.newDataAcidFlyer = false;
            GameManager.instance.playerData.killedAcidWalker = false;
            GameManager.instance.playerData.killsAcidWalker = 8;
            GameManager.instance.playerData.newDataAcidWalker = false;
            GameManager.instance.playerData.killedMossFlyer = false;
            GameManager.instance.playerData.killsMossFlyer = 25;
            GameManager.instance.playerData.newDataMossFlyer = false;
            GameManager.instance.playerData.killedMossKnightFat = false;
            GameManager.instance.playerData.killsMossKnightFat = 10;
            GameManager.instance.playerData.newDataMossKnightFat = false;
            GameManager.instance.playerData.killedMossWalker = false;
            GameManager.instance.playerData.killsMossWalker = 30;
            GameManager.instance.playerData.newDataMossWalker = false;
            GameManager.instance.playerData.killedInfectedKnight = false;
            GameManager.instance.playerData.killsInfectedKnight = 1;
            GameManager.instance.playerData.newDataInfectedKnight = false;
            GameManager.instance.playerData.killedLazyFlyer = false;
            GameManager.instance.playerData.killsLazyFlyer = 1;
            GameManager.instance.playerData.newDataLazyFlyer = false;
            GameManager.instance.playerData.killedZapBug = false;
            GameManager.instance.playerData.killsZapBug = 1;
            GameManager.instance.playerData.newDataZapBug = false;
            GameManager.instance.playerData.killedJellyfish = false;
            GameManager.instance.playerData.killsJellyfish = 12;
            GameManager.instance.playerData.newDataJellyfish = false;
            GameManager.instance.playerData.killedJellyCrawler = false;
            GameManager.instance.playerData.killsJellyCrawler = 20;
            GameManager.instance.playerData.newDataJellyCrawler = false;
            GameManager.instance.playerData.killedMegaJellyfish = false;
            GameManager.instance.playerData.killsMegaJellyfish = 1;
            GameManager.instance.playerData.newDataMegaJellyfish = false;
            GameManager.instance.playerData.killedFungoonBaby = false;
            GameManager.instance.playerData.killsFungoonBaby = 30;
            GameManager.instance.playerData.newDataFungoonBaby = false;
            GameManager.instance.playerData.killedMushroomTurret = false;
            GameManager.instance.playerData.killsMushroomTurret = 20;
            GameManager.instance.playerData.newDataMushroomTurret = false;
            GameManager.instance.playerData.killedMantis = false;
            GameManager.instance.playerData.killsMantis = 25;
            GameManager.instance.playerData.newDataMantis = false;
            GameManager.instance.playerData.killedMushroomRoller = false;
            GameManager.instance.playerData.killsMushroomRoller = 20;
            GameManager.instance.playerData.newDataMushroomRoller = false;
            GameManager.instance.playerData.killedMushroomBrawler = false;
            GameManager.instance.playerData.killsMushroomBrawler = 8;
            GameManager.instance.playerData.newDataMushroomBrawler = false;
            GameManager.instance.playerData.killedMushroomBaby = false;
            GameManager.instance.playerData.killsMushroomBaby = 20;
            GameManager.instance.playerData.newDataMushroomBaby = false;
            GameManager.instance.playerData.killedMantisFlyerChild = false;
            GameManager.instance.playerData.killsMantisFlyerChild = 25;
            GameManager.instance.playerData.newDataMantisFlyerChild = false;
            GameManager.instance.playerData.killedFungusFlyer = false;
            GameManager.instance.playerData.killsFungusFlyer = 20;
            GameManager.instance.playerData.newDataFungusFlyer = false;
            GameManager.instance.playerData.killedFungCrawler = false;
            GameManager.instance.playerData.killsFungCrawler = 15;
            GameManager.instance.playerData.newDataFungCrawler = false;
            GameManager.instance.playerData.killedMantisLord = false;
            GameManager.instance.playerData.killsMantisLord = 1;
            GameManager.instance.playerData.newDataMantisLord = false;
            GameManager.instance.playerData.killedBlackKnight = false;
            GameManager.instance.playerData.killsBlackKnight = 10;
            GameManager.instance.playerData.newDataBlackKnight = false;
            GameManager.instance.playerData.killedElectricMage = false;
            GameManager.instance.playerData.killsElectricMage = 6;
            GameManager.instance.playerData.newDataElectricMage = false;
            GameManager.instance.playerData.killedMage = false;
            GameManager.instance.playerData.killsMage = 20;
            GameManager.instance.playerData.newDataMage = false;
            GameManager.instance.playerData.killedMageKnight = false;
            GameManager.instance.playerData.killsMageKnight = 2;
            GameManager.instance.playerData.newDataMageKnight = false;
            GameManager.instance.playerData.killedRoyalDandy = false;
            GameManager.instance.playerData.killsRoyalDandy = 25;
            GameManager.instance.playerData.newDataRoyalDandy = false;
            GameManager.instance.playerData.killedRoyalCoward = false;
            GameManager.instance.playerData.killsRoyalCoward = 25;
            GameManager.instance.playerData.newDataRoyalCoward = false;
            GameManager.instance.playerData.killedRoyalPlumper = false;
            GameManager.instance.playerData.killsRoyalPlumper = 25;
            GameManager.instance.playerData.newDataRoyalPlumper = false;
            GameManager.instance.playerData.killedFlyingSentrySword = false;
            GameManager.instance.playerData.killsFlyingSentrySword = 30;
            GameManager.instance.playerData.newDataFlyingSentrySword = false;
            GameManager.instance.playerData.killedFlyingSentryJavelin = false;
            GameManager.instance.playerData.killsFlyingSentryJavelin = 25;
            GameManager.instance.playerData.newDataFlyingSentryJavelin = false;
            GameManager.instance.playerData.killedSentry = false;
            GameManager.instance.playerData.killsSentry = 25;
            GameManager.instance.playerData.newDataSentry = false;
            GameManager.instance.playerData.killedSentryFat = false;
            GameManager.instance.playerData.killsSentryFat = 20;
            GameManager.instance.playerData.newDataSentryFat = false;
            GameManager.instance.playerData.killedMageBlob = false;
            GameManager.instance.playerData.killsMageBlob = 25;
            GameManager.instance.playerData.newDataMageBlob = false;
            GameManager.instance.playerData.killedGreatShieldZombie = false;
            GameManager.instance.playerData.killsGreatShieldZombie = 10;
            GameManager.instance.playerData.newDataGreatShieldZombie = false;
            GameManager.instance.playerData.killedJarCollector = false;
            GameManager.instance.playerData.killsJarCollector = 1;
            GameManager.instance.playerData.newDataJarCollector = false;
            GameManager.instance.playerData.killedMageBalloon = false;
            GameManager.instance.playerData.killsMageBalloon = 15;
            GameManager.instance.playerData.newDataMageBalloon = false;
            GameManager.instance.playerData.killedMageLord = false;
            GameManager.instance.playerData.killsMageLord = 1;
            GameManager.instance.playerData.newDataMageLord = false;
            GameManager.instance.playerData.killedGorgeousHusk = false;
            GameManager.instance.playerData.killsGorgeousHusk = 1;
            GameManager.instance.playerData.newDataGorgeousHusk = false;
            GameManager.instance.playerData.killedFlipHopper = false;
            GameManager.instance.playerData.killsFlipHopper = 20;
            GameManager.instance.playerData.newDataFlipHopper = false;
            GameManager.instance.playerData.killedFlukeman = false;
            GameManager.instance.playerData.killsFlukeman = 20;
            GameManager.instance.playerData.newDataFlukeman = false;
            GameManager.instance.playerData.killedInflater = false;
            GameManager.instance.playerData.killsInflater = 20;
            GameManager.instance.playerData.newDataInflater = false;
            GameManager.instance.playerData.killedFlukefly = false;
            GameManager.instance.playerData.killsFlukefly = 15;
            GameManager.instance.playerData.newDataFlukefly = false;
            GameManager.instance.playerData.killedFlukeMother = false;
            GameManager.instance.playerData.killsFlukeMother = 1;
            GameManager.instance.playerData.newDataFlukeMother = false;
            GameManager.instance.playerData.killedDungDefender = false;
            GameManager.instance.playerData.killsDungDefender = 1;
            GameManager.instance.playerData.newDataDungDefender = false;
            GameManager.instance.playerData.killedCrystalCrawler = false;
            GameManager.instance.playerData.killsCrystalCrawler = 15;
            GameManager.instance.playerData.newDataCrystalCrawler = false;
            GameManager.instance.playerData.killedCrystalFlyer = false;
            GameManager.instance.playerData.killsCrystalFlyer = 20;
            GameManager.instance.playerData.newDataCrystalFlyer = false;
            GameManager.instance.playerData.killedLaserBug = false;
            GameManager.instance.playerData.killsLaserBug = 15;
            GameManager.instance.playerData.newDataLaserBug = false;
            GameManager.instance.playerData.killedBeamMiner = false;
            GameManager.instance.playerData.killsBeamMiner = 15;
            GameManager.instance.playerData.newDataBeamMiner = false;
            GameManager.instance.playerData.killedZombieMiner = false;
            GameManager.instance.playerData.killsZombieMiner = 20;
            GameManager.instance.playerData.newDataZombieMiner = false;
            GameManager.instance.playerData.killedMegaBeamMiner = false;
            GameManager.instance.playerData.killsMegaBeamMiner = 2;
            GameManager.instance.playerData.newDataMegaBeamMiner = false;
            GameManager.instance.playerData.killedMinesCrawler = false;
            GameManager.instance.playerData.killsMinesCrawler = 15;
            GameManager.instance.playerData.newDataMinesCrawler = false;
            GameManager.instance.playerData.killedAngryBuzzer = false;
            GameManager.instance.playerData.killsAngryBuzzer = 15;
            GameManager.instance.playerData.newDataAngryBuzzer = false;
            GameManager.instance.playerData.killedBurstingBouncer = false;
            GameManager.instance.playerData.killsBurstingBouncer = 15;
            GameManager.instance.playerData.newDataBurstingBouncer = false;
            GameManager.instance.playerData.killedBurstingZombie = false;
            GameManager.instance.playerData.killsBurstingZombie = 15;
            GameManager.instance.playerData.newDataBurstingZombie = false;
            GameManager.instance.playerData.killedSpittingZombie = false;
            GameManager.instance.playerData.killsSpittingZombie = 15;
            GameManager.instance.playerData.newDataSpittingZombie = false;
            GameManager.instance.playerData.killedBabyCentipede = false;
            GameManager.instance.playerData.killsBabyCentipede = 35;
            GameManager.instance.playerData.newDataBabyCentipede = false;
            GameManager.instance.playerData.killedBigCentipede = false;
            GameManager.instance.playerData.killsBigCentipede = 10;
            GameManager.instance.playerData.newDataBigCentipede = false;
            GameManager.instance.playerData.killedCentipedeHatcher = false;
            GameManager.instance.playerData.killsCentipedeHatcher = 15;
            GameManager.instance.playerData.newDataCentipedeHatcher = false;
            GameManager.instance.playerData.killedLesserMawlek = false;
            GameManager.instance.playerData.killsLesserMawlek = 10;
            GameManager.instance.playerData.newDataLesserMawlek = false;
            GameManager.instance.playerData.killedSlashSpider = false;
            GameManager.instance.playerData.killsSlashSpider = 15;
            GameManager.instance.playerData.newDataSlashSpider = false;
            GameManager.instance.playerData.killedSpiderCorpse = false;
            GameManager.instance.playerData.killsSpiderCorpse = 15;
            GameManager.instance.playerData.newDataSpiderCorpse = false;
            GameManager.instance.playerData.killedShootSpider = false;
            GameManager.instance.playerData.killsShootSpider = 20;
            GameManager.instance.playerData.newDataShootSpider = false;
            GameManager.instance.playerData.killedMiniSpider = false;
            GameManager.instance.playerData.killsMiniSpider = 25;
            GameManager.instance.playerData.newDataMiniSpider = false;
            GameManager.instance.playerData.killedSpiderFlyer = false;
            GameManager.instance.playerData.killsSpiderFlyer = 20;
            GameManager.instance.playerData.newDataSpiderFlyer = false;
            GameManager.instance.playerData.killedMimicSpider = false;
            GameManager.instance.playerData.killsMimicSpider = 1;
            GameManager.instance.playerData.newDataMimicSpider = false;
            GameManager.instance.playerData.killedBeeHatchling = false;
            GameManager.instance.playerData.killsBeeHatchling = 30;
            GameManager.instance.playerData.newDataBeeHatchling = false;
            GameManager.instance.playerData.killedBeeStinger = false;
            GameManager.instance.playerData.killsBeeStinger = 15;
            GameManager.instance.playerData.newDataBeeStinger = false;
            GameManager.instance.playerData.killedBigBee = false;
            GameManager.instance.playerData.killsBigBee = 12;
            GameManager.instance.playerData.newDataBigBee = false;
            GameManager.instance.playerData.killedHiveKnight = false;
            GameManager.instance.playerData.killsHiveKnight = 1;
            GameManager.instance.playerData.newDataHiveKnight = false;
            GameManager.instance.playerData.killedBlowFly = false;
            GameManager.instance.playerData.killsBlowFly = 20;
            GameManager.instance.playerData.newDataBlowFly = false;
            GameManager.instance.playerData.killedCeilingDropper = false;
            GameManager.instance.playerData.killsCeilingDropper = 15;
            GameManager.instance.playerData.newDataCeilingDropper = false;
            GameManager.instance.playerData.killedGiantHopper = false;
            GameManager.instance.playerData.killsGiantHopper = 10;
            GameManager.instance.playerData.newDataGiantHopper = false;
            GameManager.instance.playerData.killedGrubMimic = false;
            GameManager.instance.playerData.killsGrubMimic = 5;
            GameManager.instance.playerData.newDataGrubMimic = false;
            GameManager.instance.playerData.killedMawlekTurret = false;
            GameManager.instance.playerData.killsMawlekTurret = 10;
            GameManager.instance.playerData.newDataMawlekTurret = false;
            GameManager.instance.playerData.killedOrangeScuttler = false;
            GameManager.instance.playerData.killsOrangeScuttler = 20;
            GameManager.instance.playerData.newDataOrangeScuttler = false;
            GameManager.instance.playerData.killedHealthScuttler = false;
            GameManager.instance.playerData.killsHealthScuttler = 10;
            GameManager.instance.playerData.newDataHealthScuttler = false;
            GameManager.instance.playerData.killedPigeon = false;
            GameManager.instance.playerData.killsPigeon = 15;
            GameManager.instance.playerData.newDataPigeon = false;
            GameManager.instance.playerData.killedZombieHive = false;
            GameManager.instance.playerData.killsZombieHive = 10;
            GameManager.instance.playerData.newDataZombieHive = false;
            GameManager.instance.playerData.killedDreamGuard = false;
            GameManager.instance.playerData.killsDreamGuard = 20;
            GameManager.instance.playerData.newDataDreamGuard = false;
            GameManager.instance.playerData.killedHornet = false;
            GameManager.instance.playerData.killsHornet = 2;
            GameManager.instance.playerData.newDataHornet = false;
            GameManager.instance.playerData.killedAbyssCrawler = false;
            GameManager.instance.playerData.killsAbyssCrawler = 20;
            GameManager.instance.playerData.newDataAbyssCrawler = false;
            GameManager.instance.playerData.killedSuperSpitter = false;
            GameManager.instance.playerData.killsSuperSpitter = 25;
            GameManager.instance.playerData.newDataSuperSpitter = false;
            GameManager.instance.playerData.killedSibling = false;
            GameManager.instance.playerData.killsSibling = 25;
            GameManager.instance.playerData.newDataSibling = false;
            GameManager.instance.playerData.killedPalaceFly = false;
            GameManager.instance.playerData.killsPalaceFly = 10;
            GameManager.instance.playerData.newDataPalaceFly = false;
            GameManager.instance.playerData.killedEggSac = false;
            GameManager.instance.playerData.killsEggSac = 5;
            GameManager.instance.playerData.newDataEggSac = false;
            GameManager.instance.playerData.killedMummy = false;
            GameManager.instance.playerData.killsMummy = 10;
            GameManager.instance.playerData.newDataMummy = false;
            GameManager.instance.playerData.killedOrangeBalloon = false;
            GameManager.instance.playerData.killsOrangeBalloon = 10;
            GameManager.instance.playerData.newDataOrangeBalloon = false;
            GameManager.instance.playerData.killedAbyssTendril = false;
            GameManager.instance.playerData.killsAbyssTendril = 10;
            GameManager.instance.playerData.newDataAbyssTendril = false;
            GameManager.instance.playerData.killedHeavyMantis = false;
            GameManager.instance.playerData.killsHeavyMantis = 15;
            GameManager.instance.playerData.newDataHeavyMantis = false;
            GameManager.instance.playerData.killedTraitorLord = false;
            GameManager.instance.playerData.killsTraitorLord = 1;
            GameManager.instance.playerData.newDataTraitorLord = false;
            GameManager.instance.playerData.killedMantisHeavyFlyer = false;
            GameManager.instance.playerData.killsMantisHeavyFlyer = 16;
            GameManager.instance.playerData.newDataMantisHeavyFlyer = false;
            GameManager.instance.playerData.killedGardenZombie = false;
            GameManager.instance.playerData.killsGardenZombie = 20;
            GameManager.instance.playerData.newDataGardenZombie = false;
            GameManager.instance.playerData.killedRoyalGuard = false;
            GameManager.instance.playerData.killsRoyalGuard = 2;
            GameManager.instance.playerData.newDataRoyalGuard = false;
            GameManager.instance.playerData.killedWhiteRoyal = false;
            GameManager.instance.playerData.killsWhiteRoyal = 10;
            GameManager.instance.playerData.newDataWhiteRoyal = false;
            GameManager.instance.playerData.killedOblobble = false;
            GameManager.instance.playerData.killsOblobble = 3;
            GameManager.instance.playerData.newDataOblobble = false;
            GameManager.instance.playerData.killedZote = false;
            GameManager.instance.playerData.killsZote = 1;
            GameManager.instance.playerData.newDataZote = false;
            GameManager.instance.playerData.killedBlobble = false;
            GameManager.instance.playerData.killsBlobble = 15;
            GameManager.instance.playerData.newDataBlobble = false;
            GameManager.instance.playerData.killedColMosquito = false;
            GameManager.instance.playerData.killsColMosquito = 15;
            GameManager.instance.playerData.newDataColMosquito = false;
            GameManager.instance.playerData.killedColRoller = false;
            GameManager.instance.playerData.killsColRoller = 20;
            GameManager.instance.playerData.newDataColRoller = false;
            GameManager.instance.playerData.killedColFlyingSentry = false;
            GameManager.instance.playerData.killsColFlyingSentry = 25;
            GameManager.instance.playerData.newDataColFlyingSentry = false;
            GameManager.instance.playerData.killedColMiner = false;
            GameManager.instance.playerData.killsColMiner = 25;
            GameManager.instance.playerData.newDataColMiner = false;
            GameManager.instance.playerData.killedColShield = false;
            GameManager.instance.playerData.killsColShield = 25;
            GameManager.instance.playerData.newDataColShield = false;
            GameManager.instance.playerData.killedColWorm = false;
            GameManager.instance.playerData.killsColWorm = 20;
            GameManager.instance.playerData.newDataColWorm = false;
            GameManager.instance.playerData.killedColHopper = false;
            GameManager.instance.playerData.killsColHopper = 15;
            GameManager.instance.playerData.newDataColHopper = false;
            GameManager.instance.playerData.killedLobsterLancer = false;
            GameManager.instance.playerData.killsLobsterLancer = 1;
            GameManager.instance.playerData.newDataLobsterLancer = false;
            GameManager.instance.playerData.killedGhostAladar = false;
            GameManager.instance.playerData.killsGhostAladar = 1;
            GameManager.instance.playerData.newDataGhostAladar = false;
            GameManager.instance.playerData.killedGhostXero = false;
            GameManager.instance.playerData.killsGhostXero = 1;
            GameManager.instance.playerData.newDataGhostXero = false;
            GameManager.instance.playerData.killedGhostHu = false;
            GameManager.instance.playerData.killsGhostHu = 1;
            GameManager.instance.playerData.newDataGhostHu = false;
            GameManager.instance.playerData.killedGhostMarmu = false;
            GameManager.instance.playerData.killsGhostMarmu = 1;
            GameManager.instance.playerData.newDataGhostMarmu = false;
            GameManager.instance.playerData.killedGhostNoEyes = false;
            GameManager.instance.playerData.killsGhostNoEyes = 1;
            GameManager.instance.playerData.newDataGhostNoEyes = false;
            GameManager.instance.playerData.killedGhostMarkoth = false;
            GameManager.instance.playerData.killsGhostMarkoth = 1;
            GameManager.instance.playerData.newDataGhostMarkoth = false;
            GameManager.instance.playerData.killedGhostGalien = false;
            GameManager.instance.playerData.killsGhostGalien = 1;
            GameManager.instance.playerData.newDataGhostGalien = false;
            GameManager.instance.playerData.killedWhiteDefender = false;
            GameManager.instance.playerData.killsWhiteDefender = 1;
            GameManager.instance.playerData.newDataWhiteDefender = false;
            GameManager.instance.playerData.killedGreyPrince = false;
            GameManager.instance.playerData.killsGreyPrince = 1;
            GameManager.instance.playerData.newDataGreyPrince = false;
            GameManager.instance.playerData.killedZotelingBalloon = false;
            GameManager.instance.playerData.killsZotelingBalloon = 1;
            GameManager.instance.playerData.newDataZotelingBalloon = false;
            GameManager.instance.playerData.killedZotelingHopper = false;
            GameManager.instance.playerData.killsZotelingHopper = 1;
            GameManager.instance.playerData.newDataZotelingHopper = false;
            GameManager.instance.playerData.killedZotelingBuzzer = false;
            GameManager.instance.playerData.killsZotelingBuzzer = 1;
            GameManager.instance.playerData.newDataZotelingBuzzer = false;
            GameManager.instance.playerData.killedHollowKnight = false;
            GameManager.instance.playerData.killsHollowKnight = 1;
            GameManager.instance.playerData.newDataHollowKnight = false;
            GameManager.instance.playerData.killedFinalBoss = false;
            GameManager.instance.playerData.killsFinalBoss = 1;
            GameManager.instance.playerData.newDataFinalBoss = false;
            GameManager.instance.playerData.killedHunterMark = false;
            GameManager.instance.playerData.killsHunterMark = 1;
            GameManager.instance.playerData.newDataHunterMark = false;
            GameManager.instance.playerData.killedFlameBearerSmall = false;
            GameManager.instance.playerData.killsFlameBearerSmall = 3;
            GameManager.instance.playerData.newDataFlameBearerSmall = false;
            GameManager.instance.playerData.killedFlameBearerMed = false;
            GameManager.instance.playerData.killsFlameBearerMed = 4;
            GameManager.instance.playerData.newDataFlameBearerMed = false;
            GameManager.instance.playerData.killedFlameBearerLarge = false;
            GameManager.instance.playerData.killsFlameBearerLarge = 5;
            GameManager.instance.playerData.newDataFlameBearerLarge = false;
            GameManager.instance.playerData.killedGrimm = false;
            GameManager.instance.playerData.killsGrimm = 1;
            GameManager.instance.playerData.newDataGrimm = false;
            GameManager.instance.playerData.killedNightmareGrimm = false;
            GameManager.instance.playerData.killsNightmareGrimm = 1;
            GameManager.instance.playerData.newDataNightmareGrimm = false;
            GameManager.instance.playerData.killedBindingSeal = false;
            GameManager.instance.playerData.killsBindingSeal = 1;
            GameManager.instance.playerData.newDataBindingSeal = false;
            GameManager.instance.playerData.killedFatFluke = false;
            GameManager.instance.playerData.killsFatFluke = 8;
            GameManager.instance.playerData.newDataFatFluke = false;
            GameManager.instance.playerData.killedPaleLurker = false;
            GameManager.instance.playerData.killsPaleLurker = 1;
            GameManager.instance.playerData.newDataPaleLurker = false;
            GameManager.instance.playerData.killedNailBros = false;
            GameManager.instance.playerData.killsNailBros = 1;
            GameManager.instance.playerData.newDataNailBros = false;
            GameManager.instance.playerData.killedPaintmaster = false;
            GameManager.instance.playerData.killsPaintmaster = 1;
            GameManager.instance.playerData.newDataPaintmaster = false;
            GameManager.instance.playerData.killedNailsage = false;
            GameManager.instance.playerData.killsNailsage = 1;
            GameManager.instance.playerData.newDataNailsage = false;
            GameManager.instance.playerData.killedHollowKnightPrime = false;
            GameManager.instance.playerData.killsHollowKnightPrime = 1;
            GameManager.instance.playerData.newDataHollowKnightPrime = false;
            GameManager.instance.playerData.killedGodseekerMask = false;
            GameManager.instance.playerData.killsGodseekerMask = 1;
            GameManager.instance.playerData.newDataGodseekerMask = false;
            GameManager.instance.playerData.killedVoidIdol_1 = false;
            GameManager.instance.playerData.killsVoidIdol_1 = 1;
            GameManager.instance.playerData.newDataVoidIdol_1 = false;
            GameManager.instance.playerData.killedVoidIdol_2 = false;
            GameManager.instance.playerData.killsVoidIdol_2 = 1;
            GameManager.instance.playerData.newDataVoidIdol_2 = false;
            GameManager.instance.playerData.killedVoidIdol_3 = false;
            GameManager.instance.playerData.killsVoidIdol_3 = 1;
            GameManager.instance.playerData.newDataVoidIdol_3 = false;
            GameManager.instance.playerData.grubsCollected = 0;
            GameManager.instance.playerData.grubRewards = 0;
            GameManager.instance.playerData.finalGrubRewardCollected = false;
            GameManager.instance.playerData.fatGrubKing = false;
            GameManager.instance.playerData.falseKnightDefeated = false;
            GameManager.instance.playerData.falseKnightDreamDefeated = false;
            GameManager.instance.playerData.falseKnightOrbsCollected = false;
            GameManager.instance.playerData.mawlekDefeated = false;
            GameManager.instance.playerData.giantBuzzerDefeated = false;
            GameManager.instance.playerData.giantFlyDefeated = false;
            GameManager.instance.playerData.blocker1Defeated = false;
            GameManager.instance.playerData.blocker2Defeated = false;
            GameManager.instance.playerData.hornet1Defeated = false;
            GameManager.instance.playerData.collectorDefeated = false;
            GameManager.instance.playerData.hornetOutskirtsDefeated = false;
            GameManager.instance.playerData.mageLordDreamDefeated = false;
            GameManager.instance.playerData.mageLordOrbsCollected = false;
            GameManager.instance.playerData.infectedKnightDreamDefeated = false;
            GameManager.instance.playerData.infectedKnightOrbsCollected = false;
            GameManager.instance.playerData.whiteDefenderDefeated = false;
            GameManager.instance.playerData.whiteDefenderOrbsCollected = false;
            GameManager.instance.playerData.whiteDefenderDefeats = 0;
            GameManager.instance.playerData.greyPrinceDefeats = 0;
            GameManager.instance.playerData.greyPrinceDefeated = false;
            GameManager.instance.playerData.greyPrinceOrbsCollected = false;
            GameManager.instance.playerData.aladarSlugDefeated = 0;
            GameManager.instance.playerData.xeroDefeated = 0;
            GameManager.instance.playerData.elderHuDefeated = 0;
            GameManager.instance.playerData.mumCaterpillarDefeated = 0;
            GameManager.instance.playerData.noEyesDefeated = 0;
            GameManager.instance.playerData.markothDefeated = 0;
            GameManager.instance.playerData.galienDefeated = 0;
            GameManager.instance.playerData.XERO_encountered = false;
            GameManager.instance.playerData.ALADAR_encountered = false;
            GameManager.instance.playerData.HU_encountered = false;
            GameManager.instance.playerData.MUMCAT_encountered = false;
            GameManager.instance.playerData.NOEYES_encountered = false;
            GameManager.instance.playerData.MARKOTH_encountered = false;
            GameManager.instance.playerData.GALIEN_encountered = false;
            GameManager.instance.playerData.xeroPinned = false;
            GameManager.instance.playerData.aladarPinned = false;
            GameManager.instance.playerData.huPinned = false;
            GameManager.instance.playerData.mumCaterpillarPinned = false;
            GameManager.instance.playerData.noEyesPinned = false;
            GameManager.instance.playerData.markothPinned = false;
            GameManager.instance.playerData.galienPinned = false;
            GameManager.instance.playerData.currentInvPane = 0;
            GameManager.instance.playerData.showGeoUI = false;
            GameManager.instance.playerData.showHealthUI = false;
            GameManager.instance.playerData.promptFocus = false;
            GameManager.instance.playerData.seenFocusTablet = false;
            GameManager.instance.playerData.seenDreamNailPrompt = false;
            GameManager.instance.playerData.isFirstGame = true;
            GameManager.instance.playerData.enteredTutorialFirstTime = false;
            GameManager.instance.playerData.isInvincible = false;
            GameManager.instance.playerData.infiniteAirJump = false;
            GameManager.instance.playerData.invinciTest = false;
            GameManager.instance.playerData.hazardRespawnLocation = Vector3.zero;
            GameManager.instance.playerData.currentArea = 0;
            GameManager.instance.playerData.visitedDirtmouth = false;
            GameManager.instance.playerData.visitedCrossroads = false;
            GameManager.instance.playerData.visitedGreenpath = false;
            GameManager.instance.playerData.visitedFungus = false;
            GameManager.instance.playerData.visitedHive = false;
            GameManager.instance.playerData.visitedCrossroadsInfected = false;
            GameManager.instance.playerData.visitedRuins = false;
            GameManager.instance.playerData.visitedMines = false;
            GameManager.instance.playerData.visitedRoyalGardens = false;
            GameManager.instance.playerData.visitedFogCanyon = false;
            GameManager.instance.playerData.visitedDeepnest = false;
            GameManager.instance.playerData.visitedRestingGrounds = false;
            GameManager.instance.playerData.visitedWaterways = false;
            GameManager.instance.playerData.visitedAbyss = false;
            GameManager.instance.playerData.visitedOutskirts = false;
            GameManager.instance.playerData.visitedWhitePalace = false;
            GameManager.instance.playerData.visitedCliffs = false;
            GameManager.instance.playerData.visitedAbyssLower = false;
            GameManager.instance.playerData.visitedGodhome = false;
            GameManager.instance.playerData.visitedMines10 = false;
            GameManager.instance.playerData.scenesVisited = new List<string>();
            GameManager.instance.playerData.scenesMapped = new List<string>();
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Stag_travel");
            GameManager.instance.playerData.scenesMapped.Add("Room_Town_Stag_Station");
            GameManager.instance.playerData.scenesMapped.Add("Room_Charm_Shop");
            GameManager.instance.playerData.scenesMapped.Add("Room_Mender_House");
            GameManager.instance.playerData.scenesMapped.Add("Room_mapper");
            GameManager.instance.playerData.scenesMapped.Add("Room_nailmaster");
            GameManager.instance.playerData.scenesMapped.Add("Room_nailmaster_02");
            GameManager.instance.playerData.scenesMapped.Add("Room_nailmaster_03");
            GameManager.instance.playerData.scenesMapped.Add("Room_shop");
            GameManager.instance.playerData.scenesMapped.Add("Room_nailsmith");
            GameManager.instance.playerData.scenesMapped.Add("Room_temple");
            GameManager.instance.playerData.scenesMapped.Add("Room_ruinhouse");
            GameManager.instance.playerData.scenesMapped.Add("Room_Mansion");
            GameManager.instance.playerData.scenesMapped.Add("Room_Tram");
            GameManager.instance.playerData.scenesMapped.Add("Room_Tram_RG");
            GameManager.instance.playerData.scenesMapped.Add("Room_Bretta");
            GameManager.instance.playerData.scenesMapped.Add("Room_Fungus_Shaman");
            GameManager.instance.playerData.scenesMapped.Add("Room_Ouiji");
            GameManager.instance.playerData.scenesMapped.Add("Room_Jinn");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_01");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_02");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_03");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_Bronze");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_Silver");
            GameManager.instance.playerData.scenesMapped.Add("Room_Colosseum_Gold");
            GameManager.instance.playerData.scenesMapped.Add("Room_Slug_Shrine");
            GameManager.instance.playerData.scenesMapped.Add("Crossroads_ShamanTemple");
            GameManager.instance.playerData.scenesMapped.Add("Ruins_House_01");
            GameManager.instance.playerData.scenesMapped.Add("Ruins_House_02");
            GameManager.instance.playerData.scenesMapped.Add("Ruins_House_03");
            GameManager.instance.playerData.scenesMapped.Add("Fungus1_35");
            GameManager.instance.playerData.scenesMapped.Add("Fungus1_36");
            GameManager.instance.playerData.scenesMapped.Add("Fungus3_archive");
            GameManager.instance.playerData.scenesMapped.Add("Fungus3_archive_02");
            GameManager.instance.playerData.scenesMapped.Add("Cliffs_03");
            GameManager.instance.playerData.scenesMapped.Add("RestingGrounds_07");
            GameManager.instance.playerData.scenesMapped.Add("Deepnest_45_v02");
            GameManager.instance.playerData.scenesMapped.Add("Deepnest_Spider_Town");
            GameManager.instance.playerData.scenesMapped.Add("Room_spider_small");
            GameManager.instance.playerData.scenesMapped.Add("Room_Wyrm");
            GameManager.instance.playerData.scenesMapped.Add("Abyss_Lighthouse_room");
            GameManager.instance.playerData.scenesMapped.Add("Room_Queen");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_01");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_02");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_03_hub");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_04");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_05");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_06");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_07");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_08");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_09");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_11");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_12");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_13");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_14");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_15");
            GameManager.instance.playerData.scenesMapped.Add("White_Palace_16");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Nailcollection");
            GameManager.instance.playerData.scenesMapped.Add("Dream_01_False_Knight");
            GameManager.instance.playerData.scenesMapped.Add("Dream_03_Infected_Knight");
            GameManager.instance.playerData.scenesMapped.Add("Dream_02_Mage_Lord");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Guardian");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Guardian_Hegemol");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Guardian_Lurien");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Guardian_Monomon");
            GameManager.instance.playerData.scenesMapped.Add("Cutscene_Boss_Door");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Backer_Shrine");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Room_Believer_Shrine");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Abyss");
            GameManager.instance.playerData.scenesMapped.Add("Dream_Final_Boss");
            GameManager.instance.playerData.scenesMapped.Add("Room_Final_Boss_Atrium");
            GameManager.instance.playerData.scenesMapped.Add("Room_Final_Boss_Core");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Ending_A");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Ending_B");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Ending_C");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Ending_D");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_Ending_E");
            GameManager.instance.playerData.scenesMapped.Add("End_Credits");
            GameManager.instance.playerData.scenesMapped.Add("Cinematic_MrMushroom");
            GameManager.instance.playerData.scenesMapped.Add("End_Game_Completion");
            GameManager.instance.playerData.scenesMapped.Add("PermaDeath");
            GameManager.instance.playerData.scenesMapped.Add("PermaDeath_Unlock");
            GameManager.instance.playerData.scenesMapped.Add("Deepnest_East_17");
            GameManager.instance.playerData.scenesEncounteredBench = new List<string>();
            GameManager.instance.playerData.scenesEncounteredCocoon = new List<string>();
            GameManager.instance.playerData.scenesGrubRescued = new List<string>();
            GameManager.instance.playerData.scenesFlameCollected = new List<string>();
            GameManager.instance.playerData.scenesEncounteredDreamPlant = new List<string>();
            GameManager.instance.playerData.scenesEncounteredDreamPlantC = new List<string>();
            GameManager.instance.playerData.hasMap = false;
            GameManager.instance.playerData.mapAllRooms = false;
            GameManager.instance.playerData.atMapPrompt = false;
            GameManager.instance.playerData.mapDirtmouth = true;
            GameManager.instance.playerData.mapCrossroads = false;
            GameManager.instance.playerData.mapGreenpath = false;
            GameManager.instance.playerData.mapFogCanyon = false;
            GameManager.instance.playerData.mapRoyalGardens = false;
            GameManager.instance.playerData.mapFungalWastes = false;
            GameManager.instance.playerData.mapCity = false;
            GameManager.instance.playerData.mapWaterways = false;
            GameManager.instance.playerData.mapMines = false;
            GameManager.instance.playerData.mapDeepnest = false;
            GameManager.instance.playerData.mapCliffs = false;
            GameManager.instance.playerData.mapOutskirts = false;
            GameManager.instance.playerData.mapRestingGrounds = false;
            GameManager.instance.playerData.mapAbyss = false;
            GameManager.instance.playerData.hasPin = false;
            GameManager.instance.playerData.hasPinBench = false;
            GameManager.instance.playerData.hasPinCocoon = false;
            GameManager.instance.playerData.hasPinDreamPlant = false;
            GameManager.instance.playerData.hasPinGuardian = false;
            GameManager.instance.playerData.hasPinBlackEgg = false;
            GameManager.instance.playerData.hasPinShop = false;
            GameManager.instance.playerData.hasPinSpa = false;
            GameManager.instance.playerData.hasPinStag = false;
            GameManager.instance.playerData.hasPinTram = false;
            GameManager.instance.playerData.hasPinGhost = false;
            GameManager.instance.playerData.hasPinGrub = false;
            GameManager.instance.playerData.hasMarker = false;
            GameManager.instance.playerData.hasMarker_r = false;
            GameManager.instance.playerData.hasMarker_b = false;
            GameManager.instance.playerData.hasMarker_y = false;
            GameManager.instance.playerData.hasMarker_w = false;
            GameManager.instance.playerData.spareMarkers_r = 6;
            GameManager.instance.playerData.spareMarkers_b = 6;
            GameManager.instance.playerData.spareMarkers_y = 6;
            GameManager.instance.playerData.spareMarkers_w = 6;
            GameManager.instance.playerData.placedMarkers_r = new List<Vector3>();
            GameManager.instance.playerData.placedMarkers_b = new List<Vector3>();
            GameManager.instance.playerData.placedMarkers_y = new List<Vector3>();
            GameManager.instance.playerData.placedMarkers_w = new List<Vector3>();
            GameManager.instance.playerData.environmentType = 0;
            GameManager.instance.playerData.previousDarkness = 0;
            GameManager.instance.playerData.openedTramLower = false;
            GameManager.instance.playerData.openedTramRestingGrounds = false;
            GameManager.instance.playerData.tramLowerPosition = 0;
            GameManager.instance.playerData.tramRestingGroundsPosition = 0;
            GameManager.instance.playerData.mineLiftOpened = false;
            GameManager.instance.playerData.menderDoorOpened = false;
            GameManager.instance.playerData.vesselFragStagNest = false;
            GameManager.instance.playerData.shamanPillar = false;
            GameManager.instance.playerData.crossroadsMawlekWall = false;
            GameManager.instance.playerData.eggTempleVisited = false;
            GameManager.instance.playerData.crossroadsInfected = false;
            GameManager.instance.playerData.falseKnightFirstPlop = false;
            GameManager.instance.playerData.falseKnightWallRepaired = false;
            GameManager.instance.playerData.falseKnightWallBroken = false;
            GameManager.instance.playerData.falseKnightGhostDeparted = false;
            GameManager.instance.playerData.spaBugsEncountered = false;
            GameManager.instance.playerData.hornheadVinePlat = false;
            GameManager.instance.playerData.infectedKnightEncountered = false;
            GameManager.instance.playerData.megaMossChargerEncountered = false;
            GameManager.instance.playerData.megaMossChargerDefeated = false;
            GameManager.instance.playerData.dreamerScene1 = false;
            GameManager.instance.playerData.slugEncounterComplete = false;
            GameManager.instance.playerData.defeatedDoubleBlockers = false;
            GameManager.instance.playerData.oneWayArchive = false;
            GameManager.instance.playerData.defeatedMegaJelly = false;
            GameManager.instance.playerData.summonedMonomon = false;
            GameManager.instance.playerData.sawWoundedQuirrel = false;
            GameManager.instance.playerData.encounteredMegaJelly = false;
            GameManager.instance.playerData.defeatedMantisLords = false;
            GameManager.instance.playerData.encounteredGatekeeper = false;
            GameManager.instance.playerData.deepnestWall = false;
            GameManager.instance.playerData.queensStationNonDisplay = false;
            GameManager.instance.playerData.cityBridge1 = false;
            GameManager.instance.playerData.cityBridge2 = false;
            GameManager.instance.playerData.cityLift1 = false;
            GameManager.instance.playerData.cityLift1_isUp = false;
            GameManager.instance.playerData.liftArrival = false;
            GameManager.instance.playerData.openedMageDoor_v2 = false;
            GameManager.instance.playerData.brokenMageWindow = false;
            GameManager.instance.playerData.brokenMageWindowGlass = false;
            GameManager.instance.playerData.mageLordEncountered = false;
            GameManager.instance.playerData.mageLordEncountered_2 = false;
            GameManager.instance.playerData.mageLordDefeated = false;
            GameManager.instance.playerData.ruins1_5_tripleDoor = false;
            GameManager.instance.playerData.openedWaterwaysManhole = false;
            GameManager.instance.playerData.openedCityGate = false;
            GameManager.instance.playerData.cityGateClosed = false;
            GameManager.instance.playerData.bathHouseOpened = false;
            GameManager.instance.playerData.bathHouseWall = false;
            GameManager.instance.playerData.cityLift2 = true;
            GameManager.instance.playerData.cityLift2_isUp = false;
            GameManager.instance.playerData.city2_sewerDoor = false;
            GameManager.instance.playerData.openedLoveDoor = false;
            GameManager.instance.playerData.watcherChandelier = false;
            GameManager.instance.playerData.completedQuakeArea = false;
            GameManager.instance.playerData.kingsStationNonDisplay = false;
            GameManager.instance.playerData.tollBenchCity = false;
            GameManager.instance.playerData.waterwaysGate = false;
            GameManager.instance.playerData.defeatedDungDefender = false;
            GameManager.instance.playerData.dungDefenderEncounterReady = false;
            GameManager.instance.playerData.flukeMotherEncountered = false;
            GameManager.instance.playerData.flukeMotherDefeated = false;
            GameManager.instance.playerData.waterwaysAcidDrained = false;
            GameManager.instance.playerData.dungDefenderWallBroken = false;
            GameManager.instance.playerData.dungDefenderSleeping = false;
            GameManager.instance.playerData.defeatedMegaBeamMiner = false;
            GameManager.instance.playerData.defeatedMegaBeamMiner2 = false;
            GameManager.instance.playerData.brokeMinersWall = false;
            GameManager.instance.playerData.encounteredMimicSpider = false;
            GameManager.instance.playerData.steppedBeyondBridge = false;
            GameManager.instance.playerData.deepnestBridgeCollapsed = false;
            GameManager.instance.playerData.spiderCapture = false;
            GameManager.instance.playerData.deepnest26b_switch = false;
            GameManager.instance.playerData.openedRestingGrounds02 = false;
            GameManager.instance.playerData.restingGroundsCryptWall = false;
            GameManager.instance.playerData.dreamNailConvo = false;
            GameManager.instance.playerData.gladeGhostsKilled = 0;
            GameManager.instance.playerData.openedGardensStagStation = false;
            GameManager.instance.playerData.extendedGramophone = false;
            GameManager.instance.playerData.tollBenchQueensGardens = false;
            GameManager.instance.playerData.blizzardEnded = false;
            GameManager.instance.playerData.encounteredHornet = false;
            GameManager.instance.playerData.savedByHornet = false;
            GameManager.instance.playerData.outskirtsWall = false;
            GameManager.instance.playerData.abyssGateOpened = false;
            GameManager.instance.playerData.abyssLighthouse = false;
            GameManager.instance.playerData.blueVineDoor = false;
            GameManager.instance.playerData.gotShadeCharm = false;
            GameManager.instance.playerData.tollBenchAbyss = false;
            GameManager.instance.playerData.fountainGeo = 0;
            GameManager.instance.playerData.fountainVesselSummoned = false;
            GameManager.instance.playerData.openedBlackEggPath = false;
            GameManager.instance.playerData.enteredDreamWorld = false;
            GameManager.instance.playerData.duskKnightDefeated = false;
            GameManager.instance.playerData.whitePalaceOrb_1 = false;
            GameManager.instance.playerData.whitePalaceOrb_2 = false;
            GameManager.instance.playerData.whitePalaceOrb_3 = false;
            GameManager.instance.playerData.whitePalace05_lever = false;
            GameManager.instance.playerData.whitePalaceMidWarp = false;
            GameManager.instance.playerData.whitePalaceSecretRoomVisited = false;
            GameManager.instance.playerData.tramOpenedDeepnest = false;
            GameManager.instance.playerData.tramOpenedCrossroads = false;
            GameManager.instance.playerData.openedBlackEggDoor = false;
            GameManager.instance.playerData.unchainedHollowKnight = false;
            GameManager.instance.playerData.flamesCollected = 0;
            GameManager.instance.playerData.flamesRequired = 3;
            GameManager.instance.playerData.nightmareLanternAppeared = false;
            GameManager.instance.playerData.nightmareLanternLit = false;
            GameManager.instance.playerData.troupeInTown = false;
            GameManager.instance.playerData.divineInTown = false;
            GameManager.instance.playerData.grimmChildLevel = 1;
            GameManager.instance.playerData.elderbugConvoGrimm = false;
            GameManager.instance.playerData.slyConvoGrimm = false;
            GameManager.instance.playerData.iseldaConvoGrimm = false;
            GameManager.instance.playerData.midwifeWeaverlingConvo = false;
            GameManager.instance.playerData.metGrimm = false;
            GameManager.instance.playerData.foughtGrimm = false;
            GameManager.instance.playerData.metBrum = false;
            GameManager.instance.playerData.defeatedNightmareGrimm = false;
            GameManager.instance.playerData.grimmchildAwoken = false;
            GameManager.instance.playerData.gotBrummsFlame = false;
            GameManager.instance.playerData.brummBrokeBrazier = false;
            GameManager.instance.playerData.destroyedNightmareLantern = false;
            GameManager.instance.playerData.gotGrimmNotch = false;
            GameManager.instance.playerData.nymmInTown = false;
            GameManager.instance.playerData.nymmSpoken = false;
            GameManager.instance.playerData.elderbugNymmConvo = false;
            GameManager.instance.playerData.slyNymmConvo = false;
            GameManager.instance.playerData.iseldaNymmConvo = false;
            GameManager.instance.playerData.elderbugTroupeLeftConvo = false;
            GameManager.instance.playerData.jijiGrimmConvo = false;
            GameManager.instance.playerData.nymmMissedEggOpen = false;
            GameManager.instance.playerData.elderbugBrettaLeft = false;
            GameManager.instance.playerData.metDivine = false;
            GameManager.instance.playerData.divineFinalConvo = false;
            GameManager.instance.playerData.gaveFragileHeart = false;
            GameManager.instance.playerData.gaveFragileGreed = false;
            GameManager.instance.playerData.gaveFragileStrength = false;
            GameManager.instance.playerData.pooedFragileHeart = false;
            GameManager.instance.playerData.pooedFragileGreed = false;
            GameManager.instance.playerData.pooedFragileStrength = false;
            GameManager.instance.playerData.divineEatenConvos = 0;
            GameManager.instance.playerData.completionPercentage = 0f;
            GameManager.instance.playerData.unlockedCompletionRate = false;
            GameManager.instance.playerData.disablePause = false;
            GameManager.instance.playerData.backerCredits = false;
            GameManager.instance.playerData.mapKeyPref = 0;
            GameManager.instance.playerData.playerStory = new List<string>();
            GameManager.instance.playerData.betaEnd = false;
            GameManager.instance.playerData.bossReturnEntryGate = "";
            GameManager.instance.playerData.bossDoorStateTier1 = BossSequenceDoor.Completion.None;
            GameManager.instance.playerData.bossDoorStateTier2 = BossSequenceDoor.Completion.None;
            GameManager.instance.playerData.bossDoorStateTier3 = BossSequenceDoor.Completion.None;
            GameManager.instance.playerData.bossDoorStateTier4 = BossSequenceDoor.Completion.None;
            GameManager.instance.playerData.bossDoorStateTier5 = BossSequenceDoor.Completion.None;
            GameManager.instance.playerData.bossStatueTargetLevel = -1;
            GameManager.instance.playerData.currentBossStatueCompletionKey = "";
            GameManager.instance.playerData.statueStateGruzMother = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateVengefly = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateBroodingMawlek = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateFalseKnight = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateFailedChampion = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateHornet1 = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateHornet2 = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMegaMossCharger = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMantisLords = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateOblobbles = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateGreyPrince = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateBrokenVessel = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateLostKin = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateNosk = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateFlukemarm = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateCollector = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateWatcherKnights = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateSoulMaster = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateSoulTyrant = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateGodTamer = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateCrystalGuardian1 = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateCrystalGuardian2 = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateUumuu = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateDungDefender = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateWhiteDefender = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateHiveKnight = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateTraitorLord = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateGrimm = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateNightmareGrimm = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateHollowKnight = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateElderHu = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateGalien = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMarkoth = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMarmu = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateNoEyes = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateXero = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateGorb = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateRadiance = new BossStatue.Completion
            {
                hasBeenSeen = true
            };
            GameManager.instance.playerData.statueStateSly = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateNailmasters = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMageKnight = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStatePaintmaster = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateZote = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateNoskHornet = BossStatue.Completion.None;
            GameManager.instance.playerData.statueStateMantisLordsExtra = BossStatue.Completion.None;
            GameManager.instance.playerData.godseekerUnlocked = false;
            GameManager.instance.playerData.currentBossSequence = null;
            GameManager.instance.playerData.bossRushMode = false;
            GameManager.instance.playerData.bossDoorCageUnlocked = false;
            GameManager.instance.playerData.blueRoomDoorUnlocked = false;
            GameManager.instance.playerData.blueRoomActivated = false;
            GameManager.instance.playerData.finalBossDoorUnlocked = false;
            GameManager.instance.playerData.hasGodfinder = false;
            GameManager.instance.playerData.unlockedNewBossStatue = true;
            GameManager.instance.playerData.scaredFlukeHermitEncountered = false;
            GameManager.instance.playerData.scaredFlukeHermitReturned = false;
            GameManager.instance.playerData.enteredGGAtrium = false;
            GameManager.instance.playerData.extraFlowerAppear = false;
            GameManager.instance.playerData.givenGodseekerFlower = false;
            GameManager.instance.playerData.givenOroFlower = false;
            GameManager.instance.playerData.givenWhiteLadyFlower = false;
            GameManager.instance.playerData.givenEmilitiaFlower = false;
            GameManager.instance.playerData.unlockedBossScenes = new List<string>();
            GameManager.instance.playerData.queuedGodfinderIcon = false;
            GameManager.instance.playerData.godseekerSpokenAwake = false;
            GameManager.instance.playerData.nailsmithCorpseAppeared = false;
            GameManager.instance.playerData.godseekerWaterwaysSeenState = -1;
            GameManager.instance.playerData.godseekerWaterwaysSpoken1 = false;
            GameManager.instance.playerData.godseekerWaterwaysSpoken2 = false;
            GameManager.instance.playerData.godseekerWaterwaysSpoken3 = false;
            GameManager.instance.playerData.bossDoorEntranceTextSeen = -1;
            GameManager.instance.playerData.seenDoor4Finale = false;
            GameManager.instance.playerData.zoteStatueWallBroken = false;
            GameManager.instance.playerData.seenGGWastes = false;
            GameManager.instance.playerData.ordealAchieved = false;
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


                debugText = new GameObject("debugText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                debugText.transform.SetParent(canvas.transform);
                debugText.rectTransform.anchorMin = new Vector2(0, 0);
                debugText.rectTransform.anchorMax = new Vector2(0, 0);
                debugText.rectTransform.anchoredPosition = new Vector2(0, 0);
                debugText.alignment = TextAlignmentOptions.Center;
                debugText.fontSize = .5f;
                debugText.color = Color.white;
                debugText.text = GetDebugText();

                //set debugText position to (-12,7)
                debugText.rectTransform.position = new Vector3(-13,0, 0);
                //bring it to the front of the 2d screen
                canvas.planeDistance = 1;
                //set it to the 'UI' layer
                canvas.gameObject.layer = 5;
            }
            if (canvas != null)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    showDebug = !showDebug;
                }

                if (showDebug)
                {
                    debugText.color = Color.white;
                    debugText.text = GetDebugText();
                }
                else
                {
                    debugText.color = Color.clear;
                }

                if (godmode)
                {
                    GameManager.instance.playerData.health = 20;
                    GameManager.instance.playerData.maxHealth = 20;
                    GameManager.instance.playerData.MPCharge = 20;
                    GameManager.instance.playerData.prevHealth = 20;
                    GameManager.instance.playerData.health = 20;
                    GameManager.instance.playerData.blockerHits = 20;
                    GameManager.instance.playerData.UpdateBlueHealth();
                    GameManager.instance.playerData.isInvincible = true;
                }
                if (xdamage)
                {
                    GameManager.instance.playerData.nailDamage = 40;
                    GameManager.instance.playerData.beamDamage = 40;
                }
                if (flight)
                {
                    
                    GameManager.instance.playerData.infiniteAirJump = true;
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

                if (Input.GetKeyDown(KeyCode.Slash) || Input.GetKeyDown(KeyCode.Period))
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
                    popup_timer = 0;
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    popup_timer = 0;
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



                    else if (command_string.StartsWith("/heal"))
                    {
                        Heal();
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

                    else if (command_string.StartsWith("/setbdamage"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        if (choice < 1 || choice > 5)
                        {
                            Log("Beam damage must be between 1 and 5!");
                            return;
                        }

                        switch (choice)
                        {
                            case 1:
                                GameManager.instance.playerData.beamDamage = 4;
                                break;
                            case 2:
                                GameManager.instance.playerData.beamDamage = 8;
                                break;
                            case 3:
                                GameManager.instance.playerData.beamDamage = 16;
                                break;
                            case 4:
                                GameManager.instance.playerData.beamDamage = 20;
                                break;
                            case 5:
                                GameManager.instance.playerData.beamDamage = 24;
                                break;
                            default:
                                break;
                        }

                        Log($"Set beam damage to {GameManager.instance.playerData.beamDamage}!");
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/setndamage"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        if (choice < 1 || choice > 5)
                        {
                            Log("Nail damage must be between 1 and 5!");
                            return;
                        }

                        switch (choice)
                        {
                            case 1:
                                GameManager.instance.playerData.nailDamage = 7;
                                break;
                            case 2:
                                GameManager.instance.playerData.nailDamage = 14;
                                break;
                            case 3:
                                GameManager.instance.playerData.nailDamage = 21;
                                break;
                            case 4:
                                GameManager.instance.playerData.nailDamage = 28;
                                break;
                            case 5:
                                GameManager.instance.playerData.nailDamage = 35;
                                break;
                            default:
                                break;
                        }
                        
                        Log($"Set nail damage to {GameManager.instance.playerData.nailDamage}!");
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/reset"))
                    {
                        Reset();
                        Log("Reset/Unborked hero");
                    }

                    else if (command_string.StartsWith("/godmode"))
                    {
                        string[] strings = command_text.text.Split();
                        bool choice = bool.Parse(strings[1]);
                        godmode = choice;
                        Log($"Godmode set to {godmode}!");
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/xdamage"))
                    {
                        string[] strings = command_text.text.Split();
                        bool choice = bool.Parse(strings[1]);
                        xdamage = choice;
                        Log($"Extra Damage (instakill) set to {xdamage}");
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/jump"))
                    {
                        string[] strings = command_text.text.Split();
                        bool choice = bool.Parse(strings[1]);
                        flight = choice;
                        Log($"Infinite Jump set to {choice}!");
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

                    else if (command_string.StartsWith("/addegg"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.rancidEggs += choice;
                        Log($"Added {choice} to Rancid Eggs\nRancid Eggs: {GameManager.instance.playerData.rancidEggs}");
                    }

                    else if (command_string.StartsWith("/update"))
                    {
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/addkey"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.simpleKeys += choice;
                        Log($"Added {choice} to Simple Keys\nSimple Keys: {GameManager.instance.playerData.simpleKeys}");
                    }

                    else if (command_string.StartsWith("/addore"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.ore += choice;
                        Log($"Added {choice} to Ore\nOre: {GameManager.instance.playerData.ore}");
                    }

                    else if (command_string.StartsWith("/addorb"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.dreamOrbs += choice;
                        Log($"Added {choice} to Dream Orbs\nDream Orbs: {GameManager.instance.playerData.dreamOrbs}");
                    }

                    else if (command_string.StartsWith("/slots"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.charmSlots = choice;
                        Log($"Set {choice} as Slots amount!\n Total Slots: {GameManager.instance.playerData.charmSlots}");
                    }

                    else if (command_string.StartsWith("/allcharms"))
                    {
                        Log("Unlocking all powerups!");
                        GameManager.instance.playerData.canDash = true;
                        GameManager.instance.playerData.hasDash = true;
                        GameManager.instance.playerData.hasWalljump = true;
                        GameManager.instance.playerData.canWallJump = true;
                        GameManager.instance.playerData.hasSuperDash = true;
                        GameManager.instance.playerData.hasDreamNail = true;
                        GameManager.instance.playerData.hasShadowDash = true;
                        GameManager.instance.playerData.canShadowDash = true;
                        GameManager.instance.playerData.dreamNailUpgraded = true;
                        GameManager.instance.playerData.hasDoubleJump = true;
                        GameManager.instance.playerData.hasLantern = true;
                        GameManager.instance.playerData.hasAcidArmour = true;
                        GameManager.instance.playerData.hasTramPass = true;
                        GameManager.instance.playerData.hasSpell = true;
                        if (GameManager.instance.playerData.fireballLevel == 0)
                        {
                            GameManager.instance.playerData.fireballLevel = 1;
                        }
                        if (GameManager.instance.playerData.quakeLevel == 0)
                        {
                            GameManager.instance.playerData.quakeLevel = 1;
                        }
                        if (GameManager.instance.playerData.screamLevel == 0)
                        {
                            GameManager.instance.playerData.screamLevel = 1;
                        }
                        GameManager.instance.playerData.hasLoveKey = true;
                        GameManager.instance.playerData.hasWhiteKey = true;
                        GameManager.instance.playerData.hasKingsBrand = true;
                        GameManager.instance.playerData.hasNailArt = true;
                        GameManager.instance.playerData.hasDashSlash = true;
                        GameManager.instance.playerData.hasCyclone = true;
                        GameManager.instance.playerData.hasUpwardSlash = true;
                        GameManager.instance.playerData.hasCharm = true;
                        GameManager.instance.playerData.gotCharm_1 = true;
                        GameManager.instance.playerData.gotCharm_2 = true;
                        GameManager.instance.playerData.gotCharm_3 = true;
                        GameManager.instance.playerData.gotCharm_4 = true;
                        GameManager.instance.playerData.gotCharm_5 = true;
                        GameManager.instance.playerData.gotCharm_6 = true;
                        GameManager.instance.playerData.gotCharm_7 = true;
                        GameManager.instance.playerData.gotCharm_8 = true;
                        GameManager.instance.playerData.gotCharm_9 = true;
                        GameManager.instance.playerData.gotCharm_10 = true;
                        GameManager.instance.playerData.gotCharm_11 = true;
                        GameManager.instance.playerData.gotCharm_12 = true;
                        GameManager.instance.playerData.gotCharm_13 = true;
                        GameManager.instance.playerData.gotCharm_14 = true;
                        GameManager.instance.playerData.gotCharm_15 = true;
                        GameManager.instance.playerData.gotCharm_16 = true;
                        GameManager.instance.playerData.gotCharm_17 = true;
                        GameManager.instance.playerData.gotCharm_18 = true;
                        GameManager.instance.playerData.gotCharm_19 = true;
                        GameManager.instance.playerData.gotCharm_20 = true;
                        GameManager.instance.playerData.gotCharm_21 = true;
                        GameManager.instance.playerData.gotCharm_22 = true;
                        GameManager.instance.playerData.gotCharm_23 = true;
                        GameManager.instance.playerData.gotCharm_24 = true;
                        GameManager.instance.playerData.gotCharm_25 = true;
                        GameManager.instance.playerData.gotCharm_26 = true;
                        GameManager.instance.playerData.gotCharm_27 = true;
                        GameManager.instance.playerData.gotCharm_28 = true;
                        GameManager.instance.playerData.gotCharm_29 = true;
                        GameManager.instance.playerData.gotCharm_30 = true;
                        GameManager.instance.playerData.gotCharm_31 = true;
                        GameManager.instance.playerData.gotCharm_32 = true;
                        GameManager.instance.playerData.gotCharm_33 = true;
                        GameManager.instance.playerData.gotCharm_34 = true;
                        GameManager.instance.playerData.gotCharm_35 = true;
                        GameManager.instance.playerData.gotCharm_37 = true;
                        GameManager.instance.playerData.gotCharm_38 = true;
                        GameManager.instance.playerData.gotCharm_39 = true;
                        GameManager.instance.playerData.charmSlots = 11;
                    }

                    else if (command_string.StartsWith("/addcharm"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        AddCharm(choice);
                    }

                    else if (command_string.StartsWith("/sethealth"))
                    {
                        int amount;
                        string[] strings = command_text.text.Split();
                        amount = int.Parse(strings[1]);
                        GameManager.instance.playerData.health = amount;
                        //set max health and prevhealth too
                        GameManager.instance.playerData.maxHealth = amount;
                        GameManager.instance.playerData.prevHealth = amount;
                        Log("set health!");
                        UpdateValues();
                    }

                    else if (command_string.StartsWith("/addvessels"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.vesselFragments += choice;
                        GameManager.instance.playerData.vesselFragmentCollected = true;
                        Log($"Added {choice} vessels!\nVessel Fragments: {GameManager.instance.playerData.vesselFragments}");
                    }

                    else if (command_string.StartsWith("/nailrange"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);
                        GameManager.instance.playerData.nailRange = choice;
                        Log($"Set Nail Range to {choice}!");
                    }

                    else if (command_string.StartsWith("/allkeys"))
                    {
                        GameManager.instance.playerData.hasCityKey = true;
                        GameManager.instance.playerData.hasSlykey = true;
                        GameManager.instance.playerData.gaveSlykey = true;
                        GameManager.instance.playerData.hasWhiteKey = true;
                        GameManager.instance.playerData.usedWhiteKey = true;
                        GameManager.instance.playerData.hasMenderKey = true;
                        GameManager.instance.playerData.hasWaterwaysKey = true;
                        GameManager.instance.playerData.hasSpaKey = true;
                        GameManager.instance.playerData.hasLoveKey = true;
                        Log("Gave all keys!");
                    }

                    else if (command_string.StartsWith("/addzote"))
                    {
                        string[] strings = command_text.text.Split();
                        int choice = int.Parse(strings[1]);

                        GameManager.instance.playerData.zote += choice;
                        Log($"Added {choice} Zote kills!\nZote Kills: {GameManager.instance.playerData.zote}");
                    }

                    else if (command_string.StartsWith("/permadeath"))
                    {
                        //string[] strings = command_text.text.Split();
                        //bool choice = bool.Parse(strings[1]);
                        //permadeath = choice;
                        //Log($"Permadeath set to {choice}!");
                        
                    }

                    else if (command_string.StartsWith("/help"))
                    {
                        //holy shit i hate writing help commands like this
                        Log("<color=yellow>/addhealth</color> <color=#4d92cf><int></color> - adds the amount entered to health and maxHealth\n" +
                            "<color=yellow>/sethealth</color> <color=#4d92cf><int></color> - sets health to amount entered\n" +
                            "<color=yellow>/addmp</color> <color=#4d92cf><int></color> - adds the amount entered to MP (soul)\n" +
                            "<color=yellow>/setndamage</color> <color=#4d92cf><int 1-5></color> - sets nail damage to an upgrade tier\n" +
                            "<color=yellow>/setbdamage</color> <color=#4d92cf><int 1-5></color> - sets beam damage to an upgrade tier\n" +

                            "<color=yellow>/addmpreserve</color> <color=#4d92cf><int></color> - adds the amount entered to MP Reserve (MP cap)\n" +
                            "<color=yellow>/addmoney</color> <color=#4d92cf><int></color> - adds the amount entered to Banker\n" +
                            "<color=yellow>/addgeo</color> <color=#4d92cf><int></color> - adds the amount entered to Geo count\n" +
                            "<color=yellow>/addegg</color> <color=#4d92cf><int></color> - adds the amount entered to rancid Eggs\n" +
                            "<color=yellow>/addkey</color> <color=#4d92cf><int></color> - adds the amount entered to Simple Keys\n" +
                            "<color=yellow>/addore</color> <color=#4d92cf><int></color> - adds the amount entered to Ore\n" +
                            "<color=yellow>/addorb</color> <color=#4d92cf><int></color> - adds the amount entered to Dream Orbs\n" +
                            "<color=yellow>/addzote</color> <color=#4d92cf><int></color> - adds the amount entered to Zote count\n" +
                            "<color=yellow>/addvessels</color> <color=#4d92cf><int></color> - adds the amount entered to Vessel Fragments\n" +
                            "<color=yellow>/addcharm</color> <color=#4d92cf><int></color> - adds the charm entered to inventory\n" +
                            "<color=yellow>/allcharms</color> - unlocks all charms\n" +
                            "<color=yellow>/allkeys</color> - unlocks all keys\n" +
                            "<color=yellow>/jump</color> <color=#4d92cf><bool></color> - enables infinite jump\n" +
                            "<color=yellow>/achget</color> - unlocks all 63 steam awards (REQUIRES STEAM GAME)\n" +
                            "<color=yellow>/godmode</color> <color=#4d92cf><bool></color> - as the name suggests\n" +
                            "<color=yellow>/xdamage</color> <color=#4d92cf><bool></color> - sets nail and beam dmg to 40 every frame\n" +
                            "<color=yellow>/reset</color> - resets character to base stats, and unborks character if godmode or xdamage go bad\n" +
                            "<color=yellow>/stags</color> - unlocks all stag stations\n" +
                            "<color=yellow>/heal</color> - heals you to max health\n" +
                            "<color=yellow>F1</color> - enables or disables debug info\n" +

                            "<color=red>press return (enter) to close this window early</color>");
                        popup_timer = 2500;
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
