﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class SaveCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return false;
        }

        public override void Execute(object parameter)
        {
            SignalManager.Get<SaveSuccessSignal>().Dispatch();
        }
    }
}
