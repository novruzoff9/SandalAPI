using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Organization.Application.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Infrastructure.Telegram;

public class TelegramService : ITelegramService
{
    private readonly TelegramConfiguration _configuration;
    public TelegramService(IOptions<TelegramConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }
    public async Task SendDocumentAsync(Stream documentStream, string fileName, string caption = "")
    {
        string apiUrl = $"https://api.telegram.org/bot{_configuration.Token}/sendDocument";

        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(documentStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

        form.Add(new StringContent(_configuration.ChatId), "chat_id");
        if (!string.IsNullOrWhiteSpace(caption))
            form.Add(new StringContent(caption), "caption");
        form.Add(fileContent, "document", fileName);

        var response = await client.PostAsync(apiUrl, form);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to send document: {response.StatusCode} - {error}");
        }
    }


    public async Task SendMessageAsync(string message)
    {
        string apiUrl = $"https://api.telegram.org/bot{_configuration.Token}/sendMessage?chat_id={_configuration.ChatId}&text={message}";
        using var client = new HttpClient();
        var payload = new
        {
            chat_id = _configuration.ChatId,
            text = message
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to send message: {response.StatusCode}");
        }
    }

    public Task SendPhotoAsync(string photoUrl)
    {
        throw new NotImplementedException();
    }
}
