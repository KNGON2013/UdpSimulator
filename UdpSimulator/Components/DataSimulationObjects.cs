using System.Collections.Generic;
using System.Linq;

namespace UdpSimulator.Components
{
    /// <summary>
    /// SimulationObject(模擬データ)コレクション.
    /// </summary>
    internal class DataSimulationObjects
    {
        private readonly List<SimulationObject> objects = new List<SimulationObject>();

        /// <summary>
        /// 読み込みコレクションよりデータ設定.
        /// データ長よりコレクションが短い場合は補完処理を実行.
        /// </summary>
        /// <param name="vs">読み込みコレクション.</param>
        /// <param name="length">データ長.</param>
        public void SetValue(IEnumerable<SimulationObject> vs, int length)
        {
            this.objects.Clear();
            var list = vs.ToList();

            string IndexToName(int index)
            {
                return $"計測値データ{index + 1}";
            }

            foreach (var item in vs.Select((value, index) => (value, index)))
            {
                item.value.Name = IndexToName(item.index);
                this.objects.Add(item.value);
            }

            if (length - list.Count > 0)
            {
                foreach (var a in Enumerable.Range(list.Count, length - list.Count)
                    .Select(_ => new SimulationObject() { Name = IndexToName(_) }))
                {
                    this.objects.Add(a);
                }
            }
        }

        public IEnumerable<SimulationObject> GetValue => this.objects;
    }
}
