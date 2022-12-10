using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AutoGenCode
{
    //logic层代码路径
    const string LogicDir = "Assets/AutoGen/Logic";
    //view层代码路径
    const string ViewDir = "Assets/AutoGen/View";
    //logic层模版文件路径
    const string LogicTempletePath = "Assets/AutoGen/LogicTemplete.txt";
    //view层模版文件路径
    const string ViewTempletePath = "Assets/AutoGen/ViewTemplete.txt";
    //命名空间模板
    const string NameSpaceTemplete = "using 0;";
    //字段模板
    const string FieldTemplete = "public 0 1;\\t";
    //方法模板
    const string MethodTemplete = "0 = gameObject.transform.Find(\\1\\).GetComponent<2>();\\t\\t";

    /// <summary>
    /// 忽略的组件类型列表
    /// </summary>
    static List<Type> IgnoreComponentTypeList = new List<Type>()
    {
          typeof(CanvasRenderer),
        typeof(RectTransform),
    };

    [MenuItem("Assets/AutoGen/Create View", priority = 0)]
    static void CreateLogicAndView()
    {
        GameObject go = Selection.activeGameObject;
        //判断是否是prefab
        if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.Regular)
        {
            Debug.LogWarning("选择的不是预制体，选择的对象：" + go.name);
            return;
        }
        if (!Directory.Exists(ViewDir))

            Directory.CreateDirectory(ViewDir);
        if (!Directory.Exists(LogicDir))

            Directory.CreateDirectory(LogicDir);

        string className = go.name + "View";
        StringBuilder fieldContent = new StringBuilder();
        StringBuilder methodContent = new StringBuilder();
        StringBuilder nameSpaceContent = new StringBuilder();
        nameSpaceContent.AppendLine(NameSpaceTemplete.Replace("0", "UnityEngine"));//必须有UnityEngine命名空间

        string logicTempleteContent = File.ReadAllText(LogicTempletePath, Encoding.UTF8);
        string viewTempleteContent = File.ReadAllText(ViewTempletePath, Encoding.UTF8);
        string logicPath = LogicDir + "/" + go.name + "Logic.cs";
        string viewPath = ViewDir + "/" + go.name + "View.cs";

        List<string> tempNameSpaceList = new List<string>();

        //计算所有子物体组件数据
        List<ComponentInfo> infoList = new List<ComponentInfo>();
        CalcComponentInfo("", go.transform, infoList);
        foreach (var tempInfo in infoList)
        {
            //字段
            string tempFieldStr = FieldTemplete.Replace("0", tempInfo.TypeStr);
            tempFieldStr = tempFieldStr.Replace("1", tempInfo.FieldName);
            fieldContent.AppendLine(tempFieldStr);
            //绑定方法
            string tempMethodStr = MethodTemplete.Replace("0", tempInfo.FieldName);
            tempMethodStr = tempMethodStr.Replace("1", tempInfo.Path);
            tempMethodStr = tempMethodStr.Replace("2", tempInfo.TypeStr);
            methodContent.AppendLine(tempMethodStr);
            //命名空间
            if (!tempNameSpaceList.Contains(tempInfo.NameSpace))
            {
                string tempNameSpaceStr = NameSpaceTemplete.Replace("0", tempInfo.NameSpace);
                tempNameSpaceList.Add(tempInfo.NameSpace);
                nameSpaceContent.AppendLine(tempNameSpaceStr);
            }
        }

        //logic层脚本
        if (!File.Exists(logicPath))
        {
            using (StreamWriter sw = new StreamWriter(logicPath))
            {
                string content = logicTempleteContent;
                content = content.Replace("#CLASSNAME#", className);
                sw.Write(content);
                sw.Close();
            }
        }

        //view层脚本
        using (StreamWriter sw = new StreamWriter(viewPath))
        {
            string content = viewTempleteContent;
            content = content.Replace("#NAMESPACE#", nameSpaceContent.ToString());
            content = content.Replace("#CLASSNAME#", className);
            content = content.Replace("#FIELD_BIND#", fieldContent.ToString());
            content = content.Replace("#METHOD_BIND#", methodContent.ToString());
            sw.Write(content);
            sw.Close();
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 计算所有子物体组件数据
    /// </summary>
    static void CalcComponentInfo(string path, Transform child, List<ComponentInfo> infoList)
    {
        bool isRoot = string.IsNullOrEmpty(path);
        if (!isRoot && IsVaildField(child.name))
        {
            var componentList = child.GetComponents<Component>();
            foreach (var tempComponent in componentList)
            {
                ComponentInfo info = new ComponentInfo()
                {
                    Path = path,
                    go = child.gameObject,
                    NameSpace = tempComponent.GetType().Namespace,
                    TypeStr = tempComponent.GetType().Name,
                };
                if (!HaveSameComponentInfo(info, infoList) && !IgnoreComponentTypeList.Contains(tempComponent.GetType()))
                {
                    infoList.Add(info);
                }
            }
        }
        foreach (Transform tempTrans in child.transform)
        {
            CalcComponentInfo(isRoot ? tempTrans.name : path + "/" + tempTrans.name, tempTrans.transform, infoList);
        }
    }

    /// <summary>
    /// 是否为合法的字段名
    /// </summary>
    static bool IsVaildField(string goName)
    {
        if (goName.Contains("_"))
        {
            if (int.TryParse(goName[0].ToString(), out _))
            {
                Debug.LogWarning("字段名不能以数字开头：, goName ：" + goName);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否有相同的组件数据
    /// </summary>
    static bool HaveSameComponentInfo(ComponentInfo info, List<ComponentInfo> infoList)
    {
        foreach (var tempInfo in infoList)
        {
            if (tempInfo.FieldName == info.FieldName)
            {
                Debug.LogWarning("子物体名重复：, goName ：" + info.go.name);
                return true;
            }
        }
        return false;
    }
}


/// <summary>
/// 组件数据
/// </summary>
public class ComponentInfo
{

    public string Path;
    public GameObject go;
    public string NameSpace;
    public string TypeStr;
    public string FieldName
    {
        get { return $"go.name_TypeStr"; }
    }
}

