using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace TLib.Software
{
    /// <summary>
    /// 无污染无公害的序列化器
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// 垃圾回收的时候保存一下
        /// </summary>
        ~Serializer()
        {
            Save();
        }
        /// <summary>
        /// 变量字典,变量名,变量值;
        /// </summary>
        private SerializableDictionary<string, object> Variables { get; set; }
        /// <summary>
        /// 引用,exp:MainWindow
        /// </summary>
        private readonly object reference;
        /// <summary>
        /// XML文件路径
        /// </summary>
        private readonly string file_XML = string.Empty;

        private List<string> lstVarName;
        /// <summary>
        /// 创建序列化器,并通过Load方法加载已保存的值
        /// </summary>
        /// <param name="reference">填写this以传递引用</param>
        /// <param name="file_XML">XML文件路径</param>
        /// <param name="lstVarName">属性列表</param>
        public Serializer(object reference, string file_XML, List<string> lstVarName)
        {
            this.file_XML = file_XML;
            this.reference = reference;
            this.lstVarName = lstVarName;



            Load();
            foreach (var item in lstVarName)//字段列表增加时,添加纪录到字典
            {
                try
                {
                    PropertyInfo pi = reference.GetType().GetProperty(item);
                    object value = pi.GetValue(reference, null);
                    Variables.Add(item, value);
                }
                catch (Exception)
                {

                }
            }
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 1000
            };
            timer.Elapsed += (s, e) =>
            {
                Save();
                return;
            };
        }
        /// <summary>
        /// 保存至XML文件
        /// </summary>
        private void Save()
        {
            Console.WriteLine($"save {DateTime.Now}");
            for (int i = 0; i < Variables.Count; i++)
            {
                PropertyInfo pi = reference.GetType().GetProperty(Variables.ElementAt(i).Key);
                object value = pi.GetValue(reference, null);
                Variables[Variables.ElementAt(i).Key] = value;
            }
            using (FileStream fs = new FileStream(file_XML, FileMode.Create, FileAccess.Write))
            {
                //在进行XML序列化的时候，在类中一定要有无参数的构造方法(要使用typeof获得对象类型)
                XmlSerializer xml = new XmlSerializer(typeof(SerializableDictionary<string, object>));
                xml.Serialize(fs, Variables);
            }
        }
        /// <summary>
        /// 从XML文件读取
        /// </summary>
        private void Load()
        {

            Variables = new SerializableDictionary<string, object>();
            foreach (var item in lstVarName)
            {
                PropertyInfo pi = reference.GetType().GetProperty(item);
                object value = pi.GetValue(reference, null);
                Variables.Add(item, value);
                SerializableDictionary<string, object>.CustomTypes.Add(value.GetType());
            }
            if (File.Exists(file_XML))
            {
                SerializableDictionary<string, object> xmlDictionary = null;
                using (FileStream fs = new FileStream(file_XML, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(SerializableDictionary<string, object>));
                    xmlDictionary = (SerializableDictionary<string, object>)xml.Deserialize(fs);
                }
                foreach (var item in xmlDictionary)
                {
                    try
                    {
                        Variables[item.Key] = item.Value;
                    }
                    catch (Exception)
                    {

                        
                    }
                    //if (lstVarName.Contains(item.Key))
                    //{
                    //    Variables.Add(item.Key,item.va);
                    //}
                }
                foreach (var item in Variables)
                {
                    Type type = reference.GetType();
                    PropertyInfo pi = type.GetProperty(item.Key);
                    object value = pi.GetValue(reference, null);
                    pi.SetValue(reference, item.Value);
                }
            }


            //for (int i = Variables.Count - 1; i >= 0; i--)//字段列表减少时,移除字典纪录
            //{
            //    if (!lstVarName.Contains(Variables.ElementAt(i).Key))
            //    {
            //        Variables.Remove(Variables.ElementAt(i).Key);
            //    }
            //}

        }

    }
    /// <summary>
    /// 序列化的字典,支持集合序列化
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        /// 传入自定义类型的type
        /// </summary>
        public static List<Type> CustomTypes { get; set; } = new List<Type>();
        public SerializableDictionary() { }
        public void WriteXml(XmlWriter write)
        {
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                write.WriteStartElement("SerializableDictionary");
                write.WriteStartElement("key");
                XmlSerializer KeySerializer = new XmlSerializer(kv.Key.GetType());
                write.WriteStartAttribute("type");
                write.WriteValue(kv.Key.GetType().FullName);
                KeySerializer.Serialize(write, kv.Key);
                write.WriteEndElement();
                write.WriteStartElement("value");
                XmlSerializer ValueSerializer = new XmlSerializer(kv.Value.GetType());
                write.WriteStartAttribute("type");
                write.WriteValue(kv.Value.GetType().ToString());
                ValueSerializer.Serialize(write, kv.Value);
                write.WriteEndElement();
                write.WriteEndElement();
            }
        }
        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("SerializableDictionary");
                string str_type_key = reader.GetAttribute(0);
                var type_key = Type.GetType(str_type_key, true);
                #region ReadKey
                reader.ReadStartElement("key");
                XmlSerializer KeySerializer = new XmlSerializer(type_key);
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                #endregion
                string str_type_value = reader.GetAttribute(0);
                Type type_value = SearchType(str_type_value);
                //if (type_value == null)//试图拯救一下
                //{
                //    if (str_type_value.Contains("System.Collections.Generic.List`1"))
                //    {
                //        string inner_str = str_type_value.Substring(34, str_type_value.Length - 35);
                //        type_value = SearchType(inner_str);
                //        type_value = typeof(List<>).MakeGenericType(type_value);
                //    }
                //}
                reader.ReadStartElement("value");
                XmlSerializer ValueSerializer = new XmlSerializer(type_value);
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// 在所有已经加载的程序集中根据字符串寻找类型
        /// </summary>
        /// <param name="str_type"></param>
        /// <returns></returns>
        private Type SearchType(string str_type)
        {
            Type type = null;
            foreach (var item in CustomTypes)
            {
                if (item.ToString() == str_type)
                {
                    return item;
                }
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    type = assembly.GetType(str_type, true);
                }
                catch (Exception)
                {

                }
            }
            return type;
        }
    }

}
