namespace M295_ILBA24.DTOs;

public record RentalResponseDto(
    int RentalId,
    ushort CustomerId,
    ushort InventoryId,
    ushort StaffId,
    DateTime RentalDate,
    DateTime? ReturnDate,
    DateTime LastUpdate,
    string? FilmTitle,
    string? CustomerName
);

public record RentalRequestDto(
    ushort CustomerId,
    ushort InventoryId,
    ushort StaffId,
    ushort Rentalid
);