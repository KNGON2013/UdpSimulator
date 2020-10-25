using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UdpSimulator.Components
{
    /// <summary>
    /// SimulationObject(Date/Data)統合コレクション.
    /// 公開コレクションは日時データ+模擬データ(CSV読み込みデータ表示数依存).
    /// コレクション保存時、CSV読み込みデータ表示数が
    /// 読み込みデータ総数未満だった場合は読み込んだデータ総数も含めて保存処理を行う.
    /// </summary>
    public class SimulationObjectCollection
    {
        private readonly CsvConverter csvConverter = new CsvConverter();

        private readonly DateSimulationObject dateObjects = new DateSimulationObject();

        private readonly DataSimulationObjects dataObjects = new DataSimulationObjects();

        public SimulationObjectCollection()
        {
            this.csvConverter.Load();
            this.dateObjects.SetValue(csvConverter.Items.Take(DateSimulationObject.ValueLength));
            this.dataObjects.SetValue(csvConverter.Items.Skip(DateSimulationObject.ValueLength), csvConverter.DataLength);

            foreach (var a in this.dateObjects.GetValue)
            {
                a.Enabled = true;
                a.No = this._Objects.Count;
                this._Objects.Add(a);
            }

            foreach (var a in this.dataObjects.GetValue.Take(this.csvConverter.DataLength))
            {
                a.No = this._Objects.Count;
                a.Enabled = (this._Objects.Count - DateSimulationObject.ValueLength) < this.csvConverter.DataLength;

                this._Objects.Add(a);
            }
        }

        private readonly ObservableCollection<SimulationObject> _Objects = new ObservableCollection<SimulationObject>();
        public IEnumerable<SimulationObject> Objects => this._Objects;

        public void Save()
        {
            this.csvConverter.Save(dateObjects.GetValue.Concat(dataObjects.GetValue));
        }
    }
}
