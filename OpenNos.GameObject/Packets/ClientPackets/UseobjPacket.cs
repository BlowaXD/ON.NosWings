﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)
using OpenNos.Core;

namespace OpenNos.GameObject
{
    [PacketHeader("useobj")]
    public class UseobjPacket : PacketDefinition
    {
        #region Properties
        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        #endregion
    }
}