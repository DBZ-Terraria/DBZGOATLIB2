using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using DBZGoatLib2.Model;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria;
using DBZGoatLib2.Handlers;
using Terraria.GameContent;
using DBZGoatLib2.UI.Components;
using Terraria.ModLoader.UI;
using MonoMod.Utils;
using System.Reflection;

namespace DBZGoatLib2.UI
{
    public class TransformationMenu : UIState
    {
        public static NodePanel transformationPanel;
        public static NodePanel abilityPanel;

        public static string HoveredNode;

        public static bool Visible;
        public static bool InfoVisible;

        public static bool ButtonHovered;
        public static bool Dirty;
        public static string Tooltip;
        public static bool Transitioning;

        public static Node SelectedNode;

        public static string EquippedForm = null;
        public static string EquippedTech = null;
        public static string PanelMode = "Transform";

        public static DragableUIPanel Panel;

        public static UIText InfoName;
        public static UIText InfoSeperator;
        public static UIText InfoDescription;
        public static UIText PanelTitle;
        public static UIText InfoEquip;
        public static UIText InfoClose;

        public UIElement Grid;
        public UIElement MasteryBar;
        public UIElement PrevTreeButton;
        public UIElement NextTreeButton;
        public UIElement TransformTab;
        public UIElement AbilityTab;
        public List<Connector> Connections = new();
        public List<PanelNode> Nodes = new();

        private static Node EmptyForm = new Node(
            0, // X position on grid
            0, // Y Position on grid
            "Form", // Node type (Form, Tech, Ability)
            "Null1", // Keyname (for transformations use the buffkeyname)
            "Empty", // Display name 
            "DBZGoatLib2/Assets/UI/Undiscovered", // Node image
            "Nothing yet...", // Description
            "NOT UNLOCKED TEXT", // Description while locked
            (Player p) => true, // Unlock condition
            (Player p) => true, // Discovery condition
            true, // View-only node
            null, // On Click
            null // On Equip
        );

        private static Node EmptyTech = new Node(
            0,
            0,
            "Tech", 
            "Null2", 
            "Empty", 
            "DBZGoatLib2/Assets/UI/Undiscovered", 
            "Nothing yet...", 
            "NOT UNLOCKED TEXT",
            (Player p) => true, 
            (Player p) => true, 
            true
        );

        public static Node CurrentFormNode = EmptyForm;
        public static Node CurrentTechNode = EmptyTech;

        public static PanelNode CurrentForm;
        public static PanelNode CurrentTech;

        private Color GradientA = new Color(0,0, 184);
        private Color GradientB = new Color(96,248,248);

        public TransformationMenu(NodePanel transform, NodePanel ability)
        {
            transformationPanel = transform;
            abilityPanel = ability;
        }

        public override void OnInitialize()
        {
            CreatePanel(PanelMode);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!string.IsNullOrEmpty(Tooltip))
                Main.instance.MouseText(Tooltip);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (InfoVisible)
                Panel.Color = new Color(20, 20, 20);
            else
                Panel.Color = Color.White;

            base.Draw(spriteBatch);
            DrawMasteryBar(spriteBatch);
        }

        public void SwitchTab()
        {
            if (InfoVisible)
            {
                return;
            }

            Panel.Remove();
            CreatePanel(PanelMode);
        }

