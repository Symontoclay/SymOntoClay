/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.PlatformImplementations
{
    public class CommonNLogLogger : IPlatformLogger
    {
        private static readonly CommonNLogLogger __instance = new CommonNLogLogger();
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static CommonNLogLogger Instance => __instance;

        private CommonNLogLogger()
        {
        }

        public void WriteLn(string message)
        {
            _logger.Info(message);
        }

        public void WriteLnRawLogChannel(string message)
        {
            _logger.Info(message);
        }

        public void WriteLnRawLog(string message)
        {
            _logger.Info(message);
        }

        public void WriteLnRawWarning(string message)
        {
            _logger.Info(message);
        }

        public void WriteLnRawError(string message)
        {
            _logger.Info(message);
        }
    }
}
