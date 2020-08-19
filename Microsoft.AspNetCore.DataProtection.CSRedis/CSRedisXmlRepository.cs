﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CSRedis;
using Microsoft.AspNetCore.DataProtection.Repositories;


namespace Microsoft.AspNetCore.DataProtection.CSRedis
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class CSRedisXmlRepository : IXmlRepository
    {

        private readonly CSRedisClient _redisClient;
        private readonly string _key;

        /// <summary>
        /// Creates a <see cref="CSRedisXmlRepository"/> with keys stored at the given directory.
        /// </summary>
        public CSRedisXmlRepository(CSRedisClient redisClient, string key)
        {
            _redisClient = redisClient;
            _key = key;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            // Note: Inability to read any value is considered a fatal error (since the file may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a value contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.




            foreach (var value in _redisClient.LRange(_key, 0, -1))
            {
                yield return XElement.Parse(value);
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {

            _redisClient.RPush(_key, element.ToString(SaveOptions.DisableFormatting));

        }
    }
}