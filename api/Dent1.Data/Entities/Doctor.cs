namespace Dent1.Data.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}
