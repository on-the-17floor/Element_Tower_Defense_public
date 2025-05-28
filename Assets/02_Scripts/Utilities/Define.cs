
using System.Collections.Generic;
using UnityEngine;

static public class Define
{
    public static readonly string LANGUAGE_KEY = "LanguageIndex";
    public static readonly string ENEMY = "Enemy";

    public static readonly int ELEMENT_COUNT = 5;
    public static readonly int EFFECT_KEY_OFFESET = 500;
    public static readonly int DEFAULT_SELLCOST = 50;

    public static readonly float[,] ELEMENTAL_DAMAGE_MATRIX =
    {
        // [Tower, Enemy]
        {  1,    0.7f, 1,    1.3f, 1,    1 },  
        {  1.3f, 1,    0.7f, 1,    1,    1 }, 
        {  1,    1.3f, 1,    0.7f, 1,    1 },  
        {  0.7f, 1,    1.3f, 1,    1,    1 },    
        {  1,    1,    1,    1,    1,    1 },   
        {  1,    1,    1,    1,    1,    1 },   
    };

    public static readonly float UPGRADE_RATE = 1.025f;

    public static readonly int DEFAULT_SUMMON_COST = 100;

    public static readonly Vector3 BULLET_OFFSET = new Vector3(0, 1.5f, 0);

    public static readonly Dictionary<SceneState, string> SceneNames = new Dictionary<SceneState, string>()
    {
        {SceneState.Main, "01_Main"},
        {SceneState.Lobby, "02_Lobby" } 
    };

    public static readonly string MESSAGE_KEY_STAGE = "Message_Stage";
    public static readonly string MESSAGE_KEY_MISSION = "Message_Mission";
    public static readonly string MESSAGE_KEY_TICKET = "Message_Ticket";
    public static readonly string MESSAGE_KEY_GETREWARD = "Message_GetReward";
    public static readonly string MESSAGE_KEY_HIDDENMISSION = "Message_HiddenMission ";
 
}

