using M295_ILBA24.DTOs;
using M295_ILBA24.Exceptions;
using M295_ILBA24.Services;
using Microsoft.AspNetCore.Mvc;

namespace M295_ILBA24.Controllers;

[ApiController]
[Route("[controller]")]
public class RentalController(ILogger<RentalController> logger, RentalService service) : ControllerBase
{
    /// <summary>
    /// Gibt alle Rentals zurück, die mit den angegebenen Parametern übereinstimmen (StartsWith).
    /// </summary>
    /// <param name="customerFirstName">Kundenvorname</param>
    /// <param name="customerLastName">Kundennachname</param>
    /// <param name="filmTitle">Filmname</param>
    /// <param name="staffFirstName">Mitarbeitervorname</param>
    /// <param name="staffLastName">Mitarbeiternachname</param>
    /// <returns>Liste von passenden Rentals</returns>
    /// <response code="200">Erfolgreiche Abfrage</response>
    
    [HttpGet]
    public async Task<ActionResult<ICollection<RentalResponseDto>>> GetFilteredRentalsAsync(
        [FromQuery] string? customerFirstName = null,
        [FromQuery] string? customerLastName = null,
        [FromQuery] string? filmTitle = null,
        [FromQuery] string? staffFirstName = null,
        [FromQuery] string? staffLastName = null)
    {
        var rentals = await service.GetFilteredRentalsAsync(
            customerFirstName, 
            customerLastName, 
            filmTitle, 
            staffFirstName, 
            staffLastName);
        return Ok(rentals);
    }
    
    /// <summary>
    /// Gibt ein einzelnes Rental anhand der ID zurück.
    /// </summary>
    /// <param name="id">Die ID des gesuchten Rentals</param>
    /// <returns>RentalResponseDto mit Informationen zum Rental</returns>
    /// <response code="200">Rental gefunden</response>
    /// <response code="404">Rental nicht gefunden</response>
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentalResponseDto>> GetRentalByIdAsync(int id)
    {
        try
        {
            var rental = await service.GetById(id);
            return Ok(rental);
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Erstellt eine neue Rental (Leihgabe).
    /// </summary>
    /// <param name="rental">Das RentalRequestDto mit Kundennummer, Mitarbeiter-ID und Inventar-ID</param>
    /// <returns>Das erstellte Rental als RentalResponseDto</returns>
    /// <response code="201">Rental erfolgreich erstellt</response>
    
    [HttpPost]
    public async Task<ActionResult<RentalResponseDto>> CreateRentalAsync([FromBody] RentalRequestDto rental)
    {
        var createdRental = await service.CreateRentalAsync(rental);
        return Created($"/rental/{createdRental.RentalId}", createdRental);
    }

    /// <summary>
    /// Schließt ein Rental ab (setzt das Rückgabedatum).
    /// </summary>
    /// <param name="id">Die ID des Rentals</param>
    /// <returns>Das abgeschlossene Rental als RentalResponseDto</returns>
    /// <response code="200">Rental erfolgreich abgeschlossen</response>
    /// <response code="404">Rental mit angegebener ID nicht gefunden</response>
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RentalResponseDto>> CloseRentalAsync(int id)
    {
        try
        {
            var updatedRental = await service.CloseRentalAsync(id);
            return Ok(updatedRental);
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
}
