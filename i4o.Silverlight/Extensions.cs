using System.IO;
using System.Runtime.Serialization;

#if SILVERLIGHT
namespace System
{
//    internal interface ICloneable
//    {
//        object Clone();
//    }
    public static class CloningExtension
    {
        public static T Clone<T>(this T source)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
#endif
