using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DBZGoatLib2.Handlers;
using DBZGoatLib2.Model;
using DBZGoatLib2.Network;
using MonoMod.Core;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DBZGoatLib2
{
    public class DBZGoatLib2 : Mod
    {
        public const BindingFlags flagsAll = BindingFlags.Public | BindingFlags.NonPublic
                                                                 | BindingFlags.Instance | BindingFlags.Static |
                                                                 BindingFlags.GetField
                                                                 | BindingFlags.SetField | BindingFlags.GetProperty |
                                                                 BindingFlags.SetProperty;

        internal List<ICoreNativeDetour> Detours = [];
        internal List<Hook> Hooks = [];

        public static readonly Lazy<Mod> Instance = new(() => ModLoader.GetMod("DBZGoatLib2"));

        public static readonly Lazy<(bool loaded, Mod mod)> DBZMOD = new(() =>
            (ModLoader.TryGetMod("DBZMODPORT", out Mod dbz), dbz));

        public static ModKeybind OpenMenu;
        public static ModKeybind Transform;

        public override void Load()
        {
            TransformationHandler.TransformKey = KeybindLoader.RegisterKeybind(this, "ActivateTransformation", "d");
            TransformationHandler.TechniqueKey = KeybindLoader.RegisterKeybind(this, "ActivateTechnique", "b");
            TransformationHandler.PowerDownKey = KeybindLoader.RegisterKeybind(this, "DeactiveForm /Tech", "z");
            TransformationHandler.EnergyChargeKey = KeybindLoader.RegisterKeybind(this, "ChargeKi", "d");
            OpenMenu = KeybindLoader.RegisterKeybind(this, "OpenTransformationMenu", "b");

            UIHandler.RegisterPanel(Defaults.DefaultPanel);

            foreach (TraitInfo trait in Defaults.DBT_Traits)
                TraitHandler.RegisterTrait(trait);

            if (ModLoader.HasMod("DBZMODPORT")) {
                TypeInfo WishMenu = DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("WishMenu"));
                AddDetour(WishMenu.AsType(), "DoGeneticWish", true, typeof(Defaults), "DoGeneticWish_Detour");
            }
        }

        public override void Unload()
        {
            foreach (Hook hook in Hooks)
            {
                hook.Dispose();
            }

            Hooks.Clear();

            foreach (ICoreNativeDetour detour in Detours)
            {
                detour.Dispose();
            }

            Detours.Clear();

            UIHandler.UnregisterPanel(Defaults.DefaultPanel);

            foreach (TraitInfo trait in Defaults.DBT_Traits)
                TraitHandler.UnregisterTrait(trait);

            foreach (Transformation form in ModContent.GetContent<Transformation>())
            {
                form.Unload();
            }
        }

        public override void PostSetupContent()
        {
            base.PostSetupContent();
            foreach (Transformation form in ModContent.GetContent<Transformation>())
                form.Load();
            
            if (ModLoader.HasMod("DBZMODPORT")) {
                TypeInfo MyPlayer = DBZMOD.Value.mod.Code.DefinedTypes.First(x => x.Name.Equals("MyPlayer"));
                AddDetour(MyPlayer.AsType(), "HandleTransformations");
                AddDetour(MyPlayer.AsType(), "HandleKiDrainMasteryContribution");
                AddDetour(MyPlayer.AsType(), "HandleDamageReceivedMastery");
            }
            
        }

        internal static void SaveConfig(DBZConfig cfg)
        {
            MethodInfo saveMethodInfo =
                typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
            if (saveMethodInfo != null)
            {
                saveMethodInfo.Invoke(null, [cfg]);
                return;
            }

            Instance.Value.Logger.Warn("In-game SaveConfig failed, code update required.");
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) =>
            NetworkHelper.HandlePacket(reader, whoAmI);

        #region Hook/Detours

        public void AddHook(Type type, string name, Type to, string toName)
        {
            Logger.Info($"type {type.FullName}   name {name}   methodType Method   to {to.FullName}   toName {toName}");

            MethodInfo method;
            method = type.GetMethod(name, flagsAll);

            Hook hook = new Hook(method, to.GetMethod(toName, flagsAll));

            hook.Apply();

            Hooks.Add(hook);
        }

        public void AddHook(Type type, string name, Type[] args, Type to, string toName)
        {
            Logger.Info($"type {type.FullName}   name {name}   methodType Method   to {to.FullName}   toName {toName}");

            MethodInfo method;
            method = type.GetMethod(name, flagsAll, args);

            Hook hook = new Hook(method, to.GetMethod(toName, flagsAll));

            hook.Apply();

            Hooks.Add(hook);
        }

        public void AddDetour(Type type, string name, bool methodType, Type to, string toName)
        {
            Logger.Info(
                $"type {type.FullName}   name {name}   methodType {(methodType ? "Method" : "Property")}   to {to.FullName}   toName {toName}");

            MethodInfo method;
            if (methodType)
            {
                method = type.GetMethod(name, flagsAll);
            }
            else
            {
                method = type.GetProperty(name, flagsAll).GetMethod;
            }

            ICoreNativeDetour detour = DetourFactory.Current.CreateNativeDetour(method.GetLdftnPointer(),
                to.GetMethod(toName, flagsAll).GetLdftnPointer());

            Detours.Add(detour);
        }

        public void AddDetour(Type type, string name, Type[] args, Type to, string toName)
        {
            Logger.Info(
                $"type {type.FullName}   name {name}   args {{{string.Join(",", args.Select(a => a.FullName))}}}   to {to.FullName}   toName {toName}");

            Detours.Add(DetourFactory.Current.CreateNativeDetour(type.GetMethod(name, args).GetLdftnPointer(),
                to.GetMethod(toName, flagsAll).GetLdftnPointer()));
        }

        public void AddDetour(Type type, string name) =>
            Detours.Add(DetourFactory.Current.CreateNativeDetour(type.GetMethod(name, flagsAll).GetLdftnPointer(),
                GetType().GetMethod("Nothing", flagsAll).GetLdftnPointer()));

        public void Nothing()
        {
        }

        #endregion
    }
}