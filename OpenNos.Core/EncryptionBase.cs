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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.Core
{
    public abstract class EncryptionBase
    {
        #region Instantiation

        public EncryptionBase(bool hasCustomParameter)
        {
            HasCustomParameter = hasCustomParameter;
        }

        #endregion

        #region Properties

        public bool HasCustomParameter { get; set; }

        #endregion

        #region Methods

        public abstract string DecryptCustomParameter(byte[] data, int size);

        public abstract string Decrypt(byte[] data, int size, int customParameter = 0);

        public abstract byte[] Encrypt(string data);

        #endregion
    }
}
