﻿namespace SmartWatering.Core.Helpers.PasswordHasher;

public interface IPasswordHash
{
    public string EncryptPassword(string password, byte[] salt);
}
