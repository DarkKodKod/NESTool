namespace NESTool.VOs;

public record EventVO
{
    public object? OriginalSource { get; init; }
    public object? Sender { get; init; }
}
