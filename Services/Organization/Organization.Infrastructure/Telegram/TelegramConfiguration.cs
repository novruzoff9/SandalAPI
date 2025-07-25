﻿namespace Organization.Infrastructure.Telegram;

public class TelegramConfiguration
{
    public string Token { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
    public string BaseUrl => "https://api.telegram.org/bot";
}
