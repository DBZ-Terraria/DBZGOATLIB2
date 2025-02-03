using DBZGoatLib2.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DBZGoatLib2.Model
{
    public static class Defaults
    {
        public static Node[] TransformationNodes =
        [
            new Node(
                2, 
                3.5f, 
                "Form", 
                "ExampleTransformationBuff", 
                "Super Duper Ultra Saiyan 9", 
                "DBZGoatLib2/Assets/DBT/SSJBBuff", 
                "-| Transformation |-\nKi Drain: 60 Ki/s\nKi Damage: +100%\nKi Crit Chance: +0%\nKi Usage: +0%\nKi Cast Speed: +0%",
                "NOT UNLOCKED TEXT", 
                (Player p) => true, 
                (Player p) => true, 
                false, 
                null,
                null 
            ),
        ];

        public static Connection[] TransformationConnections =
        [
            //new Connection(0, 3, 1, false, new Gradient(Color.Yellow)),
        ];

        public static Node[] AbilityNodes =
        [

        ];

        public static Connection[] AbilityConnections =
        [

        ];

        public static NodePanel DefaultPanel = new NodePanel("Dragon Ball Terraria", true, TransformationNodes, TransformationConnections, AbilityNodes, AbilityConnections, (p) => { return true; });

        public static float GetMastery(Player player, string buffName)
        {
            TypeInfo modPlayerClass = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
            FieldInfo field = modPlayerClass.GetField(MasteryPaths[buffName]);
            dynamic modPlayer = modPlayerClass.GetMethod("ModPlayer").Invoke(null, [player]);

            return (float)field.GetValue(modPlayer);
        }

        public static Dictionary<string, string> MasteryPaths = new()
        {
            { "SSJ1Buff", "masteryLevel1" },
            { "SSJ2Buff", "masteryLevel2" },
            { "SSJ3Buff", "masteryLevel3" },
            { "SSJGBuff", "masteryLevelGod" },
            { "SSJBBuff", "masteryLevelBlue" },
            { "SSJRBuff", "masteryLevelRose" },
            { "LSSJBuff", "masteryLevelLeg" },
            { "LSSJ2Buff", "masteryLevelLeg2" },
            { "LSSJ3Buff", "masteryLevelLeg3" }
        };
        public static Dictionary<string,string> FormNames  = new()
        {
            { "SSJ1Buff", "Super Saiyan" },
            { "SuperKaiokenBuff", "Super Kaio-ken"},
            { "SSJ2Buff", "Super Saiyan 2" },
            { "SSJ3Buff", "Super Saiyan 3" },
            { "ASSJBuff", "Ascended Super Saiyan"},
            { "USSJBuff", "Ultra Super Saiyan" },
            { "SSJGBuff", "Super Saiyan God" },
            { "SSJBBuff", "Super Saiyan Blue" },
            { "SSJRBuff", "Super Saiyan Rosé" },
            { "LSSJBuff", "Legendary Super Saiyan" },
            { "LSSJ2Buff", "Legendary Super Saiyan 2" },
            { "LSSJ3Buff", "Legendary Super Saiyan 3" }
        };
        public static Dictionary<string, int> FormIDs = new()
        {
            { "None", 0 },
            { "SSJ1Buff", 1 },
            { "SSJ2Buff", 2 },
            { "SSJ3Buff", 3 },
            { "LSSJBuff", 4 },
            { "SSJGBuff", 5 },
            { "LSSJ2Buff", 6 },
            { "LSSJ3Buff", 7 },
            { "SSJBBuff", 8 },
            { "SSJRBuff", 9 },
        };
        public static Dictionary<string, string> FormPaths = new()
        {
            { "SSJ1Buff", "SSJ1" },
            { "SuperKaiokenBuff", "superKaioken" },
            { "ASSJBuff", "Assj" },
            { "USSJBuff", "Ussj" },
            { "SSJ2Buff", "SSJ2" },
            { "SSJ3Buff", "SSJ3" },
            { "LSSJBuff", "LSSJ" },
            { "SSJGBuff", "SSJG" },
            { "LSSJ2Buff", "LSSJ2" },
            { "LSSJ3Buff", "LSSJ3" },
            { "SSJBBuff", "SSJB" },
            { "SSJRBuff", "SSJR" },
        };

        public static Dictionary<string, Color> FormColors = new()
        {
            { "SSJ1Buff", Color.Yellow },
            { "SuperKaiokenBuff", Color.Red },
            { "ASSJBuff", Color.Yellow },
            { "USSJBuff", Color.Yellow },
            { "SSJ2Buff", Color.Yellow },
            { "SSJ3Buff", Color.Yellow },
            { "LSSJBuff", Color.LightGreen },
            { "SSJGBuff", Color.Red },
            { "LSSJ2Buff", Color.LightGreen },
            { "LSSJ3Buff", Color.LightGreen },
            { "SSJBBuff", new Color(15, 121, 219) },
            { "SSJRBuff", new Color(217, 39, 78) },
        };

        public static Gradient GetFormKiBar(string buffKeyName)
        {
            if (buffKeyName == "LSSJBuff" || buffKeyName == "LSSJ2Buff" || buffKeyName == "LSSJ3Buff")
            {
                Gradient g = new Gradient(new Color(104, 237, 121));
                g.AddStop(1f, new Color(4, 184, 27));
                return g;
            }

            if (buffKeyName == "SSJGBuff" || buffKeyName == "SuperKaiokenBuff")
            {
                Gradient g = new Gradient(new Color(219, 83, 83));
                g.AddStop(1f, new Color(245, 0, 0));
                return g;
            }
            if (buffKeyName == "SSJBBuff")
            {
                Gradient g = new Gradient(new Color(104, 110, 222));
                g.AddStop(1f, new Color(0, 13, 255));
                return g;
            }
            if (buffKeyName == "SSJRBuff")
            {
                Gradient g = new Gradient(new Color(240, 86, 183));
                g.AddStop(1f, new Color(255, 0, 128));
                return g;
            }
            else
            {
                Gradient g = new Gradient(new Color(245, 224, 91));
                g.AddStop(1f, new Color(240, 207, 0));
                return g;
            }
        }

        public static TraitInfo[] DBT_Traits =
        [
            new TraitInfo("Legendary", 0.05f, new Gradient(new Color(221, byte.MaxValue, 28)).AddStop(1f, new Color(70, 150, 93)), Legendary, UnTrait),
            new TraitInfo("Prodigy", 0.15f, new Gradient(new Color(0, 104, 249)).AddStop(1f, new Color(7, 28, 76)), Prodigy, UnTrait)
        ];

        private static void Legendary(Player player)
        {
            if (!ModLoader.HasMod("DBZMODPORT")) return;

            TypeInfo MyPlayer = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));

            FieldInfo playerTrait = MyPlayer.GetField("playerTrait");

            object modPlayer = MyPlayer.GetMethod("ModPlayer").Invoke(null, [player]);

            playerTrait.SetValue(modPlayer, "Legendary");
        }
        private static void UnTrait(Player player)
        {
            if (!ModLoader.HasMod("DBZMODPORT")) return;

            TypeInfo MyPlayer = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));

            FieldInfo playerTrait = MyPlayer.GetField("playerTrait");

            object modPlayer = MyPlayer.GetMethod("ModPlayer").Invoke(null, [player]);

            playerTrait.SetValue(modPlayer, "");

        }
        private static void Prodigy(Player player)
        {
            if (!ModLoader.HasMod("DBZMODPORT")) return;

            TypeInfo MyPlayer = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));

            FieldInfo playerTrait = MyPlayer.GetField("playerTrait");

            object modPlayer = MyPlayer.GetMethod("ModPlayer").Invoke(null, [player]);

            playerTrait.SetValue(modPlayer, "Prodigy");

            
        }

        public static void DoGeneticWish_Detour()
        {
            if (!ModLoader.HasMod("DBZMODPORT")) return;

            TypeInfo MyPlayer = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));

            FieldInfo playerTrait = MyPlayer.GetField("playerTrait");

            FieldInfo wishActive = MyPlayer.GetField("wishActive");

            dynamic modPlayer = MyPlayer.GetMethod("ModPlayer").Invoke(null, [Main.LocalPlayer]);

            playerTrait.SetValue(modPlayer, "");

            GPlayer player = Main.LocalPlayer.GetModPlayer<GPlayer>();

            player.RerollTraits();

            TypeInfo WishMenu = DBZGoatLib2.DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("WishMenu"));

            FieldInfo menuVisible = WishMenu.GetField("menuVisible");

            menuVisible.SetValue(null, false);

            wishActive.SetValue(modPlayer, false);
        }
    }
}
