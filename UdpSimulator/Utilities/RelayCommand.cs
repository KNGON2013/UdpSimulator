using System;
using System.Windows.Input;

namespace UdpSimulator.Utilities
{
    /// <summary>
    /// デリゲートを呼び出し、コマンドを他のオブジェクトに中継(Behavior用).
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;

        public RelayCommand(Action action)
        {
            this.execute = action;
        }

#pragma warning disable CS0067 // 本プロジェクトでは使用しない為、除外.
        public event EventHandler CanExecuteChanged;
#pragma warning disable CS0067

        public bool CanExecute(object parameter)
        {
            return this.execute != null;
        }

        public void Execute(object parameter)
        {
            this.execute?.Invoke();
        }
    }
}
