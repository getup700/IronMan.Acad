using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.ObjectModel;
using System.Linq;

namespace IronMan.Acad.Demo.Models.XData
{
    internal class PairCollection
    {
        public PairCollection(ResultBuffer rb)
        {
            if (rb == null)
            {
                return;
            }
            foreach (var item in rb)
            {
                Pairs.Add(new Pair(item));
            }
        }
        public ObservableCollection<Pair> Pairs { get; set; }

        public ResultBuffer XData()
        {
            if (Pairs == null || Pairs.Count == 0)
            {
                return new();
            }
            var result = new ResultBuffer();

            //保证ResultBuffer第一个是RegAppName
            var appPair = Pairs.FirstOrDefault(x => x.Code == (int)DxfCode.ExtendedDataRegAppName);
            if (appPair == null)
            {
                return null;
            }
            result.Add(appPair);
            Pairs.Remove(appPair);

            foreach (var pair in Pairs)
            {
                result.Add(new TypedValue(pair.Code, pair.Value));
            }
            return result;
        }
    }
}
