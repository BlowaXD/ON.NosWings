﻿////<auto-generated <- Codemaid exclusion for now (PacketIndex Order is important for maintenance)

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;

namespace OpenNos.GameObject.CommandPackets
{
    [PacketHeader("$VipTP", Authority = AuthorityType.Donator)]
    public class VipCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Arg { get; set; }

        public IEnumerable<string> Help()
        {
            return new List<string>();
        }

        public override string ToString()
        {
            return "$VipTp -help";
        }

        #endregion
    }
}