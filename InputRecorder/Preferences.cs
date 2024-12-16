using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InputRecorder;

[Serializable]
public class Preferences : INotifyPropertyChanged
{
    // private bool _isEnabledRecording = false;

    // public bool IsEnabledRecording
    // {
    //     get => _isEnabledRecording;
    //     set
    //     {
    //         _isEnabledRecording = value;
    //         OnPropertyChanged();
    //     }
    // }

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
