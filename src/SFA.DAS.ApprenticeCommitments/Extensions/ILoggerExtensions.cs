﻿using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Extensions
{
    public static class ILoggerExtensions
    {
        public static IDisposable BeginPropertyScope(
            this ILogger logger,
            params ValueTuple<string, object>[] properties)
        {
            var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
            return logger.BeginScope(dictionary);
        }
    }
}
