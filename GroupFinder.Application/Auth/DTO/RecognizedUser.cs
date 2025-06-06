namespace GroupFinder.Application.Auth.DTO;

public sealed class RecognizedUser(Guid id)
{
    public Guid Id { get; } = id;
}