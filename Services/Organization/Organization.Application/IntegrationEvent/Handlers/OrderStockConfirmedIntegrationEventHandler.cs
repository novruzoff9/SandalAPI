using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.Common.Services;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using Shared.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.IntegrationEvent.Handlers;

public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
{
    private IServiceScopeFactory _serviceScopeFactory;

    public OrderStockConfirmedIntegrationEventHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(OrderStockConfirmedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();


        var prepareOrderDocument = new PrepareOrderDocument(@event);
        using var stream = new MemoryStream();
        prepareOrderDocument.GeneratePdf(stream);
        stream.Position = 0;

        await telegramService.SendDocumentAsync(stream, $"Order-{@event.OrderId}.pdf", "Yeni sifariş sənədi");


        // PDF Faylının adı
        //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedDocuments", $"Order_{@event.Id}.pdf");

        //// Kataloqun mövcud olub-olmadığını yoxla, yoxdursa yarat
        //var directory = Path.GetDirectoryName(filePath);
        //if (!Directory.Exists(directory))
        //{
        //    Directory.CreateDirectory(directory);
        //}

        //// PDF Faylını generasiya et və yaz
        //prepareOrderDocument.GeneratePdf(filePath);

        //Console.WriteLine($"PDF faylı yaradıldı: {filePath}");



    }
}
