using UnityEngine;

namespace LUP.Define
{
    public enum AssetBundleKind
    {
        Video =0,
        Audio = 1,
        Image = 2,
        VFX = 3,
        GUI = 4,
        Model = 5,
        Shader = 6,
        Data = 7,
        Manifest = 8,
        __MAX,
    }



    public enum StageKind
    {
        Unknown = 0,
        Debug = 1,
        Main = 2,
        Intro = 3,
        RL = 4,
        ST = 5,
        ES = 6,
        PCR = 7,
        DSG = 8,
        Tutorial=9,
    }
    public enum RuntimeDataType
    {
        RoguelikeRuntime,
        ShootingRuntime,
        DeckStrategyRuntime,
        ExtractionShooterRuntime,
        ProductionRuntime,
        Versions,
        QuestList,
        DSGEnemyRuntime,
    }

    public static class RuntimeDataTypes
    {
        public static string ToFilename(this RuntimeDataType type)
        {
            return type switch
            {
                RuntimeDataType.RoguelikeRuntime => "roguelike_runtime.json",
                RuntimeDataType.ShootingRuntime => "shooting_runtime.json",
                RuntimeDataType.DeckStrategyRuntime => "deckstrategy_runtime.json",
                RuntimeDataType.ExtractionShooterRuntime => "extractionshooter_runtime.json",
                RuntimeDataType.ProductionRuntime => "production_runtime.json",
                RuntimeDataType.Versions => "Versions.json",
                RuntimeDataType.QuestList => "CurrentQuestListData.json",
                RuntimeDataType.DSGEnemyRuntime => "dsg_enemy_runtime.json"
            };
        }

    }

    public enum ItemType
    {
        None = 0,
        Weapon = 1,
        Armor = 2,
        Consumable = 3,
        Material = 4,
        Quest = 5,
        Currency = 6,
    }


    public enum DataSourceType
    {
        CSV
    }

}