        public static void RenderInfo()
        {
            PanelTitle.Remove();

            InfoName = new UIText(SelectedNode.DisplayName);
            InfoName.Height.Set(24, 0f);
            InfoName.Width.Set(516, 0f);
            InfoName.Left.Set(-5, 0f);
            InfoName.Top.Set(60, 0f);

            InfoSeperator = new UIText("------------------------------------");
            InfoSeperator.Height.Set(24, 0f);
            InfoSeperator.Width.Set(516, 0f);
            InfoSeperator.Left.Set(-20, 0f);
            InfoSeperator.Top.Set(84, 0f);

            InfoDescription = new UIText(SelectedNode.Description);
            InfoDescription.Height.Set(324, 0f);
            InfoDescription.Width.Set(516, 0f);
            InfoDescription.Left.Set(-0, 0f);
            InfoDescription.Top.Set(104, 0f);

            InfoClose = new UIText("Close");
            InfoClose.Height.Set(28, 0f);
            InfoClose.Width.Set(258, 0f);
            InfoClose.Left.Set(-20, 0f);
            InfoClose.Top.Set(365, 0f);
            InfoClose.OnLeftClick += CloseInfo;
            InfoClose.OnMouseOver += InfoHover;
            InfoClose.OnMouseOut += InfoUnhover;

            InfoEquip = new UIText("Equip");
            InfoEquip.Height.Set(28, 0f);
            InfoEquip.Width.Set(258, 0f);
            InfoEquip.Left.Set(238, 0f);
            InfoEquip.Top.Set(365, 0f);
            InfoEquip.OnLeftClick += EquipNode;
            InfoEquip.OnMouseOver += InfoHover;
            InfoEquip.OnMouseOut += InfoUnhover;

            if (SelectedNode.KeyName == CurrentFormNode.KeyName || SelectedNode.KeyName == CurrentTechNode.KeyName) {
                InfoEquip.SetText("Unequip");
            }

            Panel.Append(InfoName);
            Panel.Append(InfoSeperator);
            Panel.Append(InfoDescription);
            Panel.Append(InfoClose);

            if (SelectedNode.UnlockCondition(Main.CurrentPlayer)) {
                Panel.Append(InfoEquip);
            }
        }

        private static void InfoHover(UIMouseEvent evt, UIElement listeningElement)
        {
            ButtonHovered = true;

            var obj = listeningElement as UIText;
            obj.TextColor = Color.Yellow;
        }

        private static void InfoUnhover(UIMouseEvent evt, UIElement listeningElement)
        {
            ButtonHovered = false;

            var obj = listeningElement as UIText;
            obj.TextColor = Color.White;
        }

        public static void UnrenderInfo()
        {
            InfoVisible = false;

            InfoName.Remove();
            InfoDescription.Remove();
            InfoSeperator.Remove();
            InfoClose.Remove();

            if (SelectedNode.UnlockCondition(Main.CurrentPlayer)) {
                InfoEquip.Remove();
            }

            PanelTitle = new UIText(transformationPanel.Name);
            PanelTitle.Height.Set(22, 0f);
            PanelTitle.Width.Set(242, 0f);
            PanelTitle.Left.Set(121, 0f);
            PanelTitle.Top.Set(60, 0f);
        }

