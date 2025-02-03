using System.Security.Cryptography.Xml;
using DBZGoatLib2.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace DBZGoatLib2.Example;

    public class ExampleTransformationBuff : Transformation {
        public override string FormType() => "transformation";

        public override string FormName() => "Super Duper Ultra Saiyan 9";
        public override Color TextColor() => Color.White;
        public override bool SaiyanSparks() => false;
        public override string HairTexturePath() => null;
        public override Gradient KiBarGradient() => new Gradient(Color.White, (1f, Color.AliceBlue));

        public override AuraData AuraData() => new AuraData("DBZGoatLib2/Assets/Aura/BaseAura", 4, BlendState.Additive, Color.Yellow);
        public override SoundData SoundData() => new SoundData("DBZGoatLib2/Assets/Sounds/BaseKiCharge", "DBZGoatLib2/Assets/Sounds/BaseKiChargeLoop", 120);

        public override bool CanTransform(Player player) => true;
        public override void OnTransform(Player player) {}
        public override void PostTransform(Player player) {}

        public override void SetStaticDefaults()
        {
            damageMulti = 2f; // The Damage multiplier. 1f is 0% bonus, 1.2f is 20% bonus, 3.2f is 220% and so on.
            speedMulti = 2f; // The speed multiplier. 1f is 0% bonus 1.5f is 50% bonus, etc.

            // The rate at which ki is drained. You lose this much ki every TICK. 
            // Terraria runs at 60 ticks per second. So you lose 60 times this value every second.
            // 4.5f = 270 ki/sec for example
            kiDrainRate = 1f; 
            kiDrainRateWithMastery = 0f;

            attackDrainMulti = 1f; // Extra Ki usage multiplier, higher value means you use more Ki on ki attacks
            baseDefenceBonus = 0; // Bonus to defense

            base.SetStaticDefaults(); // ALWAYS call this somewhere in your SetStaticDefaults()!
        }
    }