#nullable enable
namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Specifies kind of <seealso cref="ObjectTreeNode"/>.
/// </summary>
public enum ObjectTreeNodeKind
{
    /// <summary>
    /// Value node, representing singular, simple value
    /// </summary>
    Value, 
    /// <summary>
    /// Array node that can hold sub-nodes in array format
    /// </summary>
    Array, 
    /// <summary>
    /// Object node that can hold sub-nodes as named properties
    /// </summary>
    Object, 
    /// <summary>
    /// Reference node that represents value already existing in upper part of the object graph (used to represent circular references)
    /// </summary>
    Reference
}