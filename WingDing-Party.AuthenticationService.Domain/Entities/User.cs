namespace WingDing_Party.AuthenticationService.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FirstName { get; set; }
    public required string LastName  { get; set; }
    public required string Email     { get; set; }
    public required string Password  { get; set; }
}
