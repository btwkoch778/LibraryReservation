using System.Text.Json;
using LibraryReservationsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryReservationsAPI.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private static readonly List<Reservation> Reservations = new();
    private static readonly object Gate = new();
    private static int nextId = 1;

    [HttpGet]
    public ActionResult<Reservation[]> GetAll(string? title, string? branch,
        bool futureOnly = false, bool multipleCopies = false, string? sort = null)
    {
        var items = Snapshot().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(title))
            items = items.Where(r => r.BookTitle.Contains(title.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(branch))
            items = items.Where(r => r.LibraryBranch.Equals(branch.Trim(), StringComparison.OrdinalIgnoreCase));
        if (futureOnly)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            items = items.Where(r => r.PickupDate > today);
        }
        if (multipleCopies)
            items = items.Where(r => r.CopyCount > 1);

        switch (sort?.ToLowerInvariant())
        {
            case "asc": items = items.OrderBy(r => r.PickupDate); break;
            case "desc": items = items.OrderByDescending(r => r.PickupDate); break;
            case null: break;
            default: return BadRequest("sort parametrinin dəyəri asc və ya desc olmalıdır.");
        }
        return Ok(items.ToArray());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reservation> GetById(int id)
    {
        lock (Gate)
        {
            var reservation = Reservations.Find(r => r.Id == id);
            if (reservation is null) return NotFound();
            return Ok(reservation);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Reservation), StatusCodes.Status201Created)]
    public IActionResult Create(ReservationRequest request)
    {
        lock (Gate)
        {
            var reservation = MakeReservation(nextId++, request);
            Reservations.Add(reservation);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, ReservationRequest request)
    {
        lock (Gate)
        {
            int index = Reservations.FindIndex(r => r.Id == id);
            if (index < 0) return NotFound();
            Reservations[index] = MakeReservation(id, request);
            return NoContent();
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (Gate)
        {
            if (Reservations.RemoveAll(r => r.Id == id) == 0) return NotFound();
            return NoContent();
        }
    }

    [HttpGet("export")]
    [Produces("application/json")]
    public IActionResult Export()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
        return File(JsonSerializer.SerializeToUtf8Bytes(Snapshot(), options),
            "application/json", "reservations.json");
    }

    private static Reservation[] Snapshot()
    {
        lock (Gate) return Reservations.ToArray();
    }

    private static Reservation MakeReservation(int id, ReservationRequest request) => new()
    {
        Id = id,
        BookTitle = request.BookTitle.Trim(),
        ReaderName = request.ReaderName.Trim(),
        LibraryBranch = request.LibraryBranch.Trim(),
        PickupDate = request.PickupDate,
        CopyCount = request.CopyCount
    };
}
