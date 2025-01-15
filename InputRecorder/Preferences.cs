using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InputRecorder;

[Serializable]
public class Preferences : INotifyPropertyChanged
{
    private bool _turnoffInputRecordingEverytime = true;

    public bool TurnoffInputRecordingEverytime
    {
        get => _turnoffInputRecordingEverytime;
        set
        {
            _turnoffInputRecordingEverytime = value;
            OnPropertyChanged();
        }
    }
    private bool _isEnabledRecording = false;

    public bool IsEnabledRecording
    {
        get => _isEnabledRecording;
        set
        {
            _isEnabledRecording = value;
            OnPropertyChanged();
        }
    }
    private bool _isSimplifyInput = false;

    public bool IsSimplifyInput
    {
        get => _isSimplifyInput;
        set
        {
            _isSimplifyInput = value;
            OnPropertyChanged();
        }
    }

    #region INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
