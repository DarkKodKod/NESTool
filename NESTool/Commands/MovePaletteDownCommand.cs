﻿using ArchitectureLibrary.Commands;
using ArchitectureLibrary.Signals;
using NESTool.Models;
using NESTool.Signals;

namespace NESTool.Commands
{
    public class MovePaletteDownCommand : Command
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            object[] values = (object[])parameter;

            if (values[0] == null)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            object[] values = (object[])parameter;

            ElementPaletteModel palette = (ElementPaletteModel)values[0];

            SignalManager.Get<MovePaletteDownSignal>().Dispatch(palette);
        }
    }
}
