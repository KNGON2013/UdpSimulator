using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace UdpSimulator.Components
{
    /// <summary>
    /// SimulationObject(日時).
    /// DispatcherTimer使用.
    /// </summary>
    internal class DateSimulationObject
    {
        /// <summary>
        /// コレクション長(日時).
        /// </summary>
        public static readonly int ValueLength = 6;

        private readonly SimulationObject[] objects;

        public DateSimulationObject()
        {
            this.objects = new SimulationObject[ValueLength];

            this.objects[0] = new SimulationObject() { IsReadOnly = true, Name = "Year", Digits = 4 };
            this.objects[1] = new SimulationObject() { IsReadOnly = true, Name = "Month", Digits = 2 };
            this.objects[2] = new SimulationObject() { IsReadOnly = true, Name = "Day", Digits = 2 };
            this.objects[3] = new SimulationObject() { IsReadOnly = true, Name = "Hour", Digits = 2 };
            this.objects[4] = new SimulationObject() { IsReadOnly = true, Name = "Minute", Digits = 2 };
            this.objects[5] = new SimulationObject() { IsReadOnly = true, Name = "Second", Digits = 2 };

            var dispahter = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(250),
            };

            dispahter.Tick += (sender, e) =>
            {
                var now = DateTime.Now;

                double? CheckDigits(double value, byte digits)
                {
                    if (digits == 0)
                    {
                        return null;
                    }
                    var str = $"{value}";

                    return double.Parse(new string(str.Skip(str.Length - digits).ToArray()));
                }

                this.objects[0].Value = CheckDigits(now.Year, this.objects[0].Digits);
                this.objects[1].Value = CheckDigits(now.Month, this.objects[1].Digits);
                this.objects[2].Value = CheckDigits(now.Day, this.objects[2].Digits);
                this.objects[3].Value = CheckDigits(now.Hour, this.objects[3].Digits);
                this.objects[4].Value = CheckDigits(now.Minute, this.objects[4].Digits);
                this.objects[5].Value = CheckDigits(now.Second, this.objects[5].Digits);
            };

            dispahter.Start();
        }

        /// <summary>
        /// 読み込みコレクションよりデータ設定.
        /// コレクション長(日時)よりコレクションが短い場合は補完処理を実行.
        /// </summary>
        /// <param name="vs">読み込みコレクション.</param>
        public void SetValue(IEnumerable<SimulationObject> vs)
        {
            var array = vs.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                this.objects[i].Digits = array[i].Digits;
            }
        }

        public IEnumerable<SimulationObject> GetValue => this.objects;
    }
}
