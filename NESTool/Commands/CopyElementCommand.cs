﻿using ArchitectureLibrary.Commands;

namespace NESTool.Commands
{
    public class CopyElementCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            return false;
        }

        public override void Execute(object parameter)
        {
            //
        }
    }
}
