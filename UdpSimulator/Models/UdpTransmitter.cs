using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using System.Windows.Threading;
using UdpSimulator.Components;
using UdpSimulator.Utilities;

namespace UdpSimulator.Models
{
    /// <summary>
    /// UDP送信処理(DataContext対応).
    /// DispatcherTimer使用.
    /// </summary>
    public class UdpTransmitter : ModelBase, IUdpTransmitter
    {
        private readonly SimulationObjectCollection simulationObjectCollection = new SimulationObjectCollection();

        private readonly DispatcherTimer udpDispatcher = new DispatcherTimer();

        private UdpClient client = new UdpClient();

        public UdpTransmitter()
        {
            this.udpDispatcher.Tick += (obj, sender) => this.SendUdp();
        }

        private bool _Connected = false;
        public bool Connected
        {
            get => this._Connected;
            set
            {
                if (this._Connected != value)
                {
                    this._Connected = value;
                    this.RaisePropertyChanged(nameof(this.Connected));
                }
            }
        }

        public string _DestinationIpAddress = "192.168.90.255";
        public string DestinationIpAddress
        {
            get => this._DestinationIpAddress;
            set
            {
                this._DestinationIpAddress = value;

                if (!this.ValidateDestinationIpAddress(value))
                {
                    throw new ArgumentException();
                }
            }
        }

        public int _DestinationPort = 50001;
        public int DestinationPort
        {
            get => this._DestinationPort;
            set
            {
                this._DestinationPort = value;

                if (!ValidateDestinationPort(value))
                {
                    throw new ArgumentException();
                }
            }
        }

        public int _TransmissionInterval = 1;
        public int TransmissionInterval
        {
            get => this._TransmissionInterval;
            set
            {
                this._TransmissionInterval = value;

                if (!ValidateTransmissionInterval(value))
                {
                    throw new ArgumentException();
                }
            }
        }

        public IEnumerable<SimulationObject> SimulationObjects => this.simulationObjectCollection.Objects;

        private ICommand _CommandConnection;
        public ICommand CommandConnection => this._CommandConnection ?? (this._CommandConnection = new RelayCommand(() =>
        {
            if (this._Connected)
            {
                this.udpDispatcher.Stop();
                this.client.Close();
                this.Connected = false;
            }
            else
            {
                var digitErrors = this.SimulationObjects.Where(_ => _.Digits == 0 || _.Digits > 10);

                if (digitErrors.Any())
                {
                    var val = string.Join(", ", digitErrors.Select(_ => $"{_.No}"));

                    CommandDialog.Execute($"設定バイト数エラーが有ります。{Environment.NewLine}CSVファイルを再設定してください。{Environment.NewLine}No:{val}");
                    return;
                }

                if (this.client.Client == null)
                {
                    this.client = new UdpClient(this._DestinationPort);
                }

                if (!ValidateTransmissionInterval(this._TransmissionInterval))
                {
                    return;
                }

                try
                {
                    // PCアドレス      :192.168.90.3:50001
                    // 制御機器アドレス:192.168.90.1:50001/192.168.90.2:50001
                    // アプリ宛先設定  :192.168.90.255(ブロードキャストアドレス)

                    this.client.EnableBroadcast = true;
                    this.client.Connect(this._DestinationIpAddress,this._DestinationPort);
                    this.SendUdp();

                    this.udpDispatcher.Interval = TimeSpan.FromSeconds(this._TransmissionInterval);
                    this.udpDispatcher.Start();

                    this.Connected = true;
                }
                catch (SocketException)
                {
                    ValidateDestinationIpAddress(this._DestinationIpAddress);
                }
                catch (ArgumentOutOfRangeException)
                {
                    ValidateDestinationPort(this._DestinationPort);
                }
            }
        }));

        private ICommand _CommandSaveObjects;
        public ICommand CommandSaveObjects => this._CommandSaveObjects ?? (this._CommandSaveObjects = new RelayCommand(() =>
        {
            this.simulationObjectCollection.Save();

            Console.WriteLine("SaveObjects");
        }));

        public ICommand CommandDialog { get; set; }

        private void SendUdp()
        {
            if (this.client.Client != null && this.client.Client.Connected)
            {
                var converted = this.simulationObjectCollection.Objects.Where(_ => _.Enabled).Select(_ => _.ConvertToBytes());

                byte[] buffer = new byte[1024];
                var dstIndex = 0;
                foreach (var a in converted)
                {
                    var array = a.ToArray();

                    if (array.Length > buffer.Length)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    if (dstIndex + array.Length > buffer.Length)
                    {
                        this.client.Send(buffer, dstIndex);
                        dstIndex = 0;
                    }

                    Array.Copy(array, 0, buffer, dstIndex, array.Length);
                    dstIndex += array.Length;
                }

                if (dstIndex > 0)
                {
                    this.client.Send(buffer, dstIndex);
                }
            }
        }

        private bool ValidateDestinationIpAddress(string value)
        {
            if (IPAddress.TryParse(value, out _))
            {
                return true;
            }
            else
            {
                CommandDialog.Execute($"宛先IPアドレス設定が{Environment.NewLine}不正です。");
                return false;
            }
        }

        private bool ValidateDestinationPort(int value)
        {
            if (value >= 0 && value <= 65535)
            {
                return true;
            }
            else
            {
                CommandDialog.Execute($"宛先ポート設定範囲は{Environment.NewLine}0～65535です。");
                return false;
            }
        }

        private bool ValidateTransmissionInterval(int value)
        {
            if (value >= 1 && value <= 10)
            {
                return true;
            }
            else
            {
                CommandDialog.Execute($"送信間隔(秒)設定範囲は{Environment.NewLine}1～10です。");
                return false;
            }
        }
    }
}
