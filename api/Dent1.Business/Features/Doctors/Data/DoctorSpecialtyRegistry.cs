using System.Collections.Concurrent;

namespace Dent1.Business.Features.Doctors.Data;

/// <summary>
/// In-memory specialty until Doctor profile storage exists (no migration in this phase).
/// </summary>
internal static class DoctorSpecialtyRegistry
{
    private static readonly ConcurrentDictionary<Guid, string> Specialties = new();

    static DoctorSpecialtyRegistry()
    {
        Set(Guid.Parse("b1c2d3e4-1001-0000-0000-000000000001"), "Endodontics");
        Set(Guid.Parse("b1c2d3e4-1002-0000-0000-000000000002"), "General Dentistry");
    }

    public static string Get(Guid doctorId) =>
        Specialties.TryGetValue(doctorId, out var specialty)
            ? specialty
            : "General Dentistry";

    public static void Set(Guid doctorId, string specialty) =>
        Specialties[doctorId] = string.IsNullOrWhiteSpace(specialty) ? "General Dentistry" : specialty.Trim();

    public static void Remove(Guid doctorId) => Specialties.TryRemove(doctorId, out _);
}
