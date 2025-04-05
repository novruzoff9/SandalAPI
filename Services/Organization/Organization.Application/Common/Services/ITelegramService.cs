using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Services;

public interface ITelegramService
{
    /// <summary>
    /// Sends a message to a Telegram chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the message to.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendMessageAsync(string message);

    /// <summary>
    /// Sends a photo to a Telegram chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the photo to.</param>
    /// <param name="photoUrl">The URL of the photo to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPhotoAsync(string photoUrl);

    /// <summary>
    /// Sends a document to a Telegram chat.
    /// </summary>
    /// <param name="pdfStream">The stream of the PDF document to send.</param>
    /// <param name="fileName">The name of the file to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendDocumentAsync(Stream pdfStream, string fileName, string capttion = "");
}
