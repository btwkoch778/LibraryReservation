using System.ComponentModel.DataAnnotations;

namespace LibraryReservationsAPI.Models;

public class ReservationRequest : IValidatableObject
{
    [Required(ErrorMessage = "Kitabın adını daxil edin.")]
    public string BookTitle { get; set; } = "";

    [Required(ErrorMessage = "Oxucunun adını daxil edin.")]
    public string ReaderName { get; set; } = "";

    [Required(ErrorMessage = "Filialın adını daxil edin.")]
    public string LibraryBranch { get; set; } = "";

    public DateOnly PickupDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Nüsxə sayı ən azı 1 olmalıdır.")]
    public int CopyCount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PickupDate < DateOnly.FromDateTime(DateTime.Today))
            yield return new ValidationResult("Götürmə tarixi bu gün və ya gələcək tarix olmalıdır.",
                new[] { nameof(PickupDate) });
    }
}

public class Reservation : ReservationRequest
{
    public int Id { get; init; }
}
