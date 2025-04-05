using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Shared.Events.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Services;

public class PrepareOrderDocument : IDocument
{
    private OrderStockConfirmedIntegrationEvent order;

    public PrepareOrderDocument(OrderStockConfirmedIntegrationEvent order)
    {
        this.order = order;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Header().Element(Header);
            page.Content().Element(Content);
            //page.Footer().Element(Footer);
        });
    }

    private void Header(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                var scale = 0.8f;
                column.Item().Text("Order Document");
                column.Item().Scale(scale).Text($"Order Id: {order.Id}");
                column.Item().Scale(scale).Text($"Order Date: {order.CreatedDate}");
            });
        });
    }

    private void Content(IContainer container)
    {
        container.Column(column =>
        {
            var scale = 0.8f;
            column.Item().Scale(scale).Text("Products");

            // Cədvəli ayrıca column.Item() içində göstər
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("#");
                    header.Cell().Text("Məhsul");
                    header.Cell().Text("Miqdar");
                    header.Cell().Text("Rəf");
                });

                int i = 1;
                foreach (var item in order.ShelfProducts)
                {
                    table.Cell().Text(i.ToString());
                    table.Cell().Text(item.ProductName);
                    table.Cell().Text(item.Quantity.ToString());
                    table.Cell().Text(item.ShelfCode);
                    i++;
                }
            });
        });
    }

    private void Footer(IContainer container)
    {
        throw new NotImplementedException();
    }
}
