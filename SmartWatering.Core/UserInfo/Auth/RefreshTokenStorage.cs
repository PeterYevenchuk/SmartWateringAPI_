﻿using System.Collections.Concurrent;

namespace SmartWatering.Core.UserInfo.Auth;

public static class RefreshTokenStorage
{
    private static ConcurrentDictionary<int, string> _refreshTokens = new ConcurrentDictionary<int, string>();

    public static void AddOrUpdateRefreshToken(int userId, string refreshToken)
    {
        _refreshTokens.AddOrUpdate(userId, refreshToken, (key, oldValue) => refreshToken);
    }

    public static string GetRefreshToken(int userId)
    {
        _refreshTokens.TryGetValue(userId, out var refreshToken);
        return refreshToken;
    }

    public static void RemoveRefreshToken(int userId)
    {
        _refreshTokens.TryRemove(userId, out _);
    }
}
