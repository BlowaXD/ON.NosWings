﻿/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class BoxItem : Item
    {
        #region Instantiation

        public BoxItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0, string[] packetsplit = null)
        {
            if (session == null)
            {
                return;
            }
            switch (Effect)
            {
                case 0:
                case 999:
                    if (option == 0)
                    {
                        if (packetsplit != null && packetsplit.Length == 9)
                        {
                            BoxInstance box = session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot, InventoryType.Equipment);
                            if (box != null)
                            {
                                if (box.Item.ItemSubType == 3)
                                {
                                    session.SendPacket($"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_OPEN_BOX")}");
                                }
                                else if (box.HoldingVNum == 0)
                                {
                                    session.SendPacket($"qna #guri^300^8023^{inv.Slot}^{packetsplit[3]} {Language.Instance.GetMessageFromKey("ASK_STORE_PET")}");
                                }
                                else
                                {
                                    session.SendPacket($"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                                }
                            }
                        }
                    }
                    else
                    {
                        //u_i 2 2000000 0 21 0 0
                        BoxInstance box = session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.Item.ItemSubType == 3)
                            {
                                List<RollGeneratedItemDTO> roll = box.Item.RollGeneratedItems.Where(s => s.MinimumOriginalItemRare <= box.Rare && s.MaximumOriginalItemRare >= box.Rare && s.OriginalItemDesign == box.Design).ToList();
                                int probabilities = roll.Sum(s => s.Probability);
                                int rnd = ServerManager.Instance.RandomNumber(0, probabilities);
                                int currentrnd = 0;
                                List<ItemInstance> newInv = null;
                                foreach (RollGeneratedItemDTO rollitem in roll)
                                {
                                    if (newInv != null)
                                    {
                                        continue;
                                    }
                                    currentrnd += rollitem.Probability;
                                    if (currentrnd < rnd)
                                    {
                                        continue;
                                    }
                                    switch (rollitem.ItemGeneratedVNum)
                                    {
                                        case 565:
                                        case 566:
                                        case 567:
                                        case 577:
                                        case 578:
                                        case 579:
                                        case 568:
                                        case 569:
                                        case 570:
                                        case 580:
                                        case 551:
                                        case 582:
                                        case 571:
                                        case 572:
                                        case 573:
                                        case 583:
                                        case 584:
                                        case 585:
                                        case 574:
                                        case 575:
                                        case 576:/* ARMOR */
                                        case 586:
                                        case 587:
                                        case 588:
                                        case 589:
                                        case 590:
                                        case 591:
                                        case 592:
                                        case 593:
                                        case 594:
                                        case 595:
                                        case 596:
                                        case 597:
                                        case 598: /* HALF SHELL */
                                        case 599:
                                        case 600:
                                        case 601:
                                        case 602:
                                        case 603:
                                        case 604:
                                        case 605:
                                        case 606:
                                        case 607:
                                        case 608: /* HALF SHELL */
                                            rollitem.ItemGeneratedUpgrade = (byte)ServerManager.Instance.RandomNumber();
                                            if (rollitem.ItemGeneratedUpgrade > 95)
                                            {
                                                rollitem.ItemGeneratedUpgrade =
                                                    (byte)ServerManager.Instance.RandomNumber(76, 90);
                                            }
                                            rollitem.ItemGeneratedUpgrade =
                                                (byte)ServerManager.Instance.RandomNumber(50, 75);

                                            break;
                                    }
                                    newInv = session.Character.Inventory.AddNewToInventory(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, rare: box.Rare, upgrade: rollitem.ItemGeneratedUpgrade);
                                    if (!newInv.Any())
                                    {
                                        continue;
                                    }
                                    short slot = inv.Slot;
                                    if (slot == -1)
                                    {
                                        continue;
                                    }
                                    session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv.FirstOrDefault()?.Item.Name ?? ""} x {rollitem.ItemGeneratedAmount})", 12));
                                    session.SendPacket($"rdi {rollitem.ItemGeneratedVNum} {rollitem.ItemGeneratedAmount}");
                                    newInv.ForEach(s => session.SendPacket(s?.GenerateInventoryAdd()));
                                    session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                }
                            }
                            else if (box.HoldingVNum == 0)
                            {
                                if (packetsplit != null && packetsplit.Length == 1)
                                {
                                    if (int.TryParse(packetsplit[0], out int petId))
                                    {
                                        Mate mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == petId);
                                        if (mate != null)
                                        {
                                            box.HoldingVNum = mate.NpcMonsterVNum;
                                            box.SpLevel = mate.Level;
                                            box.SpDamage = mate.Attack;
                                            box.SpDefence = mate.Defence;
                                            session.Character.Mates.Remove(mate);
                                            session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("PET_STORED")));
                                            session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                                            session.SendPackets(session.Character.GenerateScP());
                                            session.SendPackets(session.Character.GenerateScN());
                                            session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NpcMonster heldMonster = ServerManager.Instance.GetNpc(box.HoldingVNum);
                                if (heldMonster != null)
                                {
                                    Mate mate = new Mate(session.Character, heldMonster, 1, MateType.Pet)
                                    {
                                        Attack = box.SpDamage,
                                        Defence = box.SpDefence
                                    };
                                    if (session.Character.AddPet(mate))
                                    {
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                        session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 1:
                    if (option == 0)
                    {
                        session.SendPacket($"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                    }
                    else
                    {
                        NpcMonster heldMonster = ServerManager.Instance.GetNpc((short)EffectValue);
                        if (session.CurrentMapInstance == session.Character.Miniland && heldMonster != null)
                        {
                            Mate mate = new Mate(session.Character, heldMonster, LevelMinimum, ItemSubType == 1 ? MateType.Partner : MateType.Pet);
                            if (session.Character.AddPet(mate))
                            {
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                            }
                        }
                        else
                        {
                            //TODO ADD MINILAND SENDPACKET
                        }
                    }

                    break;

                case 69:
                    if (EffectValue == 1 || EffectValue == 2)
                    {
                        BoxInstance box = session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"wopen 44 {inv.Slot}");
                            }
                            else
                            {
                                List<ItemInstance> newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Any())
                                {
                                    ItemInstance itemInstance = newInv.First();
                                    SpecialistInstance specialist = session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(itemInstance.Slot, itemInstance.Type);
                                    if (specialist != null)
                                    {
                                        specialist.SlDamage = box.SlDamage;
                                        specialist.SlDefence = box.SlDefence;
                                        specialist.SlElement = box.SlElement;
                                        specialist.SlHP = box.SlHP;
                                        specialist.SpDamage = box.SpDamage;
                                        specialist.SpDark = box.SpDark;
                                        specialist.SpDefence = box.SpDefence;
                                        specialist.SpElement = box.SpElement;
                                        specialist.SpFire = box.SpFire;
                                        specialist.SpHP = box.SpHP;
                                        specialist.SpLevel = box.SpLevel;
                                        specialist.SpLight = box.SpLight;
                                        specialist.SpStoneUpgrade = box.SpStoneUpgrade;
                                        specialist.SpWater = box.SpWater;
                                        specialist.Upgrade = box.Upgrade;
                                        specialist.XP = box.XP;
                                    }
                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        if (specialist != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {specialist.Item.Name} + {specialist.Upgrade}", 12));
                                            newInv.ForEach(s => session.SendPacket(specialist.GenerateInventoryAdd()));
                                        }
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }
                    if (EffectValue == 3)
                    {
                        BoxInstance box = session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 26 0 {inv.Slot}");
                            }
                            else
                            {
                                List<ItemInstance> newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Any())
                                {
                                    ItemInstance itemInstance = newInv.First();
                                    WearableInstance fairy = session.Character.Inventory.LoadBySlotAndType<WearableInstance>(itemInstance.Slot, itemInstance.Type);
                                    if (fairy != null)
                                    {
                                        fairy.ElementRate = box.ElementRate;
                                    }
                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        if (fairy != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {fairy.Item.Name} ({fairy.ElementRate}%)", 12));
                                            newInv.ForEach(s => session.SendPacket(fairy.GenerateInventoryAdd()));
                                        }
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }
                    if (EffectValue == 4)
                    {
                        BoxInstance box = session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot, InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 24 0 {inv.Slot}");
                            }
                            else
                            {
                                List<ItemInstance> newInv = session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Any())
                                {
                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        session.SendPacket(session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv.First().Item.Name} x 1)", 12));
                                        newInv.ForEach(s => session.SendPacket(s.GenerateInventoryAdd()));
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }
                    break;

                default:
                    Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}