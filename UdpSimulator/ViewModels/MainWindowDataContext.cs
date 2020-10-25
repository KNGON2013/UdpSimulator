using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UdpSimulator.Components;
using UdpSimulator.Models;
using UdpSimulator.Utilities;

namespace UdpSimulator.ViewModels
{
    /// <summary>
    /// MainWindow～UDP送信処理用DataContext.
    /// </summary>
    public class MainWindowDataContext : ModelBase, IUdpTransmitter
    {
        private readonly IWeakEventListener propertyChangedListener;

        private readonly IUdpTransmitter udpTransmitter;

        public MainWindowDataContext()
        {
            this.udpTransmitter = new UdpTransmitter()
            {
                CommandDialog = new RelayCommand<string>((_) =>
                {
                    DialogService.Show("入力エラー", _);
                })
            };

            this.propertyChangedListener = new PropertyChangedWeakEventListener(
                    this.RaisePropertyChanged);

            PropertyChangedEventManager.AddListener(
                this.udpTransmitter,
                this.propertyChangedListener,
                string.Empty);
        }

        public bool Connected { get => udpTransmitter.Connected; set => udpTransmitter.Connected = value; }

        public string DestinationIpAddress { get => udpTransmitter.DestinationIpAddress; set => udpTransmitter.DestinationIpAddress = value; }

        public int DestinationPort { get => udpTransmitter.DestinationPort; set => udpTransmitter.DestinationPort = value; }

        public int TransmissionInterval { get => udpTransmitter.TransmissionInterval; set { udpTransmitter.TransmissionInterval = value; } }

        public IEnumerable<SimulationObject> SimulationObjects => udpTransmitter.SimulationObjects;

        public ICommand CommandConnection => udpTransmitter.CommandConnection;

        public ICommand CommandSaveObjects => udpTransmitter.CommandSaveObjects;

        /// <summary>
        /// テキスト入力許可:0～9.
        /// </summary>
        /// <param name="sender">送信元コントロール(TextBox).</param>
        /// <param name="e">EventArgs.</param>
        public void IsAllowedUnsignedInput(object sender, TextCompositionEventArgs e)
        {
            this.IsAllowed(sender as TextBox, e, new Regex("[^0-9]+"));
        }

        /// <summary>
        /// テキスト入力許可:0～9、ピリオド(.)、マイナス符号(-).
        /// </summary>
        /// <param name="sender">送信元コントロール(TextBox).</param>
        /// <param name="e">EventArgs.</param>
        public void IsAllowedNumericalInput(object sender, TextCompositionEventArgs e)
        {
            this.IsAllowed(sender as TextBox, e, new Regex("[^0-9.-]+"));
        }

        /// <summary>
        /// テキスト入力許可.
        /// </summary>
        /// <param name="sender">TextBox.</param>
        /// <param name="e">EventArgs.</param>
        /// <param name="regex">入力可否判別.</param>
        private void IsAllowed(TextBox textBox, TextCompositionEventArgs e, Regex regex)
        {
            var text = textBox.Text + e.Text;
            e.Handled = regex.IsMatch(text);
        }

        /// <summary>
        /// WeakEventListener(ViewModel破棄時メモリリーク対策用).
        /// </summary>
        private class PropertyChangedWeakEventListener : IWeakEventListener
        {
            private readonly Action<string> raisePropertyChangedAction;

            public PropertyChangedWeakEventListener(Action<string> raisePropertyChangedAction)
            {
                this.raisePropertyChangedAction = raisePropertyChangedAction;
            }

            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                // PropertyChangedEventManager時のみ処理.
                if (typeof(PropertyChangedEventManager) != managerType)
                {
                    return false;
                }

                // PropertyChangedEventArgs時のみ処理.
                if (!(e is PropertyChangedEventArgs evt))
                {
                    return false;
                }

                // コンストラクタで渡されたコールバックを呼び出す.
                this.raisePropertyChangedAction(evt.PropertyName);
                return true;
            }
        }
    }
}
