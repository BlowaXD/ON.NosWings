﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using NosSharp.Enums;
using OpenNos.Core.Serializing;

namespace OpenNos.GameObject.Packets.CommandPackets
{
    [PacketHeader("$MapDance", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MapDancePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$MapDance";
    }
}