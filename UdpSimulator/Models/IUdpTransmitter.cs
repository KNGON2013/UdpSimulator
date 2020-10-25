using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using UdpSimulator.Components;

namespace UdpSimulator.Models
{
    /// <summary>
    /// UDP送信処理 DataContext対応用インターフェース.
    /// </summary>
    public interface IUdpTransmitter : INotifyPropertyChanged
    {
        bool Connected { get; set; }

        string DestinationIpAddress { get; set; }

        int DestinationPort { get; set; }

        int TransmissionInterval { get; set; }

        IEnumerable<SimulationObject> SimulationObjects { get; }

        ICommand CommandConnection { get; }

        ICommand CommandSaveObjects { get; }
    }
}
