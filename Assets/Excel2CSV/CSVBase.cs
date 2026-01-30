namespace CSV_SPACE
{
    /// <summary>
    /// CSV数据类的基类
    /// 提供通用的数据访问接口
    /// </summary>
    public class CSVBase
    {
        /// <summary>
        /// 获取CSV数据的ID（通常是第一列）
        /// </summary>
        public virtual string GetID()
        {
            return "";
        }
        
        /// <summary>
        /// 将对象转换为字典格式，便于调试和序列化
        /// </summary>
        public virtual System.Collections.Generic.Dictionary<string, string> ToDictionary()
        {
            var dict = new System.Collections.Generic.Dictionary<string, string>();
            var properties = GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.CanRead)
                {
                    var value = prop.GetValue(this);
                    dict[prop.Name] = value?.ToString() ?? "";
                }
            }
            return dict;
        }
    }
}