        public void CreatePanel(string mode)
        {
            Main.NewText("CreatePanel");
            
            Panel = new NodePanelComponent(ModContent.Request<Texture2D>("DBZGoatLib2/Assets/UI/NodeUI/NodeUI-"+mode));
            Panel.SetPadding(0);
            Panel.Width.Set(516, 0f);
            Panel.Height.Set(428, 0f);
            Append(Panel);

            MasteryBar = new UIElement();
            MasteryBar.Left.Set(22, 0f);
            MasteryBar.Top.Set(386, 0f);
            MasteryBar.Width.Set(436, 0f);
            MasteryBar.Height.Set(10, 0f);
            MasteryBar.OnMouseOver += MasteryBarMouseOver;
            MasteryBar.OnMouseOut += (o, e) => { Tooltip = null; };

            Panel.Append(MasteryBar);

            Grid = new();
            Grid.Left.Set(25, 0f);
            Grid.Top.Set(102, 0f);
            Grid.Width.Set(432, 0f);
            Grid.Height.Set(272, 0f);

            Panel.Append(Grid);

            PrevTreeButton = new UIElement();
            PrevTreeButton.Width.Set(96, 0f);
            PrevTreeButton.Height.Set(34, 0f);
            PrevTreeButton.Left.Set(22, 0f);
            PrevTreeButton.Top.Set(54, 0f);
            PrevTreeButton.OnLeftClick += PrevTree;
            PrevTreeButton.OnMouseOver += (o, e) => { Tooltip = "Previous Transformation Tree"; };
            PrevTreeButton.OnMouseOut += (o, e) => { Tooltip = null; };
            Panel.Append(PrevTreeButton);

            NextTreeButton = new UIElement();
            NextTreeButton.Width.Set(96, 0f);
            NextTreeButton.Height.Set(34, 0f);
            NextTreeButton.Left.Set(364, 0f);
            NextTreeButton.Top.Set(54, 0f);
            NextTreeButton.OnLeftClick += NextTree;
            NextTreeButton.OnMouseOver += (o, e) => { Tooltip = "Next Transformation Tree"; };
            NextTreeButton.OnMouseOut += (o, e) => { Tooltip = null; };
            Panel.Append(NextTreeButton);

            TransformTab = new();
            TransformTab.Width.Set(226, 0f);
            TransformTab.Height.Set(38, 0f);
            TransformTab.Left.Set(14, 0);
            TransformTab.Top.Set(8, 0);
            TransformTab.OnLeftClick += (o, e) => { Transitioning = true; PanelMode = "Transform"; SwitchTab(); };
            Panel.Append(TransformTab);

            AbilityTab = new();
            AbilityTab.Width.Set(226, 0f);
            AbilityTab.Height.Set(38, 0f);
            AbilityTab.Left.Set(241, 0);
            AbilityTab.Top.Set(8, 0);
            AbilityTab.OnLeftClick += (o, e) => { Transitioning = true; PanelMode = "Ability"; SwitchTab(); };
            Panel.Append(AbilityTab);

            CurrentFormNode.PosX = 13.375f;
            CurrentFormNode.PosY = 3.5f;
            CurrentForm = new PanelNode(ModContent.Request<Texture2D>(CurrentFormNode.IconPath), CurrentFormNode);
            CurrentForm.Width.Set(32,0f);
            CurrentForm.Height.Set(32,0f);
            CurrentForm.Left.Set(476,0f);
            CurrentForm.Top.Set(242,0f);

            Panel.Append(CurrentForm);

            CurrentTechNode.PosX = 13.375f;
            CurrentTechNode.PosY = 4.5f;
            CurrentTech = new PanelNode(ModContent.Request<Texture2D>(CurrentTechNode.IconPath), CurrentTechNode);
            CurrentTech.Width.Set(32,0f);
            CurrentTech.Height.Set(32,0f);
            CurrentTech.Left.Set(476,0f);
            CurrentTech.Top.Set(202,0f);

            Panel.Append(CurrentTech);

            if (mode == "Transform") {
                foreach (Connection connection in transformationPanel.TransformationConnections)
                {
                    Connector line = new Connector(connection);
                    Connections.Add(line);
                    Grid.Append(line);
                }

                foreach (Node node in transformationPanel.TransformationNodes)
                {
                    PanelNode Button = new PanelNode(ModContent.Request<Texture2D>(node.IconPath), node);
                    Nodes.Add(Button);
                    Grid.Append(Button);
                }

                PanelTitle = new UIText(transformationPanel.Name);
                PanelTitle.Height.Set(22, 0f);
                PanelTitle.Width.Set(242, 0f);
                PanelTitle.Left.Set(121, 0f);
                PanelTitle.Top.Set(60, 0f);

                Panel.Append(PanelTitle);
            } else if (mode == "Ability") {
                foreach (Connection connection in abilityPanel.AbilityConnections)
                {
                    Connector line = new Connector(connection);
                    Connections.Add(line);
                    Grid.Append(line);
                }

                foreach (Node node in abilityPanel.AbilityNodes)
                {
                    PanelNode Button = new PanelNode(ModContent.Request<Texture2D>(node.IconPath), node);
                    Nodes.Add(Button);
                    Grid.Append(Button);
                }

                PanelTitle = new UIText(abilityPanel.Name);
                PanelTitle.Height.Set(22, 0f);
                PanelTitle.Width.Set(242, 0f);
                PanelTitle.Left.Set(121, 0f);
                PanelTitle.Top.Set(60, 0f);

                Panel.Append(PanelTitle);
            }
            

            this.IgnoresMouseInteraction = false;
        }

