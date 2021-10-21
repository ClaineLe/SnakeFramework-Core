using UnityEngine;
static public partial class ObjectExtensions
{
    /// <summary>
    /// 判断对象是否为null值
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static bool IsNull(this Object obj)
    {
        return obj == null || obj.Equals(null);
    }
}