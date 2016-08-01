#if UNITY_EDITOR

using System.Collections.Generic;

namespace OldFrame
{

    public enum OldType
    {
        SubsystemGraph,
        MVVMGraph,
        SceneTypeNode,
        ElementNode,
        ViewNode,
        TypeReferenceNode,
        PropertiesChildItem,
        CommandsChildItem,
        CollectionsChildItem,
        BindingsReference,
        ComputedPropertyNode,
        ServiceNode,
        SimpleClassNode,
        CommandNode,
        StateMachineNode,
        StateNode,
        EnumNode,
        EnumChildItem,
        TransitionsChildItem,
        StateTransitionsReference,
        HandlersReference
    }

    public class RootNode
    {
        public string _CLRType { get; set; }
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public string Identifier { get; set; }
        public List<Item> Items { get; set; }
        public Dictionary<string, bool> CollapsedValues { get; set; }
        //public Dictionary<string, bool> Flags { get; set; }
        //public Dictionary<string, string> DataBag { get; set; }
        public bool IsNewNode { get; set; }
    }

    public class Item
    {
        public string _CLRType { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool Precompiled { get; set; }
        //public Dictionary<string, bool> Flags { get; set; }
        //public Dictionary<string, string> DataBag { get; set; }
        public string ItemType { get; set; }
        public string BindingName { get; set; }
        public string SourceIdentifier { get; set; }
    }

    public class ScenePropertiesInputSlot
    {
        public string _CLRType { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool Precompiled { get; set; }
        //public Dictionary<string, bool> Flags { get; set; }
        //public Dictionary<string, string> DataBag { get; set; }
    }

    public class ElementInputSlot
    {
        public string _CLRType { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool Precompiled { get; set; }
        //public Dictionary<string, bool> Flags { get; set; }
        //public Dictionary<string, string> DataBag { get; set; }
    }
    
    public class StartStateOutputSlot
    {
        //public string _CLRType { get; set; }
        //public string Name { get; set; }
        public string Identifier { get; set; }
        //public bool Precompiled { get; set; }
        //public  Flags { get; set; }
        //public  DataBag { get; set; }
    }

    public class Node
    {
        public string _CLRType { get; set; }
        public string Name { get; set; }
        public bool IsCollapsed { get; set; }
        public string Identifier { get; set; }
        public List<Item> Items { get; set; }
        public Dictionary<string, bool> CollapsedValues { get; set; }
        //public Dictionary<string, bool> Flags { get; set; }
        //public Dictionary<string, string> DataBag { get; set; }
        public bool IsNewNode { get; set; }
        public ScenePropertiesInputSlot ScenePropertiesInputSlot { get; set; }
        public ElementInputSlot ElementInputSlot { get; set; }
        public StartStateOutputSlot StartStateOutputSlot { get; set; }
        public string PropertyType { get; set; }
    }

    public class ConnectedItem
    {
        public string _CLRType { get; set; }
        public string OutputIdentifier { get; set; }
        public string InputIdentifier { get; set; }
    }

    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class FilterState
    {
        public string _CLRType { get; set; }
        public List<string> FilterStack { get; set; }
    }

    public class Settings
    {
        public string _CLRType { get; set; }
        public bool CodeGenDisabled { get; set; }
        public int SnapSize { get; set; }
        public bool Snap { get; set; }
        public string CodePathStrategyName { get; set; }
        //public Dictionary<object, object> GridLinesColor { get; set; }
        //public Dictionary<object, object> GridLinesColorSecondary { get; set; }
        //public Dictionary<object, object> AssociationLinkColor { get; set; }
        //public Dictionary<object, object> DefinitionLinkColor { get; set; }
        //public Dictionary<object, object> InheritanceLinkColor { get; set; }
        //public Dictionary<object, object> SceneManagerLinkColor { get; set; }
        //public Dictionary<object, object> SubSystemLinkColor { get; set; }
        //public Dictionary<object, object> TransitionLinkColor { get; set; }
        //public Dictionary<object, object> ViewLinkColor { get; set; }
        public string RootNamespace { get; set; }
    }

    public class MainGraph
    {
        public string m_Name { get; set; }
        public string _projectNamespace { get; set; }
        public List<string> _currentTabs { get; set; }

        public MainGraph() { }

        public bool Parse(string[] fileLines)
        {
            _currentTabs = new List<string>();
            bool ok = false;

            foreach (string line in fileLines)
            {
                if (line.Contains("m_Name"))
                {
                    string data = line.Replace("m_Name:", string.Empty);
                    m_Name = data.Trim();
                }
                else if (line.Contains("_projectNamespace"))
                {
                    string data = line.Replace("_projectNamespace:", string.Empty);
                    _projectNamespace = data.Trim();
                    ok = true;
                }
                else if (line.Contains("_graphIdentifier"))
                {
                    string data = line.Replace("- _graphIdentifier:", string.Empty);
                    _currentTabs.Add(data.Trim());
                }
            }

            if (ok) return true;
            return false;
        }
    }

    public class MyGraph
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public bool DocumentationMode { get; set; }
        public RootNode RootNode { get; set; }
        public List<Node> Nodes { get; set; }
        public List<ConnectedItem> ConnectedItems { get; set; }
        public Dictionary<string, object> PositionData { get; set; }
        public FilterState FilterState { get; set; }
        public Settings Settings { get; set; }
        public List<object> Changes { get; set; }
    }

}

#endif