using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Common.Services;

public static class RandomIdService
{
    private static readonly char[] Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    private static readonly Random Random = new Random();

    public static string GenerateRandomCode(int length)
    {
        var stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            char randomChar = Characters[Random.Next(Characters.Length)];
            stringBuilder.Append(randomChar);
        }
        return stringBuilder.ToString();
    }
}
