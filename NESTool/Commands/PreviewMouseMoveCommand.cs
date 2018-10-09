using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;
using NESTool.VOs;
using System.Windows.Input;

namespace NESTool.Commands
{
    public class PreviewMouseMoveCommand : Command
    {
        public override void Execute(object parameter)
        {
            var mouseEvent = parameter as MouseEventArgs;

            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
            {
                MouseMoveVO vo = new MouseMoveVO();
                vo.Position = mouseEvent.GetPosition(null);
                vo.OriginalSource = mouseEvent.OriginalSource;
                vo.Sender = mouseEvent.Source;

                SignalManager.Get<MouseMoveSignal>().Dispatch(vo);
            }
        }
    }
}
