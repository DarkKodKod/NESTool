using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArchitectureLibrary.Commands;

public class Command : ICommand, IDisposable
{
    private bool _isExecuting = false;

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            RaiseCanExecuteChanged();
        }
    }

    private EventHandler? _internalCanExecuteChanged;

    public virtual event EventHandler? CanExecuteChanged
    {
        add
        {
            _internalCanExecuteChanged += value;
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            _internalCanExecuteChanged -= value;
            CommandManager.RequerySuggested -= value;
        }
    }

    public virtual bool CanExecute(object? parameter)
    {
        return !IsExecuting;
    }

    public virtual Task ExecuteAsync(object? parameter)
    {
        return Task.FromResult<object?>(null);
    }

    public virtual async void Execute(object? parameter)
    {
        IsExecuting = true;
        await ExecuteAsync(parameter).ConfigureAwait(false);
        IsExecuting = false;
    }

    public void RaiseCanExecuteChanged()
    {
        _internalCanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Deactivate()
    {
    }

    #region IDisposable Support
    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            Deactivate();

            disposedValue = true;
        }
    }

    ~Command()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}