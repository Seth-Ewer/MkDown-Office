using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MkDownOffice.Models;

public abstract class ViewModelBase : INotifyPropertyChanged
{
  #region INotifyPropertyChanged
  public event PropertyChangedEventHandler PropertyChanged;
  protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(backingField, value)) return;
    backingField = value;
    this.OnPropertyChanged(propertyName);
  }
  #endregion INotifyPropertyChanged
}