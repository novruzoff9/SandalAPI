using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Infrastructure.Telegram;

public class TelegramConfiguration
{
    public string Token { get; set; } = string.Empty;
    public string ChatId { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.telegram.org/bot";
}
