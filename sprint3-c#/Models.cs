// Models
public record User
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? InstitutionId { get; set; }
    public override string ToString() => $"User {{Id={Id}, Name={Name}, Email={Email}, InstitutionId={InstitutionId}}}";
}

public record Institution
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public override string ToString() => $"Institution {{Id={Id}, Name={Name}, Address={Address}}}";
}