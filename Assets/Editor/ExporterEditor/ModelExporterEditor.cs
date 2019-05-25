using GameModel;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class ModelExporterEditor : EditorWindow
{
    public static Dictionary<long, ResourcesPath> resourcesPathInfos = new Dictionary<long, ResourcesPath>();//所有可以升级的目标信息
    protected ResourcesPath jsonData;
    [MenuItem("Examples/创建模型")]
    static void CreateModelWithTemplate()
    {
        resourcesPathInfos.Clear();
        LoadData();
        CreatePrefabWithData();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void LoadData() {
         
        TextAsset resourcesPathConfig = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Config/ResourcesPath.txt");
        foreach (string str in resourcesPathConfig.text.Split(new[] { "\n" }, StringSplitOptions.None))
        {
            try
            {
                string str2 = str.Trim();
                if (str2 == "")
                {
                    continue;
                }
                ResourcesPath jsonData = JsonMapper.ToObject<ResourcesPath>(str2.Trim());
                resourcesPathInfos.Add(jsonData.Id,jsonData);
            }
            catch (Exception e)
            {
                throw new Exception($"parser json fail: {str}", e);
            }
        }
    }
    static void CreatePrefabWithData()
    {
        foreach (ResourcesPath monsterResData in resourcesPathInfos.Values) {

            string localPath = "Assets/Resources/Prefab/Monster/" + monsterResData.Id + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                //if (EditorUtility.DisplayDialog("Are you sure?",
                //    "The Prefab" + monsterResData.Id + " already exists. Do you want to overwrite it?",
                //    "Yes",
                //    "No"))
                //{
                    //CreateNew(go, localPath, jsonData);
                    CreatePrefab(localPath, monsterResData);
                //}
            }
            else
            {
                Debug.Log(monsterResData.Id + " is not a Prefab, will convert");
                CreatePrefab(localPath, monsterResData);
            }
             
        }
    }

    static void CreatePrefab(string localPath, ResourcesPath monsterResData)
    {
        string towerTemplatePath = "Assets/Resources/CopyModel/Tower.prefab";
        string agentTemplatePath = "Assets/Resources/CopyModel/Enemy.prefab";
        GameObject obj;
        GameObject monster = null;
        if (monsterResData.Id > 2000 && monsterResData.Id < 5000)
        {
            obj = AssetDatabase.LoadAssetAtPath<GameObject>(towerTemplatePath);
            //塔
            monster = CreateTower(obj, localPath, monsterResData);
        }
        else if (monsterResData.Id > 1000)
        {
            obj = AssetDatabase.LoadAssetAtPath<GameObject>(agentTemplatePath);
            //怪物
            monster = CreateAgent(obj, localPath, monsterResData);
        }
        else {

        }

        if (monster == null) return;

        Animator animator = monster.GetComponent<Animator>();
        RuntimeAnimatorController runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>
            ("Assets/Resources/Model/" + monsterResData.Id + "/Animation/Controller.controller");

        if (runtimeAnimatorController != null)
        {
            animator.runtimeAnimatorController = runtimeAnimatorController;
        }
    }

    static GameObject CreateTower(GameObject obj, string localPath, ResourcesPath monsterResData) {
        UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        GameObject temp = PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);

        SpriteRenderer bodySpriteRenderer = temp.transform.Find("Body").GetComponent<SpriteRenderer>();

        string spritePath = monsterResData.MaterialResFlieName;
        UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);

        foreach (UnityEngine.Object sprite in sprites)
        {
            if (sprite.name == monsterResData.MaterialResName)
            {
                bodySpriteRenderer.sprite = sprite as Sprite;
            }
        }

        return temp;
    }

    static GameObject CreateAgent(GameObject obj, string localPath, ResourcesPath monsterResData)
    {
        UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        GameObject temp = PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);

        SpriteRenderer bodySpriteRenderer = temp.transform.Find("Body").GetComponent<SpriteRenderer>();
        SpriteRenderer shadowSpriteRenderer = temp.transform.Find("Shadow").GetComponent<SpriteRenderer>();

        string spritePath = monsterResData.MaterialResFlieName;
        UnityEngine.Object [] sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);

        foreach (UnityEngine.Object sprite in sprites) {
            if (sprite.name == monsterResData.MaterialResName) {
                bodySpriteRenderer.sprite = sprite as Sprite;
            }
            if (sprite.name == monsterResData.ShadowName)
            {
                shadowSpriteRenderer.sprite = sprite as Sprite;
            }
        }

        return temp;
    }
}
