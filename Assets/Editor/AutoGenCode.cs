using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AutoGenCode
{
    //logic�����·��
    const string LogicDir = "Assets/AutoGen/Logic";
    //view�����·��
    const string ViewDir = "Assets/AutoGen/View";
    //logic��ģ���ļ�·��
    const string LogicTempletePath = "Assets/AutoGen/LogicTemplete.txt";
    //view��ģ���ļ�·��
    const string ViewTempletePath = "Assets/AutoGen/ViewTemplete.txt";
    //�����ռ�ģ��
    const string NameSpaceTemplete = "using 0;";
    //�ֶ�ģ��
    const string FieldTemplete = "public 0 1;\\t";
    //����ģ��
    const string MethodTemplete = "0 = gameObject.transform.Find(\\1\\).GetComponent<2>();\\t\\t";

    /// <summary>
    /// ���Ե���������б�
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
        //�ж��Ƿ���prefab
        if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.Regular)
        {
            Debug.LogWarning("ѡ��Ĳ���Ԥ���壬ѡ��Ķ���" + go.name);
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
        nameSpaceContent.AppendLine(NameSpaceTemplete.Replace("0", "UnityEngine"));//������UnityEngine�����ռ�

        string logicTempleteContent = File.ReadAllText(LogicTempletePath, Encoding.UTF8);
        string viewTempleteContent = File.ReadAllText(ViewTempletePath, Encoding.UTF8);
        string logicPath = LogicDir + "/" + go.name + "Logic.cs";
        string viewPath = ViewDir + "/" + go.name + "View.cs";

        List<string> tempNameSpaceList = new List<string>();

        //���������������������
        List<ComponentInfo> infoList = new List<ComponentInfo>();
        CalcComponentInfo("", go.transform, infoList);
        foreach (var tempInfo in infoList)
        {
            //�ֶ�
            string tempFieldStr = FieldTemplete.Replace("0", tempInfo.TypeStr);
            tempFieldStr = tempFieldStr.Replace("1", tempInfo.FieldName);
            fieldContent.AppendLine(tempFieldStr);
            //�󶨷���
            string tempMethodStr = MethodTemplete.Replace("0", tempInfo.FieldName);
            tempMethodStr = tempMethodStr.Replace("1", tempInfo.Path);
            tempMethodStr = tempMethodStr.Replace("2", tempInfo.TypeStr);
            methodContent.AppendLine(tempMethodStr);
            //�����ռ�
            if (!tempNameSpaceList.Contains(tempInfo.NameSpace))
            {
                string tempNameSpaceStr = NameSpaceTemplete.Replace("0", tempInfo.NameSpace);
                tempNameSpaceList.Add(tempInfo.NameSpace);
                nameSpaceContent.AppendLine(tempNameSpaceStr);
            }
        }

        //logic��ű�
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

        //view��ű�
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
    /// ���������������������
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
    /// �Ƿ�Ϊ�Ϸ����ֶ���
    /// </summary>
    static bool IsVaildField(string goName)
    {
        if (goName.Contains("_"))
        {
            if (int.TryParse(goName[0].ToString(), out _))
            {
                Debug.LogWarning("�ֶ������������ֿ�ͷ��, goName ��" + goName);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// �Ƿ�����ͬ���������
    /// </summary>
    static bool HaveSameComponentInfo(ComponentInfo info, List<ComponentInfo> infoList)
    {
        foreach (var tempInfo in infoList)
        {
            if (tempInfo.FieldName == info.FieldName)
            {
                Debug.LogWarning("���������ظ���, goName ��" + info.go.name);
                return true;
            }
        }
        return false;
    }
}


/// <summary>
/// �������
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

