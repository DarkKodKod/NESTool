using System.Windows;

namespace NESTool.VOs
{
    public class MouseMoveVO
    {
        public Point Position { get; set; }
        public object OriginalSource { get; set; }
        public object Sender { get; set; }
    }
}
