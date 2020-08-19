﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using CSRedis;
using Microsoft.AspNetCore.DataProtection.CSRedis;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;


namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
    /// </summary>
    public static class CSRedisDataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";



        /// <summary>
        /// Configures the data protection system to persist keys to the specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="redisClient"></param>
        /// <param name="key"></param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToCSRedis(this IDataProtectionBuilder builder, CSRedisClient redisClient, string key = DataProtectionKeysName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (redisClient == null)
            {
                throw new ArgumentNullException(nameof(redisClient));
            }
            return PersistKeysToCSRedisInternal(builder, redisClient, key);
        }

        private static IDataProtectionBuilder PersistKeysToCSRedisInternal(IDataProtectionBuilder builder, CSRedisClient redisClient, string key)
        {
            builder.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new CSRedisXmlRepository(redisClient, key);
            });
            return builder;
        }
    }
}