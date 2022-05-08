using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class NewAnimationFrameCommand : Command
    {
        public override void Execute(object parameter)
        {
            object[] values = (object[])parameter;

            FileHandler fileHandler = (FileHandler)values[0];
            string tabID = (string)values[1];

            CharacterModel model = fileHandler.FileModel as CharacterModel;

            for (int i = 0; i < model.Animations.Length; ++i)
            {
                ref CharacterAnimation animation = ref model.Animations[i];

                if (animation.ID == tabID)
                {
                    if (animation.Frames == null)
                    {
                        animation.Frames = new FrameModel[64];
                    }

                    for (int j = 0; j < animation.Frames.Length; ++j)
                    {
                        FrameModel frame = animation.Frames[j];

                        if (frame.Tiles == null)
                        {
                            animation.Frames[j].Tiles = new CharacterTile[CharacterTile.MaxCharacterTiles];
                            animation.Frames[j].FixToGrid = true;

                            fileHandler.Save();

                            SignalManager.Get<NewAnimationFrameSignal>().Dispatch(tabID);

                            return;
                        }
                    }

                    break;
                }
            }
        }
    }
}
