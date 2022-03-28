using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class StopCharacterAnimationCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;
            CharacterModel model = (CharacterModel)values[0];

            if (model == null)
            {
                return false;
            }

            string tabID = (string)values[1];

            foreach (CharacterAnimation anim in model.Animations)
            {
                if (anim.ID == tabID && anim.Frames != null)
                {
                    foreach (FrameModel frame in anim.Frames)
                    {
                        if (frame.Tiles != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;
            CharacterModel model = (CharacterModel)values[0];
            string tabID = (string)values[1];

            SignalManager.Get<StopCharacterAnimationSignal>().Dispatch(tabID);
        }
    }
}
