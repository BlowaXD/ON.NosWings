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

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.DAL;
using OpenNos.DAL.EF.Helpers;

namespace NosSharp.Parser
{
    public class Program
    {
        #region Methods

        public static void Main(string[] args)
        {
            // initialize logger
            Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Console.Title = $@"[N#] Parser";
            if (DataAccessHelper.Initialize())
            {
                RegisterMappings();
            }

            var key = new ConsoleKeyInfo();
            Logger.Log.Warn(Language.Instance.GetMessageFromKey("NEED_TREE"));
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"Root");
            Console.ResetColor();
            Console.WriteLine($@"-----_code_{ConfigurationManager.AppSettings["Language"]}_Card.txt");
            Console.WriteLine($@"-----_code_{ConfigurationManager.AppSettings["Language"]}_Item.txt");
            Console.WriteLine($@"-----_code_{ConfigurationManager.AppSettings["Language"]}_MapIDData.txt");
            Console.WriteLine($@"-----_code_{ConfigurationManager.AppSettings["Language"]}_monster.txt");
            Console.WriteLine($@"-----_code_{ConfigurationManager.AppSettings["Language"]}_Skill.txt");
            Console.WriteLine(@"-----packet.txt");
            Console.WriteLine(@"-----Card.dat");
            Console.WriteLine(@"-----Item.dat");
            Console.WriteLine(@"-----MapIDData.dat");
            Console.WriteLine(@"-----monster.dat");
            Console.WriteLine(@"-----Skill.dat");
            Console.WriteLine(@"-----quest.dat");
            Console.WriteLine(@"-----qstprize.dat");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"-----map");
            Console.ResetColor();
            Console.WriteLine(@"----------0");
            Console.WriteLine(@"----------1");
            Console.WriteLine(@"----------...");

            try
            {
                Logger.Log.Warn(Language.Instance.GetMessageFromKey("ENTER_PATH"));
                string folder = string.Empty;
                if (args.Length == 0)
                {
                    folder = Console.ReadLine();
                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_ALL")} [Y/n]");
                    key = Console.ReadKey(true);
                }
                else
                {
                    folder = args.Aggregate(folder, (current, str) => current + str + " ");
                }

                var factory = new ImportFactory(folder);
                factory.ImportPackets();

