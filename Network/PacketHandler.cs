using System.IO;

using Terraria.ModLoader;

namespace DBZGoatLib2.Network {

    internal abstract class PacketHandler {
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected PacketHandler(byte handlerType) => HandlerType = handlerType;

        protected ModPacket GetPacket(byte packetType) {
            ModPacket packet;
            packet = DBZGoatLib2.Instance.Value.GetPacket(256);
            packet.Write(HandlerType);
            packet.Write(packetType);
            return packet;
        }
    }
}