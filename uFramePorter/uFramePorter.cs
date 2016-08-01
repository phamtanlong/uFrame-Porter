#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using NewFrame;
using OldFrame;
using UnityEditor;
using Pathfinding.Serialization.JsonFx;

public class uFramePorter : MonoBehaviour
{
    public static string ProjectRootFolder;
    public static string ProjectName;
    public static string ProjectDbName;
    public static string ProjectDbPath
    {
        get { return Path.Combine(RootAssets.Replace("Assets",""), ProjectDbName); }
    }
    public static string WorkspaceName;
    public static string WorkspaceId;
    public static string DatabaseId;

    #region Data

    public static Dictionary<OldType, string> DictOldType = new Dictionary<OldType, string>()
    {
        {OldType.SubsystemGraph, "Invert.uFrame.MVVM.SubsystemGraph"},
        {OldType.MVVMGraph, "Invert.uFrame.MVVM.MVVMGraph"},
        {OldType.SceneTypeNode, "Invert.uFrame.MVVM.SceneTypeNode"},
        {OldType.ElementNode, "Invert.uFrame.MVVM.ElementNode"},
        {OldType.ViewNode, "Invert.uFrame.MVVM.ViewNode"},
        {OldType.TypeReferenceNode, "Invert.Core.GraphDesigner.TypeReferenceNode"},
        {OldType.PropertiesChildItem, "Invert.uFrame.MVVM.PropertiesChildItem"},
        {OldType.CommandsChildItem, "Invert.uFrame.MVVM.CommandsChildItem"},
        {OldType.CollectionsChildItem, "Invert.uFrame.MVVM.CollectionsChildItem"},
        {OldType.BindingsReference, "Invert.uFrame.MVVM.BindingsReference"},
        {OldType.ComputedPropertyNode, "Invert.uFrame.MVVM.ComputedPropertyNode"},
        {OldType.ServiceNode, "Invert.uFrame.MVVM.ServiceNode"},
        {OldType.SimpleClassNode, "Invert.uFrame.MVVM.SimpleClassNode"},
        {OldType.CommandNode, "Invert.uFrame.MVVM.CommandNode"},
        {OldType.StateMachineNode, "Invert.uFrame.MVVM.StateMachineNode"},
        {OldType.StateNode, "Invert.uFrame.MVVM.StateNode"},
        {OldType.EnumNode, "EnumNode"},
        {OldType.EnumChildItem, "EnumChildItem"},
        {OldType.TransitionsChildItem, "Invert.uFrame.MVVM.TransitionsChildItem"},
        {OldType.StateTransitionsReference, "Invert.uFrame.MVVM.StateTransitionsReference"},
        {OldType.HandlersReference, "Invert.uFrame.MVVM.HandlersReference"},
    };