        private void DrawMasteryBar(SpriteBatch spriteBatch)
        {
            if (InfoVisible){
                return;
            }

            GPlayer Player = Main.CurrentPlayer.GetModPlayer<GPlayer>();

            if (string.IsNullOrEmpty(EquippedForm) && string.IsNullOrEmpty(HoveredNode))
                return;

            string form = string.IsNullOrEmpty(HoveredNode) ? EquippedForm : HoveredNode;

            if (string.IsNullOrEmpty(form))
                return;

            float Quotient = 0f;
            if (TransformationHandler.DBTForms.Contains(form))
                Quotient = (float)Player.GetMastery(form) / 1f;
            else
                Quotient = (float)Player.GetMastery(TransformationHandler.GetTransformation(form).Value.buffID) / 1f;

            Quotient = Utils.Clamp(Quotient, 0f, 1f);

            Rectangle hitbox = MasteryBar.GetDimensions().ToRectangle();

            hitbox.X += 2;
            hitbox.Y += 2;
            hitbox.Width -= 4;
            hitbox.Height -= 4;

            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * Quotient);
            for (int i = 0; i < steps; i++)
            {
                float percent = (float)i / (right - left);
                spriteBatch.Draw((Texture2D)TextureAssets.MagicPixel, new Rectangle(left + i, hitbox.Y, 1 , hitbox.Height), Color.Lerp(GradientA, GradientB, percent));
            }
        }
        private void MasteryBarMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            if (InfoVisible){
                return;
            }

            if (string.IsNullOrEmpty(EquippedForm))
                return;
            if (Defaults.MasteryPaths.TryGetValue(EquippedForm, out string path))
            {
                Tooltip = string.Format("{0:P2} Mastery", Defaults.GetMastery(Main.CurrentPlayer, EquippedForm));
            }
            else
                Tooltip = string.Format("{0:P2} Mastery", Main.CurrentPlayer.GetModPlayer<GPlayer>().GetMastery(TransformationHandler.GetTransformation(EquippedForm).Value.buffID));

        }

        private static void CloseInfo(UIMouseEvent evt, UIElement listeningElement) {
            UnrenderInfo();
        }

        public static void EquipNode(UIMouseEvent evt, UIElement listeningElement)
        {
            if (SelectedNode.NodeType == "Form") {
                if (EquippedForm == SelectedNode.KeyName) {
                    Main.NewText("Unequip Form");
                    CurrentFormNode = EmptyForm;
                    EquippedForm = null;
                } else {
                    Main.NewText("Equip Form");
                    EquippedForm = SelectedNode.KeyName;
                    CurrentFormNode = SelectedNode;
                }
            } else if (SelectedNode.NodeType == "Tech") {
                if (EquippedTech == SelectedNode.KeyName) {
                    Main.NewText("Unequip Tech");
                    CurrentTechNode = EmptyForm;
                    EquippedForm = null;
                } else {
                    Main.NewText("Equip Tech");
                    EquippedTech = SelectedNode.KeyName;
                    CurrentTechNode = SelectedNode;
                }
            }

            UnrenderInfo();
            CurrentForm.Remove();
            CurrentTech.Remove();

            CurrentFormNode.PosX = 13.375f;
            CurrentFormNode.PosY = 3.5f;
            CurrentForm = new PanelNode(ModContent.Request<Texture2D>(CurrentFormNode.IconPath), CurrentFormNode);
            CurrentForm.Width.Set(32,0f);
            CurrentForm.Height.Set(32,0f);
            CurrentForm.Left.Set(476,0f);
            CurrentForm.Top.Set(242,0f);

            Panel.Append(CurrentForm);

            CurrentTechNode.PosX = 13.375f;
            CurrentTechNode.PosY = 4.5f;
            CurrentTech = new PanelNode(ModContent.Request<Texture2D>(CurrentTechNode.IconPath), CurrentTechNode);
            CurrentTech.Width.Set(32,0f);
            CurrentTech.Height.Set(32,0f);
            CurrentTech.Left.Set(476,0f);
            CurrentTech.Top.Set(202,0f);

            Panel.Append(CurrentTech);
        }

        private void PrevTree(UIMouseEvent evt, UIElement listeningElement)
        {
            UIHandler.PrevTree();
        }

        private void NextTree(UIMouseEvent evt, UIElement listeningElement)
        {
            UIHandler.NextTree();
        }
    }
}
