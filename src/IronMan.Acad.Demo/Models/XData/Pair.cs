using Autodesk.AutoCAD.DatabaseServices;

namespace IronMan.Acad.Demo.Models.XData
{
    internal class Pair
    {
        public Pair()
        {

        }
        public Pair(TypedValue key)
        {
            Code = key.TypeCode;
            Value = key.Value;
        }
        public short Code { get; private set; }
        public object Value { get; set; }
    }
}