                if (key.KeyChar != 'n')
                {
                    factory.ImportMaps();
                    factory.LoadMaps();
                    factory.ImportCards();
                    factory.ImportRespawnMapType();
                    factory.ImportMapType();
                    factory.ImportMapTypeMap();
                    ImportFactory.ImportAccounts();
                    factory.ImportPortals();
                    factory.ImportScriptedInstances();
                    factory.ImportItems();
                    factory.ImportSkills();
                    factory.ImportNpcMonsters();
                    factory.ImportNpcMonsterData();
                    factory.ImportMapNpcs();
                    factory.ImportMonsters();
                    factory.ImportShops();
                    factory.ImportTeleporters();
                    factory.ImportShopItems();
                    factory.ImportShopSkills();
                    factory.ImportRecipe();
                    factory.ImportHardcodedItemRecipes();
                    factory.ImportQuests();
                }
                else
                {
                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_MAPS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMaps();
                        factory.LoadMaps();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_MAPTYPES")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMapType();
                        factory.ImportMapTypeMap();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_ACCOUNTS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        ImportFactory.ImportAccounts();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_PORTALS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportPortals();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_TIMESPACES")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportScriptedInstances();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_ITEMS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportItems();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportNpcMonsters();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERDATA")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportNpcMonsterData();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_CARDS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportCards();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_SKILLS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportSkills();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_MAPNPCS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMapNpcs();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_MONSTERS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportMonsters();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShops();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_TELEPORTERS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportTeleporters();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPITEMS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShopItems();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_SHOPSKILLS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportShopSkills();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_RECIPES")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportRecipe();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_HARDCODED_RECIPES")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportHardcodedItemRecipes();
                    }

                    Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_QUESTS")} [Y/n]");
                    key = Console.ReadKey(true);
                    if (key.KeyChar != 'n')
                    {
                        factory.ImportQuests();
                    }
                }

                Console.WriteLine($@"{Language.Instance.GetMessageFromKey("DONE")}");
                Thread.Sleep(5000);
            }
            catch (FileNotFoundException)
            {
                Logger.Log.Error(Language.Instance.GetMessageFromKey("AT_LEAST_ONE_FILE_MISSING"));
                Thread.Sleep(5000);
            }
        }

        private static void RegisterMappings()
        {
            // entities
            DaoFactory.AccountDao.RegisterMapping(typeof(AccountDTO)).InitializeMapper();
            DaoFactory.EquipmentOptionDao.RegisterMapping(typeof(EquipmentOptionDTO)).InitializeMapper();
            DaoFactory.CharacterDao.RegisterMapping(typeof(CharacterDTO)).InitializeMapper();
            DaoFactory.CharacterSkillDao.RegisterMapping(typeof(CharacterSkillDTO)).InitializeMapper();
            DaoFactory.CharacterQuestDao.RegisterMapping(typeof(CharacterQuestDTO)).InitializeMapper();
            DaoFactory.ComboDao.RegisterMapping(typeof(ComboDTO)).InitializeMapper();
            DaoFactory.DropDao.RegisterMapping(typeof(DropDTO)).InitializeMapper();
            DaoFactory.GeneralLogDao.RegisterMapping(typeof(GeneralLogDTO)).InitializeMapper();
            DaoFactory.ItemDao.RegisterMapping(typeof(ItemDTO)).InitializeMapper();
            DaoFactory.MailDao.RegisterMapping(typeof(MailDTO)).InitializeMapper();
            DaoFactory.MapDao.RegisterMapping(typeof(MapDTO)).InitializeMapper();
            DaoFactory.MapMonsterDao.RegisterMapping(typeof(MapMonsterDTO)).InitializeMapper();
            DaoFactory.MapNpcDao.RegisterMapping(typeof(MapNpcDTO)).InitializeMapper();
            DaoFactory.MapTypeDao.RegisterMapping(typeof(MapTypeDTO)).InitializeMapper();
            DaoFactory.MapTypeMapDao.RegisterMapping(typeof(MapTypeMapDTO)).InitializeMapper();
            DaoFactory.NpcMonsterDao.RegisterMapping(typeof(NpcMonsterDTO)).InitializeMapper();
            DaoFactory.NpcMonsterSkillDao.RegisterMapping(typeof(NpcMonsterSkillDTO)).InitializeMapper();
            DaoFactory.PenaltyLogDao.RegisterMapping(typeof(PenaltyLogDTO)).InitializeMapper();
            DaoFactory.PortalDao.RegisterMapping(typeof(PortalDTO)).InitializeMapper();
            DaoFactory.QuestDao.RegisterMapping(typeof(QuestDTO)).InitializeMapper();
            DaoFactory.QuestRewardDao.RegisterMapping(typeof(QuestRewardDTO)).InitializeMapper();
            DaoFactory.QuestObjectiveDao.RegisterMapping(typeof(QuestObjectiveDTO)).InitializeMapper();
            DaoFactory.QuicklistEntryDao.RegisterMapping(typeof(QuicklistEntryDTO)).InitializeMapper();
            DaoFactory.RecipeDao.RegisterMapping(typeof(RecipeDTO)).InitializeMapper();
            DaoFactory.RecipeItemDao.RegisterMapping(typeof(RecipeItemDTO)).InitializeMapper();
            DaoFactory.RespawnDao.RegisterMapping(typeof(RespawnDTO)).InitializeMapper();
            DaoFactory.BCardDao.RegisterMapping(typeof(BCardDTO)).InitializeMapper();
            DaoFactory.RespawnMapTypeDao.RegisterMapping(typeof(RespawnMapTypeDTO)).InitializeMapper();
            DaoFactory.ShopDao.RegisterMapping(typeof(ShopDTO)).InitializeMapper();
            DaoFactory.ShopItemDao.RegisterMapping(typeof(ShopItemDTO)).InitializeMapper();
            DaoFactory.ShopSkillDao.RegisterMapping(typeof(ShopSkillDTO)).InitializeMapper();
            DaoFactory.CardDao.RegisterMapping(typeof(CardDTO)).InitializeMapper();
            DaoFactory.MateDao.RegisterMapping(typeof(MateDTO)).InitializeMapper();
            DaoFactory.SkillDao.RegisterMapping(typeof(SkillDTO)).InitializeMapper();
            DaoFactory.TeleporterDao.RegisterMapping(typeof(TeleporterDTO)).InitializeMapper();
            DaoFactory.ScriptedInstanceDao.RegisterMapping(typeof(ScriptedInstanceDTO)).InitializeMapper();
        }

        #endregion
    }
}