using System.Collections.Generic;
using System.Linq;
using System.Text;
using UdpSimulator.Utilities;

namespace UdpSimulator.Components
{
    /// <summary>
    /// SimulationObject(DataContext対応).
    /// </summary>
    public class SimulationObject : ModelBase
    {
        public int No { get; set; }

        public bool Enabled { get; set; }

        public bool IsReadOnly { get; set; }

        public string Name { get; set; }

        public byte Digits { get; set; } = 0;

        private double? _Value;
        public double? Value
        {
            get => this._Value;
            set
            {
                if (this._Value != value)
                {
                    if (!value.HasValue || (value.HasValue && ValidateDigits(value.Value)))
                    {
                        this._Value = value;
                        RaisePropertyChanged(nameof(this.Value));
                    }
                }
            }
        }

        /// <summary>
        /// ASCIIコード変換.
        /// 未入力(Null)時はアスタリスク(*:0x2a)、Digits不足時はアンダーバー(_:0x5f)にて補完.
        /// </summary>
        /// <returns>変換コレクション.</returns>
        public IEnumerable<byte> ConvertToBytes()
        {
            if (!this._Value.HasValue)
            {
                return Enumerable.Repeat<byte>(0x2a, this.Digits);
            }
            else
            {
                var val = Encoding.ASCII.GetBytes($"{this._Value.Value}");

                return Enumerable.Repeat<byte>(0x5f, this.Digits - val.Length).Concat(val);
            }
        }

        private bool ValidateDigits(double value)
        {
            return Encoding.ASCII.GetBytes(value.ToString()).Length <= this.Digits;
        }
    }
}
