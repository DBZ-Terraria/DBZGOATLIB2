using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using DBZGoatLib2.Model;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using DBZGoatLib2.Handlers;
using Terraria.ID;

namespace DBZGoatLib2.UI.Components
{
    public class PanelNode : UIImageButton
    {
        public Node Node;
        private Asset<Texture2D> TextureCache;
        private Asset<Texture2D> Hidden;

        private UIImage Selector;
        private UIImage Lock;
        public PanelNode(Asset<Texture2D> texture, Node node) : base(texture)
        {
            Node = node;
            TextureCache = texture;
            Hidden = ModContent.Request<Texture2D>("DBZGoatLib2/Assets/UI/Undiscovered");

            Width.Set(32, 0f);
            Height.Set(32, 0f);

            MaxHeight.Set(32, 0f);
            MaxWidth.Set(32, 0f);

            Left.Set(40 * node.PosX, 0f);
            Top.Set(40 * node.PosY, 0f);

            Selector = new UIImage(ModContent.Request<Texture2D>("DBZGoatLib2/Assets/UI/Selected"));

            Selector.Width.Set(32, 0f);
            Selector.Height.Set(32, 0f);
            Selector.Left.Set(-2, 0f);
            Selector.Top.Set(-2, 0f);
            Selector.IgnoresMouseInteraction = true;

            Lock = new UIImage(ModContent.Request<Texture2D>("DBZGoatLib2/Assets/UI/Locked"));

            Lock.Width.Set(32, 0f);
            Lock.Height.Set(32, 0f);
            Lock.IgnoresMouseInteraction = true;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (TransformationMenu.InfoVisible)
            {
                return;
            }

            SoundHandler.PlayVanillaSound(SoundID.MenuTick, Main.CurrentPlayer.position);

            if (!Node.DiscoverCondition(Main.CurrentPlayer))
            {
                Main.NewText("Report bug please");
                return;
            }

            if (Node.ViewOnly)
            {
                return;
            }

            //TransformationMenu.ActiveForm = Node.KeyName;

            Node.OnClick?.Invoke(Main.CurrentPlayer);

            TransformationMenu.SelectedNode = Node;
            TransformationMenu.InfoVisible = true;

            TransformationMenu.RenderInfo();
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            TransformationMenu.Tooltip = null;
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            if (TransformationMenu.InfoVisible)
            {
                return;
            }

            if (!Node.DiscoverCondition(Main.CurrentPlayer))
                return;
            else
                TransformationMenu.Tooltip = Node.DisplayName;

            TransformationMenu.HoveredNode = Node.KeyName;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var color = Color.White;
            if (TransformationMenu.InfoVisible)
                color = new Color(20, 20, 20);

            if (Node.DiscoverCondition(Main.CurrentPlayer))
                spriteBatch.Draw(TextureCache.Value, GetDimensions().ToRectangle(), color);
        }
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (TransformationMenu.EquippedForm == Node.KeyName)
            {
                //Append(Selector);
            }
            else
            {
                if (Children.Contains(Selector))
                    RemoveChild(Selector);
            }

            if (!Node.UnlockCondition(Main.CurrentPlayer) & Node.DiscoverCondition(Main.CurrentPlayer)) 
            {
                if (TransformationMenu.InfoVisible)
				    Lock.Color = new Color(20,20,20);
                else
                    Lock.Color = Color.White;

                Append(Lock);
            }
            else
            {
                if (Children.Contains(Lock))
                    RemoveChild(Lock);
            }
            
            base.DrawChildren(spriteBatch);
        }
    }
}
