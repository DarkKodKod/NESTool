using System.Windows;

namespace NESTool.VOs;

public record MouseMoveVO : EventVO
{
    public Point Position { get; init; }
}
