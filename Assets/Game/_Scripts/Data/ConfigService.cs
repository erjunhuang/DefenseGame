using System;
using System.Reflection;
using QGame.Core.Device;
using GameModel;
using UnityEngine;
using LitJson;

namespace QGame.Core.Config
{
    public class ConfigService
    {
        public ConfigList<SkillCfg> SkillCfgList;

        public ConfigList<MapCfg> MapCfgList;
        public ConfigList<SpawnCfg> SpawnCfgList;
        public ConfigList<WaveCfg> WaveCfgList;
        public ConfigList<MonsterCfg> MonsterCfgList;

        public static readonly ConfigService Instance = new ConfigService();
        public void Initialize()
        {
            Type type = this.GetType();
            MethodInfo method = type.GetMethod("ReadConfig", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in typeof(ConfigService).GetFields())
            {
                if (field.FieldType.IsGenericType)
                {
                    Type[] ts = field.FieldType.GetGenericArguments();

                    if (ts != null && ts.Length == 1 && ts[0].BaseType == typeof(ConfigBase))
                    {
                        var m = method.MakeGenericMethod(ts);
                        field.SetValue(this, m.Invoke(this, null));
                    }
                }
            }
        }

        private ConfigList<T> ReadConfig<T>() where T : ConfigBase
        {
            Type type = typeof(T);
            var list = new ConfigList<T>();
             
            TextAsset monsterConfig = Resources.Load<TextAsset>("Config/"+ type.Name);
            foreach (string str in monsterConfig.text.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                try
                {
                    string str2 = str.Trim();
                    if (str2 == "")
                    {
                        continue;
                    }
                    //var item = Activator.CreateInstance<T>();
                    var item = JsonMapper.ToObject<T>(str2.Trim());
                    list.Add(item);
                }
                catch (Exception e)
                {
                    throw new Exception($"parser json fail: {str}", e);
                }
            }
            
            return list;
        }

        //private ConfigList<T> ReadConfig<T>() where T : ConfigBase
        //{
        //    Type type = typeof(T);

        //    String path = String.Format("{0}config/{1}.dat", DeviceFactory.Instance.DataPath, type.Name);

        //    //#if UNITY_EDITOR||UNITY_STANDALONE_WIN
        //    //            path = String.Format("{0}config/{1}.dat", "file://" + Application.streamingAssetsPath+"/", type.Name);//DeviceFactory.Instance.DataPath
        //    //#elif UNITY_ANDROID
        //    //            path =  String.Format("{0}config/{1}.dat", "jar:file://" + Application.dataPath + "!/assets/", type.Name);
        //    //#else
        //    //            Log.Debug("ReadConfig<T> path:null Config Name={0}", type.Name);
        //    //#endif
        //    return ConfigReader.Parse<T>(path);
        //}
    }
}

