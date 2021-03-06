/*
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
using System.Globalization;
using log4net;
using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Data;
using OpenNos.DAL;
using OpenNos.DAL.EF.Helpers;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Packets.ClientPackets;
using OpenNos.Handler;
using OpenNos.Master.Library.Client;

namespace NosSharp.Login
{
    public class Program
    {
        #region Methods

        public static void Main()
        {
            checked
            {
                try
                {
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    // initialize Logger
                    Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

                    int port = Convert.ToInt32(ConfigurationManager.AppSettings["LoginPort"]);
                    Console.Title = $"N# - Login Server - {port}";

                    // initialize api
                    if (CommunicationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]))
                    {
                        Logger.Log.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                    }

                    // initialize DB
                    if (!DataAccessHelper.Initialize())
                    {
                        Console.ReadLine();
                        return;
                    }

                    Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                    try
                    {
                        // register EF -> GO and GO -> EF mappings
                        RegisterMappings();

                        // initialize PacketSerialization
                        PacketFactory.Initialize<WalkPacket>();

                        NetworkManager<LoginEncryption> networkManager = new NetworkManager<LoginEncryption>("127.0.0.1", port, typeof(LoginPacketHandler), typeof(LoginEncryption), false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("General Error Server", ex);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("General Error", ex);
                    Console.ReadKey();
                }
            }
        }

        private static void RegisterMappings()
        {
            // entities
            DaoFactory.AccountDao.RegisterMapping(typeof(Account)).InitializeMapper();
            DaoFactory.PenaltyLogDao.RegisterMapping(typeof(PenaltyLogDTO)).InitializeMapper();
        }

        #endregion
    }
}