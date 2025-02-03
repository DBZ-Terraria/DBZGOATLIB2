using DBZGoatLib2.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DBZGoatLib2.Model
{
    public abstract class TransformationTree : IDBZLoadable
    {
        /// <summary>
        /// This tree's transformation nodes. Return an empty array if this tree has no transformation nodes.
        /// </summary>
        public abstract Node[] FormNodes();

        /// <summary>
        /// This tree's transformation Connectors. Return an empty array if this tree has no transformation connectors.
        /// </summary>
        public abstract Connection[] FormConnections();

        /// <summary>
        /// This tree's ability nodes. Return an empty array if this tree has no ability nodes.
        /// </summary>
        public abstract Node[] AbilityNodes();

        /// <summary>
        /// This tree's ability Connectors. Return an empty array if this tree has no ability connectors.
        /// </summary>
        public abstract Connection[] AbilityConnections();
        
        /// <summary>
        /// This tree's name. Highly recommended to be unique!
        /// </summary>
        public abstract string Name();

        /// <summary>
        /// Whether this transformation tree is a complete tree (True) or a partial tree (False) which is appended to the default DBT tree.
        /// </summary>
        public abstract bool Complete();

        /// <summary>
        /// Optional condition which, if it returns false, prevents the user from viewing this transformation tree.
        /// Defaults returns true.
        /// </summary>
        /// <param name="player">Player attempting to view the tree.</param>
        /// <returns>Whether the user can view this tree or not.</returns>
        public abstract bool Condition(Player player);

        public NodePanel Panel => new NodePanel(Name(), Complete(), FormNodes(), FormConnections(), AbilityNodes(), AbilityConnections(), Condition);

        public void Load(Mod mod)
        {
            UIHandler.RegisterPanel(Panel);
        }

        public void Unload()
        {
            UIHandler.UnregisterPanel(Panel);
        }
    }
}
