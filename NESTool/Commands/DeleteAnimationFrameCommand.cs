using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class DeleteAnimationFrameCommand : Command
    {
        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;
            string tabID = (string)values[0];
            int frameIndex = (int)values[1];
            FileHandler fileHandler = (FileHandler)values[2];

            CharacterModel model = fileHandler.FileModel as CharacterModel;

            bool frameDeleted = false;

            for (int i = 0; i < model.Animations.Count; ++i)
            {
                CharacterAnimation animation = model.Animations[i];

                if (animation.ID == tabID && animation.Frames != null)
                {
                    for (int j = 0; j < animation.Frames.Count; ++j)
                    {
                        if (j == frameIndex)
                        {
                            animation.Frames[j].Tiles = null;

                            SignalManager.Get<DeleteAnimationFrameSignal>().Dispatch(tabID, frameIndex);

                            frameDeleted = true;
                        }
                        else if (frameDeleted)
                        {
                            FrameModel prevFrame = animation.Frames[j - 1];
                            animation.Frames[j - 1] = animation.Frames[j];
                            animation.Frames[j] = prevFrame;
                        }
                    }

                    break;
                }
            }

            if (frameDeleted)
            {
                fileHandler.Save();
            }
        }
    }
}