    public static Dictionary<NewType, string> DictNewType = new Dictionary<NewType, string>()
    {
        {NewType.EnumChildItem, "uFrame.Editor.Compiling.CommonNodes.EnumChildItem"},
        {NewType.EnumNode, "uFrame.Editor.Compiling.CommonNodes.EnumNode"},
        {NewType.TypeReferenceNode, "uFrame.Editor.Compiling.CommonNodes.TypeReferenceNode"},
        {NewType.uFrameDatabaseConfig, "uFrame.Editor.Database.Data.uFrameDatabaseConfig"},
        {NewType.ConnectionData, "uFrame.Editor.Graphs.Data.ConnectionData"},
        {NewType.FilterItem, "uFrame.Editor.Graphs.Data.FilterItem"},
        {NewType.FilterStackItem, "uFrame.Editor.Graphs.Data.FilterStackItem"},
        {NewType.FlagItem, "uFrame.Editor.Graphs.Data.FlagItem"},
        {NewType.NavHistoryItem, "uFrame.Editor.NavigationSystem.NavHistoryItem"},
        {NewType.RedoItem, "uFrame.Editor.Undo.RedoItem"},
        {NewType.UndoItem, "uFrame.Editor.Undo.UndoItem"},
        {NewType.WorkspaceGraph, "uFrame.Editor.Workspaces.Data.WorkspaceGraph"},
        {NewType.BindingsReference, "uFrame.MVVM.BindingsReference"},
        {NewType.CollectionsChildItem, "uFrame.MVVM.CollectionsChildItem"},
        {NewType.CommandNode, "uFrame.MVVM.CommandNode"},
        {NewType.CommandsChildItem, "uFrame.MVVM.CommandsChildItem"},
        {NewType.ComputedPropertyNode, "uFrame.MVVM.ComputedPropertyNode"},
        {NewType.ElementNode, "uFrame.MVVM.ElementNode"},
        {NewType.HandlersReference, "uFrame.MVVM.HandlersReference"},
        {NewType.MVVMGraph, "uFrame.MVVM.MVVMGraph"},
        {NewType.MVVMNode, "uFrame.MVVM.MVVMNode"},
        {NewType.MvvmWorkspace, "uFrame.MVVM.MvvmWorkspace"},
        {NewType.PropertiesChildItem, "uFrame.MVVM.PropertiesChildItem"},
        {NewType.SceneTypeNode, "uFrame.MVVM.SceneTypeNode"},
        {NewType.ServiceNode, "uFrame.MVVM.ServiceNode"},
        {NewType.SimpleClassNode, "uFrame.MVVM.SimpleClassNode"},
        {NewType.StateMachineNode, "uFrame.MVVM.StateMachineNode"},
        {NewType.StateNode, "uFrame.MVVM.StateNode"},
        {NewType.StateTransitionsReference, "uFrame.MVVM.StateTransitionsReference"},
        {NewType.SubSystemGraph, "uFrame.MVVM.SubSystemGraph"},
        {NewType.SubSystemNode, "uFrame.MVVM.SubSystemNode"},
        {NewType.TransitionsChildItem, "uFrame.MVVM.TransitionsChildItem"},
        {NewType.ViewComponentNode, "uFrame.MVVM.ViewComponentNode"},
        {NewType.ViewNode, "uFrame.MVVM.ViewNode"},
        {NewType.InstancesReference, "uFrame.MVVM.InstancesReference"},

    };

    public static Dictionary<string, object> All = new Dictionary<string, object>();

    #endregion //Data

    [MenuItem("uFramePorter/Start Port")]
    public static void Run()
    {
        Debug.Log("<color=red>Start! ...</color>");
        //start

        Dictionary<FileInfo, DirectoryInfo> projects = GetAlluFrameProjects();

        foreach (KeyValuePair<FileInfo, DirectoryInfo> pair in projects)
        {
            string projectName = Path.GetFileNameWithoutExtension(pair.Key.Name);
            string projectRootFolder = pair.Value.Name;

            //old information
            ProjectRootFolder = projectRootFolder;
            ProjectName = projectName;
            ProjectDbName = projectName + ".db";
            //new information
            WorkspaceName = projectName;
            WorkspaceId = GetNewGuid();
            DatabaseId = GetNewGuid();

            #region Get old information

            All.Clear();

            List<MyGraph> listGraph = new List<MyGraph>();
            MainGraph mainGraph = null;

            List<FileInfo> listFiles = GetAllGraphFiles(pair.Key, pair.Value);
            foreach (FileInfo file in listFiles)
            {
                string json = GetJsonFromFileGraph(file.FullName);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        MyGraph graph = JsonReader.Deserialize<MyGraph>(json);
                        listGraph.Add(graph);
                        CountObjectInGraph(graph);
                    }
                    catch (JsonDeserializationException ejson)
                    {
                        Debug.Log("Plz change setting:");
                        Debug.Log("Edit > Project Setting > Editor > Assets Serialization > Force Text");
                        return;//yield break;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        Debug.Log(file.FullName);
                    }
                }
                else
                {
                    mainGraph = ParseMainGraph(file.FullName);
                }
            }

            #endregion //Get old information

            #region Parse to new 

            PrepareAllFolders();

            //Common
            ProcessMvvmWorkspace();

            if (mainGraph != null)
            {
                //Config
                ProcessuFrameDatabaseConfig(mainGraph);
            }

            foreach (MyGraph graph in listGraph)
            {
                ProcessNodes(graph);
                ProcessConnectionData(graph);
                ProcessPositionData(graph);
                ProcessWorkspaceGraph(graph);
                ProcessFilterStackItem(graph);
                ProcessInstancesReference(graph);

                if (graph.Type.Contains(DictOldType[OldType.MVVMGraph]))
                {
                    //MVVM Graph
                    ProcessMVVMGraph(graph);
                }
                else if (graph.Type.Contains(DictOldType[OldType.SubsystemGraph]))
                {
                    //Sub System Graph
                    ProcessSubSytem(graph);
                }
            }

