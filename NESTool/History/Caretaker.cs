using System.Collections.Generic;

namespace NESTool.History
{
    public class Caretaker
    {
        private readonly Stack<Memento> _undoStack = new Stack<Memento>();
        private readonly Stack<Memento> _redoStack = new Stack<Memento>();

        public Memento GetUndoMemento()
        {
            if (_undoStack.Count >= 2)
            {
                _redoStack.Push(_undoStack.Pop());

                return _undoStack.Peek();
            }
            else
            {
                return null;
            }   
        }

        public Memento GetRedoMemento()
        {
            if (_redoStack.Count != 0)
            {
                Memento m = _redoStack.Pop();
                _undoStack.Push(m);
                return m;
            }
            else
            {
                return null;
            }   
        }

        public void InsertMementoForUndoRedo(Memento memento)
        {
            if (memento != null)
            {
                _undoStack.Push(memento);
                _redoStack.Clear();
            }
        }

        public bool IsUndoPossible()
        {
            if (_undoStack.Count >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRedoPossible()
        {
            if (_redoStack.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }   
        }
    }
}
