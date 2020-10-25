using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UdpSimulator.Utilities
{
    /// <summary>
    /// DataContext通知用NotifyPropertyChanged実装.
    /// </summary>
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
