#if UNITY_EDITOR

namespace NewFrame
{

    public enum NewType
    {
        EnumChildItem,
        EnumNode,
        TypeReferenceNode,
        uFrameDatabaseConfig,
        ConnectionData,
        FilterItem,
        FilterStackItem,
        FlagItem,
        NavHistoryItem,
        RedoItem,
        UndoItem,
        WorkspaceGraph,
        BindingsReference,
        CollectionsChildItem,
        CommandNode,
        CommandsChildItem,
        ComputedPropertyNode,
        ElementNode,
        HandlersReference,
        MVVMGraph,
        MVVMNode,
        MvvmWorkspace,
        PropertiesChildItem,
        SceneTypeNode,
        ServiceNode,
        SimpleClassNode,
        StateMachineNode,
        StateNode,
        StateTransitionsReference,
        SubSystemGraph,
        SubSystemNode,
        TransitionsChildItem,
        ViewComponentNode,
        ViewNode,
        InstancesReference,
    }

    public class InstancesReference
    {
        public string Name { get; set; }
        public string SourceIdentifier { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
    }


    public class ViewNode
    {
        public string ElementInputSlotId { get; set; }
        public string ScenePropertiesInputSlotId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class SubSystemNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }


    public class SubSystemGraph
    {
        public bool IsDirty { get; set; }
        public string Identifier { get; set; }
        public string RootFilterId { get; set; }
        public bool Expanded { get; set; }
    }

    public class SceneTypeNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class PropertiesChildItem
    {
        public string RelatedType { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }

    public class MvvmWorkspace
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string CurrentGraphId { get; set; }
        public bool Expanded { get; set; }
    }

    public class MVVMNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class MVVMGraph
    {
        public bool IsDirty { get; set; }
        public string Identifier { get; set; }
        public string RootFilterId { get; set; }
        public bool Expanded { get; set; }
    }

    public class ElementNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class CommandsChildItem
    {
        public string Name { get; set; }
        public string RelatedType { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
    }

    public class CollectionsChildItem
    {
        public string RelatedType { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }

    public class BindingsReference
    {
        public string BindingName { get; set; }
        public string SourceIdentifier { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
    }

    public class WorkspaceGraph
    {
        public string GraphId { get; set; }
        public string WorkspaceId { get; set; }
        public string Identifier { get; set; }
    }

    public class UndoItem
    {
        public string Identifier { get; set; }
        public string Time { get; set; }
        public int Group { get; set; }
        public string DataRecordId { get; set; }
        public int Type { get; set; }
        public string RecordType { get; set; }
        public string Data { get; set; }
    }

    public class NavHistoryItem
    {
        public string Identifier { get; set; }
        public string Time { get; set; }
        public bool IsActive { get; set; }
        public string FilterId { get; set; }
        public string WorkspaceId { get; set; }
        public string GraphId { get; set; }
        public Position Scroll { get; set; }
    }

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
    }

    public class FilterItem
    {
        public string Identifier { get; set; }
        public bool Collapsed { get; set; }
        public string NodeId { get; set; }
        public string FilterId { get; set; }
        public Position Position { get; set; }
    }

    public class ConnectionData
    {
        public string OutputIdentifier { get; set; }
        public string InputIdentifier { get; set; }
        public string Identifier { get; set; }
    }

    public class uFrameDatabaseConfig
    {
        public string Identifier { get; set; }
        public string CodeOutputPath { get; set; }
        public string Namespace { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int BuildVersion { get; set; }
        public int BuildNumber { get; set; }
    }

    public class TypeReferenceNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class EnumNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class EnumChildItem
    {
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }

    public class CommandNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class ComputedPropertyNode
    {
        public string PropertyType { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class HandlersReference
    {
        public string SourceIdentifier { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
    }

    public class ServiceNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class SimpleClassNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class StateMachineNode
    {
        public string StartStateOutputSlotId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class StateNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class StateTransitionsReference
    {
        public string SourceIdentifier { get; set; }
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
    }

    public class TransitionsChildItem
    {
        public string Identifier { get; set; }
        public string NodeId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }

    public class ViewComponentNode
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string GraphId { get; set; }
        public string Identifier { get; set; }
        public bool Expanded { get; set; }
    }

    public class FilterStackItem
    {
        public string Identifier { get; set; }
        public string GraphId { get; set; }
        public string FilterId { get; set; }
        public int Index { get; set; }
    }

    public class FlagItem
    {
        
    }

    public class RedoItem
    {
        
    }

}

#endif