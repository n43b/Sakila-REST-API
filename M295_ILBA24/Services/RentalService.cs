using M295_ILBA24.Context;
using M295_ILBA24.DTOs;
using M295_ILBA24.Entities;
using M295_ILBA24.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace M295_ILBA24.Services;

public class RentalService(ILogger<RentalService> logger, SakilaDbContext dbContext)
{
    public async Task<ICollection<RentalResponseDto>> GetFilteredRentalsAsync(
        string? customerFirstName, 
        string? customerLastName, 
        string? filmTitle, 
        string? staffFirstName, 
        string? staffLastName)
    {
        var query = dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Staff)
            .Include(r => r.Inventory).ThenInclude(i => i.Film)
            .AsQueryable();
   
        if (!string.IsNullOrWhiteSpace(customerFirstName))
            query = query.Where(r => r.Customer.FirstName.StartsWith(customerFirstName));
   
        if (!string.IsNullOrWhiteSpace(customerLastName))
            query = query.Where(r => r.Customer.LastName.StartsWith(customerLastName));
   
        if (!string.IsNullOrWhiteSpace(filmTitle))
            query = query.Where(r => r.Inventory.Film.Title.StartsWith(filmTitle));
   
        if (!string.IsNullOrWhiteSpace(staffFirstName))
            query = query.Where(r => r.Staff.FirstName.StartsWith(staffFirstName));
   
        if (!string.IsNullOrWhiteSpace(staffLastName))
            query = query.Where(r => r.Staff.LastName.StartsWith(staffLastName));
   
        var rentals = await query.ToListAsync();
        return rentals.Adapt<List<RentalResponseDto>>();
    }
    public async Task<RentalResponseDto> GetById(int id)
    {
        var rental = await dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Staff)
            .Include(r => r.Inventory).ThenInclude(i => i.Film)
            .FirstOrDefaultAsync(r => r.RentalId == id);

        if (rental == null)
            throw new ResourceNotFoundException($"Rental with ID {id} not found");

        return rental.Adapt<RentalResponseDto>();
    }
    public async Task<RentalResponseDto> CreateRentalAsync(RentalRequestDto rentalDto)
    {
        var rental = rentalDto.Adapt<Rental>();
        rental.RentalDate = DateTime.UtcNow;
        rental.LastUpdate = DateTime.UtcNow;

        dbContext.Rentals.Add(rental);
        await dbContext.SaveChangesAsync();

        return await GetById(rental.RentalId);
    }

    public async Task<RentalResponseDto> CloseRentalAsync(int id)
    {
        var rental = await dbContext.Rentals.FindAsync(id);
        if (rental == null)
            throw new ResourceNotFoundException($"Rental with ID {id} not found");

        rental.ReturnDate = DateTime.UtcNow;
        rental.LastUpdate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return rental.Adapt<RentalResponseDto>();
    }
}
