using Shared.ResultTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Products.Commands.IncreaseProductCommand;

public record IncreaseProductCommand(string Id, int Quantity) : IRequest<Response<bool>>;

public class IncreaseProductCommandHandler : IRequestHandler<IncreaseProductCommand, Response<bool>>
{
    private readonly IApplicationDbContext _context;

    public IncreaseProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<bool>> Handle(IncreaseProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (product == null)
        {
            return Response<bool>.Fail("Product not found", 404);
        }
        product.Quantity += request.Quantity;
        _context.Products.Update(product);
        bool result = await _context.SaveChangesAsync(cancellationToken) > 0;
        if (!result)
        {
            return Response<bool>.Fail("Failed to increase product quantity", 500);
        }
        return Response<bool>.Success(true, 200);
    }
}