            #endregion //Parse to new 

            //end project
            Debug.Log("<color=green>Finish Project: </color>" + ProjectName);
        }

        //end all
        Debug.Log("<color=blue>Finish All!</color>");
    }

    // Process Graph

    public static void ProcessMvvmWorkspace()
    {

        //MvvmWorkspace
        NewFrame.MvvmWorkspace workspace = new MvvmWorkspace();
        workspace.Identifier = WorkspaceId;
        workspace.Name = WorkspaceName;
        workspace.CurrentGraphId = string.Empty;
        workspace.Expanded = false;

        string folderMvvmWorkspace = Path.Combine(ProjectDbPath, DictNewType[NewType.MvvmWorkspace]);
        string pathMvvmWorkspace = Path.Combine(folderMvvmWorkspace, workspace.Identifier + ".json");

        File.WriteAllText(pathMvvmWorkspace, JsonWriter.Serialize(workspace));

    }

    public static void ProcessuFrameDatabaseConfig(MainGraph mainGraph)
    {
        //Config
        NewFrame.uFrameDatabaseConfig config = new uFrameDatabaseConfig();
        config.Identifier = DatabaseId;
        config.CodeOutputPath = "Assets/" + ProjectRootFolder;
        config.Namespace = mainGraph._projectNamespace;
        config.BuildNumber = 0;
        config.BuildVersion = 0;
        config.MinorVersion = 0;
        config.MajorVersion = 1;
        string folderConfig = Path.Combine(ProjectDbPath, DictNewType[NewType.uFrameDatabaseConfig]);
        string pathConfig = Path.Combine(folderConfig, config.Identifier + ".json");
        File.WriteAllText(pathConfig, JsonWriter.Serialize(config));
    }

    public static void ProcessSubSytem(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.SubSystemNode]);
        string pathNode = Path.Combine(folderNode, graph.RootNode.Identifier + ".json");
        string folderGraph = Path.Combine(ProjectDbPath, DictNewType[NewType.SubSystemGraph]);
        string pathGraph = Path.Combine(folderGraph, graph.Identifier + ".json");

        NewFrame.SubSystemNode subSystemNode = new SubSystemNode();
        subSystemNode.Expanded = false;
        subSystemNode.GraphId = graph.Identifier;
        subSystemNode.Identifier = graph.RootNode.Identifier;
        subSystemNode.Name = graph.Name;
        subSystemNode.Order = 0;

        //SubSystemNode
        File.WriteAllText(pathNode, JsonWriter.Serialize(subSystemNode));

        NewFrame.SubSystemGraph subSystemGraph = new SubSystemGraph();
        subSystemGraph.Expanded = false;
        subSystemGraph.Identifier = graph.Identifier;
        subSystemGraph.IsDirty = false;
        subSystemGraph.RootFilterId = graph.RootNode.Identifier;

        //SubSystemGraph
        File.WriteAllText(pathGraph, JsonWriter.Serialize(subSystemGraph));
    }

    public static void ProcessMVVMGraph(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.MVVMNode]);
        string pathNode = Path.Combine(folderNode, graph.RootNode.Identifier + ".json");
        string folderGraph = Path.Combine(ProjectDbPath, DictNewType[NewType.MVVMGraph]);
        string pathGraph = Path.Combine(folderGraph, graph.Identifier + ".json");

        NewFrame.MVVMNode mvvmNode = new MVVMNode();
        mvvmNode.Expanded = false;
        mvvmNode.GraphId = graph.Identifier;
        mvvmNode.Identifier = graph.RootNode.Identifier;
        mvvmNode.Name = graph.Name;
        mvvmNode.Order = 0;

        //MVVMNode
        File.WriteAllText(pathNode, JsonWriter.Serialize(mvvmNode));

        NewFrame.MVVMGraph mvvmGraph = new MVVMGraph();
        mvvmGraph.Expanded = false;
        mvvmGraph.Identifier = graph.Identifier;
        mvvmGraph.IsDirty = false;
        mvvmGraph.RootFilterId = graph.RootNode.Identifier;

        //MVVMGraph
        File.WriteAllText(pathGraph, JsonWriter.Serialize(mvvmGraph));
    }

    public static void ProcessWorkspaceGraph(MyGraph graph)
    {
        string GraphId = GetNewGuid();

        string folderWorkspace = Path.Combine(ProjectDbPath, DictNewType[NewType.WorkspaceGraph]);
        string pathWorkspace = Path.Combine(folderWorkspace, GraphId + ".json");

        NewFrame.WorkspaceGraph spaceGraph = new WorkspaceGraph();
        spaceGraph.WorkspaceId = WorkspaceId;
        spaceGraph.GraphId = graph.Identifier;
        spaceGraph.Identifier = GraphId;

        //WorkspaceGraph
        File.WriteAllText(pathWorkspace, JsonWriter.Serialize(spaceGraph));
    }

    public static void ProcessInstancesReference(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.InstancesReference]);

        for (int i = 0; i < graph.RootNode.Items.Count; ++i)
        {
            Item item = graph.RootNode.Items[i];

            NewFrame.InstancesReference refs = new InstancesReference();
            refs.NodeId = graph.RootNode.Identifier;
            refs.Order = i;
            refs.SourceIdentifier = item.SourceIdentifier;
            refs.Identifier = GetNewGuid();
            refs.Name = item.Name;
            
            //todo: empty => default name
            if (string.IsNullOrEmpty(refs.Name))
            {
                if (All.ContainsKey(refs.SourceIdentifier))
                {
                    object source = All[refs.SourceIdentifier];
                    if (source is Node)
                    {
                        Node sourceNode = source as Node;
                        refs.Name = sourceNode.Name;
                    }
                    else if (source is Item)
                    {
                        Item sourceItem = source as Item;
                        refs.Name = sourceItem.Name;
                    }
                }
            }

            string pathNode = Path.Combine(folderNode, refs.Identifier + ".json");
            File.WriteAllText(pathNode, JsonWriter.Serialize(refs));
        }
    }

    // Process List

    public static void ProcessFilterStackItem(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.FilterStackItem]);
        List<string> ls = graph.FilterState.FilterStack;
        for (int i = 0; i < ls.Count; ++i)
        {
            NewFrame.FilterStackItem item = new FilterStackItem();
            item.GraphId = graph.Identifier;
            item.Index = i;
            item.FilterId = ls[i];
            item.Identifier = GetNewGuid();

            string pathNode = Path.Combine(folderNode, item.Identifier + ".json");
            File.WriteAllText(pathNode, JsonWriter.Serialize(item));
        }
    }

    public static void ProcessPositionData(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.FilterItem]);

        foreach (KeyValuePair<string, object> pair in graph.PositionData)
        {
            if (!(pair.Value is string))
            {
                Dictionary<string, object> dict = pair.Value as Dictionary<string, object>;
                foreach (KeyValuePair<string, object> point in dict)
                {
                    string identifire = GetNewGuid();
                    string filterId = pair.Key;
                    string nodeId = point.Key;
                    string pathNode = Path.Combine(folderNode, identifire + ".json");

                    NewFrame.FilterItem filterItem = new FilterItem();
                    Point position = JsonReader.Deserialize<Point>(JsonWriter.Serialize(point.Value));
                    filterItem.Position = new Position()
                    {
                        x = position.x,
                        y = position.y
                    };
                    filterItem.Identifier = identifire;
                    filterItem.NodeId = nodeId;
                    filterItem.FilterId = filterId;
                    filterItem.Collapsed = false;
                    //todo: FilterId

                    

                    File.WriteAllText(pathNode, JsonWriter.Serialize(filterItem));
                }
            }
        }
    }

    public static void ProcessConnectionData(MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.ConnectionData]);
        foreach (ConnectedItem item in graph.ConnectedItems)
        {
            string identifier = GetNewGuid();
            string pathNode = Path.Combine(folderNode, identifier + ".json");

            NewFrame.ConnectionData connection = new ConnectionData();
            connection.Identifier = identifier;
            connection.InputIdentifier = item.InputIdentifier;
            connection.OutputIdentifier = item.OutputIdentifier;

            File.WriteAllText(pathNode, JsonWriter.Serialize(connection));
        }
    }

    public static void ProcessNodes(MyGraph graph)
    {
        foreach (Node node in graph.Nodes)
        {
            if (node._CLRType.Contains(DictOldType[OldType.SceneTypeNode]))
            {
                ParseSceneTypeNode(node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.ElementNode]))
            {
                ParseElementNode(node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.ViewNode]))
            {
                ParseViewNode(node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.TypeReferenceNode]))
            {
                ParseTypeReferenceNode(node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.EnumNode]))
            {
                ParseEnumNode(node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.ComputedPropertyNode]))
            {
                ParseComputedPropertyNode( node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.ServiceNode]))
            {
                ParseServiceNode( node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.SimpleClassNode]))
            {
                ParseSimpleClassNode( node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.CommandNode]))
            {
                ParseCommandNode( node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.StateMachineNode]))
            {
                ParseStateMachineNode( node, graph);
            }
            else if (node._CLRType.Contains(DictOldType[OldType.StateNode]))
            {
                ParseStateNode( node, graph);
            }
            else
            {
                Debug.LogError("Node Miss: " + node._CLRType + " || " + node.Name);
            }


            ProcessItems(graph, node);
        }
    }

    public static void ProcessItems(MyGraph graph, Node node)
    {
        for (int i = 0; i < node.Items.Count; ++i)
        {
            Item item = node.Items[i];

            if (item._CLRType.Contains(DictOldType[OldType.PropertiesChildItem]))
            {
                ParsePropertiesChildItem(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.CommandsChildItem]))
            {
                ParseCommandsChildItem(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.CollectionsChildItem]))
            {
                ParseCollectionsChildItem(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.BindingsReference]))
            {
                ParseBindingsReference(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.EnumChildItem]))
            {
                ParseEnumChildItem(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.TransitionsChildItem]))
            {
                ParseTransitionsChildItem(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.StateTransitionsReference]))
            {
                ParseStateTransitionsReference(i, item, node, graph);
            }
            else if (item._CLRType.Contains(DictOldType[OldType.HandlersReference]))
            {
                ParseHandlersReference(i, item, node, graph);
            }
            else
            {
                Debug.LogError("Item Miss: " + item._CLRType + " || " + item.Name);
            }
        }
    }

    // Node

    static void ParseTypeReferenceNode(OldFrame.Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.TypeReferenceNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.TypeReferenceNode referenceNode = new TypeReferenceNode();
        referenceNode.Expanded = false;
        referenceNode.GraphId = graph.Identifier;
        referenceNode.Identifier = node.Identifier;
        referenceNode.Name = node.Name;
        referenceNode.Order = 0;

        File.WriteAllText(pathNode, JsonWriter.Serialize(referenceNode));
    }

    static void ParseViewNode(OldFrame.Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.ViewNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.ViewNode viewNode = new ViewNode();
        viewNode.Expanded = false;
        viewNode.GraphId = graph.Identifier;
        viewNode.Identifier = node.Identifier;
        viewNode.Name = node.Name;
        viewNode.Order = 0;
        viewNode.ElementInputSlotId = node.ElementInputSlot.Identifier;
        viewNode.ScenePropertiesInputSlotId = node.ScenePropertiesInputSlot.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(viewNode));
    }

    static void ParseElementNode(OldFrame.Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.ElementNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.ElementNode elementNode = new ElementNode();
        elementNode.Expanded = false;
        elementNode.GraphId = graph.Identifier;
        elementNode.Identifier = node.Identifier;
        elementNode.Name = node.Name;
        elementNode.Order = 0;

        File.WriteAllText(pathNode, JsonWriter.Serialize(elementNode));
    }

    static void ParseSceneTypeNode(OldFrame.Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.SceneTypeNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.SceneTypeNode sceneTypeNode = new SceneTypeNode();
        sceneTypeNode.Expanded = false;
        sceneTypeNode.GraphId = graph.Identifier;
        sceneTypeNode.Identifier = node.Identifier;
        sceneTypeNode.Name = node.Name;
        sceneTypeNode.Order = 0;

        File.WriteAllText(pathNode, JsonWriter.Serialize(sceneTypeNode));
    }

    static void ParseEnumNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.EnumNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        //Enum Node
        NewFrame.EnumNode enu = new EnumNode();
        enu.Identifier = node.Identifier;
        enu.Name = node.Name;
        enu.Order = 0;
        enu.Expanded = false;
        enu.GraphId = graph.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(enu));
    }

    static void ParseComputedPropertyNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.ComputedPropertyNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.ComputedPropertyNode computed = new ComputedPropertyNode();
        computed.Identifier = node.Identifier;
        computed.Name = node.Name;
        computed.Order = 0;
        computed.Expanded = false;
        computed.GraphId = graph.Identifier;
        computed.PropertyType = node.PropertyType;

        File.WriteAllText(pathNode, JsonWriter.Serialize(computed));

    }

    static void ParseServiceNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.ServiceNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.ServiceNode service = new ServiceNode();
        service.Identifier = node.Identifier;
        service.Name = node.Name;
        service.Order = 0;
        service.Expanded = false;
        service.GraphId = graph.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(service));

    }

    static void ParseSimpleClassNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.SimpleClassNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.SimpleClassNode simple = new SimpleClassNode();
        simple.Identifier = node.Identifier;
        simple.Name = node.Name;
        simple.Order = 0;
        simple.Expanded = false;
        simple.GraphId = graph.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(simple));

    }

    static void ParseCommandNode(Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.CommandNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.CommandNode command = new CommandNode();
        command.Identifier = node.Identifier;
        command.Name = node.Name;
        command.Order = 0;
        command.Expanded = false;
        command.GraphId = graph.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(command));

    }

    static void ParseStateMachineNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.StateMachineNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.StateMachineNode machine = new StateMachineNode();
        machine.Identifier = node.Identifier;
        machine.Name = node.Name;
        machine.Order = 0;
        machine.Expanded = false;
        machine.GraphId = graph.Identifier;
        machine.StartStateOutputSlotId = node.StartStateOutputSlot.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(machine));

    }

    static void ParseStateNode(Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.StateNode]);
        string pathNode = Path.Combine(folderNode, node.Identifier + ".json");

        NewFrame.StateNode state = new StateNode();
        state.Identifier = node.Identifier;
        state.Name = node.Name;
        state.Order = 0;
        state.Expanded = false;
        state.GraphId = graph.Identifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(state));

    }

    // Item

    static void ParseBindingsReference(int order, Item item, Node node, MyGraph graph)
    {
        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.BindingsReference]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.BindingsReference binding = new BindingsReference();
        binding.Identifier = item.Identifier;
        binding.NodeId = node.Identifier;
        binding.Order = order;
        binding.BindingName = item.BindingName;
        binding.SourceIdentifier = item.SourceIdentifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(binding));

    }

    static void ParseCollectionsChildItem(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.CollectionsChildItem]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.CollectionsChildItem collectionChild = new CollectionsChildItem();
        collectionChild.Identifier = item.Identifier;
        collectionChild.Name = item.Name;
        collectionChild.NodeId = node.Identifier;
        collectionChild.Order = order;
        collectionChild.RelatedType = item.ItemType;
        if (string.IsNullOrEmpty(item.ItemType))
        {
            collectionChild.RelatedType = "String";
        }

        File.WriteAllText(pathNode, JsonWriter.Serialize(collectionChild));

    }

    static void ParseCommandsChildItem(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.CommandsChildItem]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.CommandsChildItem commandChild = new CommandsChildItem();
        commandChild.Identifier = item.Identifier;
        commandChild.Name = item.Name;
        commandChild.NodeId = node.Identifier;
        commandChild.Order = order;
        commandChild.RelatedType = item.ItemType;

        File.WriteAllText(pathNode, JsonWriter.Serialize(commandChild));

    }

    static void ParsePropertiesChildItem(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.PropertiesChildItem]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.PropertiesChildItem childItem = new PropertiesChildItem();
        childItem.Identifier = item.Identifier;
        childItem.Name = item.Name;
        childItem.NodeId = node.Identifier;
        childItem.Order = order;
        childItem.RelatedType = item.ItemType;
        if (string.IsNullOrEmpty(item.ItemType))
        {
            childItem.RelatedType = "String";
        }

        File.WriteAllText(pathNode, JsonWriter.Serialize(childItem));

    }

    static void ParseEnumChildItem(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.EnumChildItem]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.EnumChildItem childItem = new EnumChildItem();
        childItem.Identifier = item.Identifier;
        childItem.Name = item.Name;
        childItem.NodeId = node.Identifier;
        childItem.Order = order;

        File.WriteAllText(pathNode, JsonWriter.Serialize(childItem));

    }

    static void ParseHandlersReference(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.HandlersReference]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.HandlersReference handler = new HandlersReference();
        handler.Identifier = item.Identifier;
        handler.NodeId = node.Identifier;
        handler.Order = order;
        handler.SourceIdentifier = item.SourceIdentifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(handler));

    }

    static void ParseStateTransitionsReference(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.StateTransitionsReference]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.StateTransitionsReference trans = new StateTransitionsReference();
        trans.Identifier = item.Identifier;
        trans.NodeId = node.Identifier;
        trans.Order = order;
        trans.SourceIdentifier = item.SourceIdentifier;

        File.WriteAllText(pathNode, JsonWriter.Serialize(trans));

    }

    static void ParseTransitionsChildItem(int order, Item item, Node node, MyGraph graph)
    {

        string folderNode = Path.Combine(ProjectDbPath, DictNewType[NewType.TransitionsChildItem]);
        string pathNode = Path.Combine(folderNode, item.Identifier + ".json");

        NewFrame.TransitionsChildItem trans = new TransitionsChildItem();
        trans.Identifier = item.Identifier;
        trans.NodeId = node.Identifier;
        trans.Order = order;
        trans.Name = item.Name;

        File.WriteAllText(pathNode, JsonWriter.Serialize(trans));

    }
    

    // Extension

    public static void CountObjectInGraph(MyGraph graph)
    {
        All.Add(graph.Identifier, graph);

        RootNode root = graph.RootNode;
        All.Add(root.Identifier, root);
        foreach (Item item in root.Items)
        {
            All.Add(item.Identifier, item);
        }

        foreach (Node node in graph.Nodes)
        {
            All.Add(node.Identifier, node);
            foreach (Item item in node.Items)
            {
                All.Add(item.Identifier, item);
            }
        }
    }

    public static MainGraph ParseMainGraph(string filePath)
    {
        MainGraph mainGraph = new MainGraph();
        try
        {
            string[] ss = File.ReadAllLines(filePath);

            if (mainGraph.Parse(ss))
                return mainGraph;
            else
                return null;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log(filePath);
            return null;
        }
        return mainGraph;
    }

    public static string GetJsonFromFileGraph(string path)
    {
        string endPattern = "'";
        string startPattern = "_jsonData: '";

        string[] ss = File.ReadAllLines(path);
        string jsonData = string.Empty;
        bool isGetJson = false;
        foreach (string line in ss)
        {
            if (isGetJson == false && line.Contains("_jsonData"))
            {
                isGetJson = true;
            }

            if (isGetJson)
            {
                jsonData += line.Trim();
            }
        }

        if (jsonData.EndsWith(endPattern))
        {
            jsonData = jsonData.Remove(jsonData.Length - 1, endPattern.Length);
        }

        if (jsonData.StartsWith(startPattern))
        {
            jsonData = jsonData.Remove(0, startPattern.Length);
        }

        return jsonData.Trim();
    }

    /// <summary>
    /// Get all uFrame project in solution
    /// </summary>
    /// <returns>Project File, Root Folder</returns>
    public static Dictionary<FileInfo,DirectoryInfo> GetAlluFrameProjects()
    {
        Dictionary<FileInfo, DirectoryInfo> listName = new Dictionary<FileInfo, DirectoryInfo>();

        string[] dirs = Directory.GetDirectories(RootAssets);
        foreach (string dir in dirs)
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension.Equals(".asset"))
                {
                    MainGraph mainGraph = ParseMainGraph(file);
                    if (mainGraph != null)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dir);
                        listName.Add(fileInfo, dirInfo);
                    }
                }
            }
        }

        return listName;
    } 

    public static List<FileInfo> GetAllGraphFiles(FileInfo projectFile, DirectoryInfo projectFolder)
    {
        FileInfo[] filePaths = projectFolder.GetFiles();
        string s = "";
        List<FileInfo> files = new List<FileInfo>();
        foreach (FileInfo fileInfo in filePaths)
        {
            if (fileInfo.Extension.Equals(".asset"))
            {
                files.Add(fileInfo);
                s += fileInfo.FullName + "\n";
            }
        }

        return files;
    }

    public static void CreateFolder(string path)
    {
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void PrepareAllFolders()
    {
        foreach (KeyValuePair<NewType, string> pair in DictNewType)
        {
            string path = Path.Combine(ProjectDbPath, pair.Value);
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                Directory.Delete(path);
            }
            CreateFolder(path);
        }
    }

    public static string GetNewGuid()
    {
        return System.Guid.NewGuid().ToString();
    }

    public static string RootAssets
    {
        get
        {
            return Application.dataPath;
        }
    }


}
#endif