using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands;

public class NewAnimationFrameCommand : Command
{
    public override void Execute(object? parameter)
    {
        if (parameter == null)
            return;

        object[] values = (object[])parameter;

        FileHandler fileHandler = (FileHandler)values[0];
        string tabID = (string)values[1];

        CharacterModel? model = fileHandler.FileModel as CharacterModel;

        if (model == null)
            return;

        for (int i = 0; i < model.Animations.Count; ++i)
        {
            CharacterAnimation animation = model.Animations[i];

            if (animation.ID == tabID)
            {
                animation.Frames ??= [];

                FrameModel frame = new()
                {
                    Tiles = [],
                    FixToGrid = true
                };

                animation.Frames.Add(frame);

                fileHandler.Save();

                SignalManager.Get<NewAnimationFrameSignal>().Dispatch(tabID);

                return;
            }
        }
    }
}
