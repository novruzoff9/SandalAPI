﻿using AutoMapper;
using EventBus.Base.Abstraction;
using Organization.Application.DTOs.Shelf;
using Shared.Events;

namespace Organization.Application.Common.Services;

public class ShelfProductService
{
    private readonly IApplicationDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;

    public ShelfProductService(IApplicationDbContext context, IMapper mapper, IEventBus eventBus)
    {
        _context = context;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    public async Task<ShelfProductDTO> GetShelfProduct(string productId, int quantity)
    {
        ShelfProduct? shelfProduct = await _context.ShelfProducts
            .Include(sp=>sp.Shelf)
            .Include(sp => sp.Product)
            .FirstOrDefaultAsync(sp => sp.ProductID == productId && sp.Quantity >= quantity);

        if(shelfProduct is null)
        {
            ShelfProductDTO nullShelfProductDTO = new ShelfProductDTO();
            var product = await _context.Products.FindAsync(productId);
            if (product is not null)
            {
                nullShelfProductDTO.ProductName = product.Name;
                nullShelfProductDTO.ImageUrl = product.ImageUrl;
            }
            else
            {
                nullShelfProductDTO.ProductName = "Product not found";
                nullShelfProductDTO.ImageUrl = "Image not found";
            }
            return nullShelfProductDTO;
        }

        var shelfProductDTO = _mapper.Map<ShelfProductDTO>(shelfProduct);
        shelfProductDTO.Quantity = quantity;

        return shelfProductDTO;
    }

    public async Task<bool> CheckProductAvailability(string productId, int quantity)
    {
        var shelfProduct = await _context.ShelfProducts
            .FirstOrDefaultAsync(sp => sp.Id == productId && sp.Quantity >= quantity);
        return shelfProduct is not null;
    }

    public async Task RemoveProductFromShelf(string productId, int quantity, string shelfId, CancellationToken cancellationToken)
    {
        var shelfProduct = _context.ShelfProducts
            .FirstOrDefault(sp => sp.ProductID == productId && sp.ShelfID == shelfId);
        var product = await _context.Products.FindAsync(productId);
        if (shelfProduct is null)
        {
            throw new Exception($"Product {productId} is not available in the warehouse");
        }
        if (product is null)
        {
            throw new Exception($"Product {productId} is not available in the Products");
        }
        shelfProduct.Quantity -= quantity;
        product.Quantity -= quantity;

        if(product.Quantity < product.MinRequire)
        {
            _eventBus.Publish(
                new ProductStockBelowMinimumIntegrationEvent(productId, product.MinRequire, product.Quantity)
                );
        }
        _context.ShelfProducts.Update(shelfProduct);
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
