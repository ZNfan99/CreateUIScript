using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.IO;

#region UI命名规则
//UI名:以'_'分割，如:Btn_StartBtn_Panel,类型_名_父节点名
/*switch (选中的UI模块名首字符串)
        {
            case "Panel":
                return " Image ";
            case "Img":
                return " Image ";
            case "Btn":
                return " Button ";
            case "SV":
                return " ScrollRect ";
            case "Text":
                return " Text ";
            default:
                return null;
        }*/
#endregion

public class UIToCSWindow : EditorWindow
{
    StringBuilder content= new StringBuilder();

    RectTransform target;
    string fileName;
    string filepath = "Assets/Scripts";
    Dictionary<string, Component> dic = new Dictionary<string, Component>();
    [MenuItem("Tools/UIToCSWindow")]
    public static void Init()
    {
        UIToCSWindow window = EditorWindow.GetWindow<UIToCSWindow>("UIToCS");
        if (window!=null)
        {
            window.minSize = new Vector2(400,800);
            window.Show();
        }
    }
    private void OnEnable()
    {
        //添加命名空间
        content.AppendLine("using UnityEngine.Events;");
        content.AppendLine("using UnityEngine;");
        content.AppendLine("using UnityEngine.UI;");
    }
    private void OnGUI()
    {
        
        using(new EditorGUILayout.HorizontalScope())
        {
            //选择UI目标
            GUILayout.Label("选择目标UI:");
            target = EditorGUILayout.ObjectField(target, typeof(RectTransform), true) as RectTransform;
            if (target==null)
            {
                return;
            }
            if (target!=null||fileName!=target.name)
            {
                fileName = target.name;
            }
            
        }
        if (target!=null)
        {
            EditorGUILayout.LabelField("选择保存目标源文件夹");
            EditorGUILayout.TextField(filepath);
            if (GUILayout.Button("选择"))
            {
                filepath = EditorUtility.OpenFolderPanel("选择文件夹", filepath, "");
            }
        }
        

        if (GUILayout.Button("保存"))
        {
            List<StringBuilder> actionlist=new List<StringBuilder>();
            content.AppendLine("public class " + fileName+ ":MonoBehaviour");
            content.AppendLine("{");
            string strtype1 = target.name.Split('_')[0];
            content.AppendLine("\tpublic " + GetUIComponent(strtype1) + target.name+"_info" + ";");
            
            content.AppendLine("\tpublic string " + target.name + "_Path" + "=" + "\"" + GetTransPath(target) + "\"" + ";");
            for (int i = 0; i < target.childCount; i++)
            {
                string strtype = target.GetChild(i).name.Split('_')[0];
                content.AppendLine("\tpublic " + GetUIComponent(strtype) + target.GetChild(i).name+";");
                content.AppendLine("\tpublic string "  + target.GetChild(i).name+"_Path"+"="+"\"" + GetTransPath(target.GetChild(i)) + "\"" +";");
                actionlist.Add(AddSetAction(target.GetChild(i)));
            }
            AddAwake();
            AddStart();
            AddUpdate();
            for (int i = 0; i < actionlist.Count; i++)
            {
                content.Append(actionlist[i]);
            }
            content.AppendLine("}");
            File.WriteAllText(filepath+"/" + fileName + ".cs", content.ToString());
            Debug.Log("写入成功");
            AssetDatabase.Refresh();
        }

    }
    /// <summary>
    /// 添加Awake
    /// </summary>
    public void AddAwake()
    {
        content.AppendLine("\tvoid Awake()");
        content.AppendLine("\t{");

        content.AppendLine("\t}");
    }
    /// <summary>
    /// 添加Start
    /// </summary>
    public void AddStart()
    {
        content.AppendLine("\tvoid Start()");
        content.AppendLine("\t{");
        content.AppendLine("\t\t "+ target.name + "_info"+"="+ "GetComponent<"+GetUIComponent(target.name.Split('_')[0]) +">()" + ";");
        for (int i = 0; i < target.childCount; i++)
        {
            content.AppendLine("\t\t "+ target.GetChild(i).name +"="+ "transform.Find(" + target.GetChild(i).name + "_Path.Replace("+target.name+"_Path+\"/\""+",\"\")" + ").GetComponent<"+GetUIComponent(target.GetChild(i).name.Split('_')[0]) +">()" + ";");
        }
        content.AppendLine("\t}");
    }
    /// <summary>
    /// 添加Update
    /// </summary>
    public void AddUpdate()
    {
        content.AppendLine("\tvoid Update()");
        content.AppendLine("\t{");
        
        content.AppendLine("\t}");
    }

    public StringBuilder AddSetAction(Transform transform)
    {
        //添加对应的参数的刷新方法
        StringBuilder str= new StringBuilder();
        string s = transform.name.Split('_')[0];
        str.AppendLine("\tpublic void Set"+transform.name+"("+ GetParameter(s)+"content"+")");
        str.AppendLine("\t{");
        switch (s)
        {
            case "Panel":
                str.AppendLine("\t\t"+transform.name+ "_info.sprite=" +"content;");
                break;
            case "Img":
                str.AppendLine("\t\t"+transform.name + ".sprite=" + "content;");
                break;
            case "Btn":
                str.AppendLine("\t\t" + transform.name + ".onClick.AddListener(content);" );
                break;
            case "SV":
                str.AppendLine("\t\t");
                break;
            case "Text":
                str.AppendLine("\t\t" + transform.name + ".text=" + "content;");
                break;
            default:
                break;
        }
        

        str.AppendLine("\t}");
        return str;
    }
    public string GetParameter(string type)
    {
        //获取对应的参数,随命名规则的扩展而扩展
        switch (type)
        {
            case "Panel":
                return " Sprite ";
            case "Img":
                return " Sprite ";
            case "Btn":
                return " UnityAction ";
            case "SV":
                return " float ";
            case "Text":
                return " string ";
            default:
                return null;
        }
    }
    public static string GetTransPath(Transform trans)
    {
        if (!trans.parent)
        {
            return trans.name;

        }
        return GetTransPath(trans.parent) + "/" + trans.name;
    }
    public string GetUIComponent(string str)
    {
        //定义命名规则,自己扩展
        switch (str)
        {
            case "Panel":
                return " Image ";
            case "Img":
                return " Image ";
            case "Btn":
                return " Button ";
            case "SV":
                return " ScrollRect ";
            case "Text":
                return " Text ";
            default:
                return null;
        }
    }
}
