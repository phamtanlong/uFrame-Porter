#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using OldFrame;
using UnityEditor;
using Pathfinding.Serialization.JsonFx;
using UniRx;

public class CodeOrganizer : MonoBehaviour
{
    #region Const

    public const string END = "\n";
    public const string BEGIN_CLASS = "public class ";
    public const string BEGIN_CLASS2 = "public partial class ";
    public const string BEGIN_ENUM = "public enum ";

    #endregion //Const

    public class FileTrunks
    {
        public Trunk import { get; set; }
        public List<Trunk> allclass { get; set; }
    }

    public enum TrunkType
    {
        Import,
        Class,
        Enum,
    }

    public class Trunk
    {
        public TrunkType type;
        public string name;
        public string code;

        public override string ToString()
        {
            return type + END + name + END + code;
        }
    }



    [MenuItem("uFramePorter/Organize Code")]
    public static void Run()
    {
        Dictionary<FileInfo, DirectoryInfo> projects = uFramePorter.GetAlluFrameProjects();

        foreach (KeyValuePair<FileInfo, DirectoryInfo> pair in projects)
        {

            string projectName = Path.GetFileNameWithoutExtension(pair.Key.Name);
            string projectRootFolder = pair.Value.Name;

            List<MyGraph> listGraph = new List<MyGraph>();
            MainGraph mainGraph = null;

            //read list

            List<FileInfo> listFiles = uFramePorter.GetAllGraphFiles(pair.Key, pair.Value);
            foreach (FileInfo file in listFiles)
            {
                string json = uFramePorter.GetJsonFromFileGraph(file.FullName);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        MyGraph graph = JsonReader.Deserialize<MyGraph>(json);
                        listGraph.Add(graph);
                        OrganizeGraph(graph, projectName, projectRootFolder);
                    }
                    catch (JsonDeserializationException ejson)
                    {
                        Debug.Log("Plz change setting:");
                        Debug.Log("Edit > Project Setting > Editor > Assets Serialization > Force Text");
                        return;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        Debug.Log(file.FullName);
                    }
                }
                else
                {
                    mainGraph = uFramePorter.ParseMainGraph(file.FullName);
                }
            }
        }

        AssetDatabase.Refresh();
    }



    //Organize

    public static void OrganizeGraph(MyGraph graph, string projectName, string projectRootFolder)
    {
        string rootFolder = Path.Combine(Application.dataPath, projectRootFolder);
        string subFolder = Path.Combine(rootFolder, graph.Name);

        //Commands.designer.cs
        OrganizeCommands(graph, subFolder);

        //Controllers.designer.cs
        OrganizeControllers(graph, subFolder);

        //Enums.designer.cs
        OrganizeEnums(graph, subFolder, rootFolder);

        //SystemLoaders.designer.cs
        OrganizeSystemLoaders(graph, subFolder);

        //ViewModels.designer.cs
        OrganizeViewModels(graph, subFolder);

        //Views.designer.cs
        OrganizeViews(graph, subFolder);

        //MVVM Graph Only

        //SceneLoaders
        OrganizeSceneLoaders(graph, subFolder);

    }

    // Items

    public static void OrganizeSceneLoaders(MyGraph graph, string subsystemFolder)
    {
        //SceneLoaders

        string folderScenes = Path.Combine(subsystemFolder, "Scenes");
        if (Directory.Exists(folderScenes))
        {
            string destFolderSceneLoader = Path.Combine(subsystemFolder, "SceneLoaders");
            uFramePorter.CreateFolder(destFolderSceneLoader);

            string[] files = Directory.GetFiles(folderScenes);
            foreach (string file in files)
            {
                if (file.EndsWith("Loader.cs"))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string destFile = Path.Combine(destFolderSceneLoader, fileInfo.Name);
                    File.Move(file, destFile);
                }
            }
        }

        //SceneSettings

        string folderSettings = Path.Combine(subsystemFolder, "ScenesSettings");
        if (Directory.Exists(folderSettings))
        {
            string[] ss = Directory.GetFiles(folderSettings);
            foreach (string s in ss)
            {
                FileUtil.CopyFileOrDirectory(s, s.Replace("Settings.cs", "Setting.cs"));
                FileUtil.DeleteFileOrDirectory(s);
            }
        }

        string fileScenesSettings = Path.Combine(subsystemFolder, "ScenesSettings.designer.cs");
        if (File.Exists(fileScenesSettings))
        {
            FileUtil.CopyFileOrDirectory(fileScenesSettings, fileScenesSettings.Replace("ScenesSettings.designer.cs", "SceneSettings.designer.cs"));
            FileUtil.DeleteFileOrDirectory(fileScenesSettings);
        }


        //Scene 

        string fileScenesDesigner = Path.Combine(subsystemFolder, "Scenes.designer.cs");
        if (File.Exists(fileScenesDesigner))
        {
            string destSceneLoaderDesigner = "SceneLoader.designer.cs";

            FileTrunks data = AnalyticFile(fileScenesDesigner);
            string lastCode = string.Empty;
            lastCode += data.import.code;

            if (data.allclass.Count > 0)
            {
                foreach (Trunk trunk in data.allclass)
                {
                    if (trunk.name.EndsWith("LoaderBase"))
                    {
                        CreateFileOrApppend(subsystemFolder, destSceneLoaderDesigner, trunk, data.import);
                    }
                    else
                    {
                        lastCode += trunk.code;
                    }
                }
            }

            File.WriteAllText(fileScenesDesigner, lastCode);
        }
    }

    public static void OrganizeViews(MyGraph graph, string subsystemFolder)
    {
        string file = Path.Combine(subsystemFolder, "Views.designer.cs");
        string destFolder = Path.Combine(subsystemFolder, "Views.designer");
        if (!File.Exists(file)) return;

        FileTrunks data = AnalyticFile(file);

        if (data.allclass.Count == 0) return;

        foreach (Trunk trunk in data.allclass)
        {
            string filename = trunk.name.Replace("Base", string.Empty) + ".designer.cs";
            CreateFileOrApppend(destFolder, filename, trunk, data.import);
        }

        //delete source file
        FileUtil.DeleteFileOrDirectory(file);
    }

    public static void OrganizeViewModels(MyGraph graph, string subsystemFolder)
    {
        string file = Path.Combine(subsystemFolder, "ViewModels.designer.cs");
        string destFolder = Path.Combine(subsystemFolder, "ViewModels.designer");
        if (!File.Exists(file)) return;

        FileTrunks data = AnalyticFile(file);

        if (data.allclass.Count == 0) return;

        foreach (Trunk trunk in data.allclass)
        {
            string filename = trunk.name.Replace("Base", string.Empty) + ".designer.cs";
            CreateFileOrApppend(destFolder, filename, trunk, data.import);
        }

        //delete source file
        FileUtil.DeleteFileOrDirectory(file);
    }

    public static void OrganizeSystemLoaders(MyGraph graph, string subsystemFolder)
    {
        string file = Path.Combine(subsystemFolder, "SystemLoaders.designer.cs");
        string destFolder = Path.Combine(subsystemFolder, "SystemLoaders.designer");
        if (!File.Exists(file)) return;

        uFramePorter.CreateFolder(destFolder);

        DirectoryInfo dir1 = new DirectoryInfo(subsystemFolder);
        string destFile = Path.Combine(destFolder, dir1.Name + "Loader.designer.cs");
        
        //delete source file
        FileUtil.MoveFileOrDirectory(file, destFile);

        //Rename folder

        string systemLoader = Path.Combine(subsystemFolder, "SystemLoaders");
        if (Directory.Exists(systemLoader))
        {
            DirectoryInfo dir2 = new DirectoryInfo(systemLoader);
            string dest = dir2.FullName.Replace("SystemLoaders", "SystemsLoaders");
            dir2.MoveTo(dest);
        }
    }

    public static void OrganizeEnums(MyGraph graph, string subsystemFolder, string footFolder)
    {

        string file = Path.Combine(subsystemFolder, "Enums.designer.cs");
        string destFolder = Path.Combine(footFolder, "Enums");
        if (!File.Exists(file)) return;

        FileTrunks data = AnalyticFile(file);

        if (data.allclass.Count == 0) return;

        foreach (Trunk trunk in data.allclass)
        {
            string filename = trunk.name + ".cs";
            CreateFileOrApppend(destFolder, filename, trunk, data.import);
        }

        //delete source file
        FileUtil.DeleteFileOrDirectory(file);
    }

    public static void OrganizeControllers(MyGraph graph, string subsystemFolder)
    {
        string file = Path.Combine(subsystemFolder, "Controllers.designer.cs");
        string destFolder = Path.Combine(subsystemFolder, "Controllers.designer");
        if (!File.Exists(file)) return;

        FileTrunks data = AnalyticFile(file);

        if (data.allclass.Count == 0) return;

        foreach (Trunk trunk in data.allclass)
        {
            string filename = trunk.name.Replace("Base", string.Empty) + ".designer.cs";
            CreateFileOrApppend(destFolder, filename, trunk, data.import);
        }

        //delete source file
        FileUtil.DeleteFileOrDirectory(file);
    }

    public static void OrganizeCommands(MyGraph graph, string subsystemFolder)
    {
        string srcFile = Path.Combine(subsystemFolder, "Commands.designer.cs");
        string destFolder = Path.Combine(subsystemFolder, "ViewModelCommands");
        if (!File.Exists(srcFile)) return;

        FileTrunks data = AnalyticFile(srcFile);
        
        if(data.allclass.Count == 0) return;

        foreach (Trunk trunk in data.allclass)
        {
            string filename = trunk.name + ".cs";
            CreateFileNameOnly(destFolder, filename, trunk, data.import);
        }

        string destFile = Path.Combine(subsystemFolder, "ViewModelCommands.designer.cs");
        //delete source file
        File.Move(srcFile, destFile);
    }

    // Extension

    public static FileTrunks AnalyticFile(string path)
    {
        string[] lines = File.ReadAllLines(path);

        Trunk import = new Trunk();
        List<Trunk> allTrunk = new List<Trunk>();
        string quote = string.Empty;
        string name = string.Empty;
        bool isUsing = true;
        for (int i = 0; i < lines.Length; ++i)
        {
            string line = lines[i];
            if (line.Contains(BEGIN_CLASS) || line.Contains(BEGIN_CLASS2) || line.Contains(BEGIN_ENUM) || i == lines.Length-1)
            {
                if (i == lines.Length - 1)
                {
                    quote += line;
                }

                if (isUsing)
                {
                    import = new Trunk()
                    {
                        type = TrunkType.Import,
                        name = name,
                        code = quote
                    };
                    isUsing = false;
                }
                else
                {
                    allTrunk.Add(new Trunk()
                    {
                        type = TrunkType.Class,
                        name = name,
                        code = quote
                    });
                }
                quote = string.Empty;

                //class name
                name = GetClassName(line);
            }
            quote += line + END;
        }

        FileTrunks fileTrunks = new FileTrunks()
        {
            import = import,
            allclass = allTrunk
        };
        return fileTrunks;
    }

    public static void CreateFileOrApppend(string folder, string filename, Trunk trunk, Trunk import)
    {
        uFramePorter.CreateFolder(folder);
        string file = Path.Combine(folder, filename);
        string allcode = import.code + trunk.code;

        if (File.Exists(file))
        {
            Debug.Log("<color=blue>Append</color>: " + file);
            File.AppendAllText(file, trunk.code);
        }
        else
        {
            File.WriteAllText(file, allcode);
        }
    }

    public static void CreateFileNameOnly(string folder, string filename, Trunk trunk, Trunk import)
    {
        uFramePorter.CreateFolder(folder);
        filename = Path.Combine(folder, filename);

        string allcode = string.Empty;
        allcode += import.code + "public partial class " + trunk.name + " {\n}\n\n";
        
        File.WriteAllText(filename, allcode);
    }

    public static string GetClassName(string line)
    {
        string s = line;
        if (s.Length < 4) return string.Empty;

        string replace = string.Empty;
        if (s.Contains(" class "))
        {
            replace = BEGIN_CLASS;
            if (s.Contains(BEGIN_CLASS2))
            {
                replace = BEGIN_CLASS2;
            }
        }
        else if (s.Contains(" enum "))
        {
            replace = BEGIN_ENUM;
        }
        
        s = s.Replace(replace, string.Empty);

        string[] ss = s.Split(' ');
        if (ss.Length > 0)
        {
            s = ss[0];
        }

        return s;
    }
}



#endif //UNITY_EDITOR