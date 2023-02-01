using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// N-Ary tree that represents the objectives structure of a level
/// </summary>
[Serializable]
public class LevelTree
{
    /// <summary>
    /// Node from the objectives tree
    /// </summary>
    [Serializable]
    public class Node
    {
        public string name;
        public string parentName;
        public int height;
        public int siblingIndex;

        public float x;
        public float y;

        public Dialog dialog;

        private List<Transition> transitions;
        public string[] transitionList;

        //public Node[] children;
        private List<Node> children;
        private Node parent;
        
        /// <summary>
        /// When a node is deleted, the isValid flag is set to false
        /// This informs a NodeButton that it must destroy itself because its node has been removed from the tree
        /// </summary>
        private bool isValid = true;

        /// <summary>
        /// Default constructor
        /// Requires a reference to its parent node
        /// </summary>
        /// <param name="parent">Reference to its parent node</param>
        public Node(Node parent)
        {
            transitions = new List<Transition>();
            children = new List<Node>();
            dialog = new Dialog();
            if(parent != null)
            {
                this.height = parent.height + 1;
                this.parent = parent;
            }
            else
            {
                this.name = "0";
            }
        }

        /// <summary>
        /// Adds a new Objective Transition to this node
        /// In the level progression a Node requires all its transitions to Check before the node itself can Check
        /// A node represents a set of objectives to be achieved during a level
        /// Once a node Checks the game loads its children
        /// A linear level has only one child per node
        /// If a level has more than one child per node, the level completes as soon as a Leaf Node Checks
        /// </summary>
        /// <param name="newTransition">The transition to be added to this node</param>
        public void AddTransition(Transition newTransition)
        {
            if (transitions == null) transitions = new List<Transition>();
            transitions.Add(newTransition);
            SerializeTransitions();
        }

        /// <summary>
        /// Removes a transition from this node
        /// </summary>
        /// <param name="transitionToRemove">The transition to be removed</param>
        public void RemoveTransition(Transition transitionToRemove)
        {
            if (transitions == null) transitions = new List<Transition>();
            transitions.Remove(transitionToRemove);
            SerializeTransitions();
        }

        /// <summary>
        /// Turns all transitions into JSON strings
        /// </summary>
        public void SerializeTransitions()
        {
            if (transitions == null) transitions = new List<Transition>();
            transitionList = new string[transitions.Count];
            for(int i = 0; i < transitions.Count; ++i)
            {
                transitionList[i] = transitions[i].ToJson();
            }
        }

        /// <summary>
        /// Recovers all transitions from their serialized version
        /// </summary>
        public void RebuildTransitions()
        {
            transitions = new List<Transition>();
            if(transitionList != null)
            {
                for (int i = 0; i < transitionList.Length; ++i)
                {
                    transitions.Add(Transition.FromJson(transitionList[i]));
                }
            }
        }

        /// <summary>
        /// Returns the full list of transitions for this node
        /// </summary>
        /// <returns></returns>
        public List<Transition> GetTransitions()
        {
            return transitions;
        }

        /// <summary>
        /// A node Checks if all of its transitions Check
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            bool check = true;
            foreach(Transition t in transitions)
            {
                check = t.Check() && check;
            }
            return check;
        }

        /// <summary>
        /// If this node doesn't have a list of children, create a new one
        /// </summary>
        public void RebuildChildren()
        {
            if(children == null)
            {
                children = new List<Node>();
            }
        }

        /// <summary>
        /// Adds an empty new node to this node's children
        /// </summary>
        /// <returns></returns>
        public Node AddChild()
        {
            Node newNode = new Node(this);
            children.Add(newNode);
            RefreshChildren();
            return newNode;
        }

        /// <summary>
        /// Adds an existing child to the node
        /// </summary>
        /// <param name="n">the child node to add</param>
        public void AddChild(Node n)
        {
            children.Add(n);
            RefreshChildren();
        }

        /// <summary>
        /// Removes a specific child from this node's children
        /// </summary>
        /// <param name="child">The node to be removed</param>
        public void RemoveChild(Node child)
        {
            children.Remove(child);
            RefreshChildren();
        }

        public List<Node> GetChildren()
        {
            return children;
        }

        /// <summary>
        /// Removes this node and all its children from the tree
        /// </summary>
        public void Remove()
        {
            //DeleteChildren();

            foreach(Node n in children)
            {
                parent.AddChild(n);
            }

            parent.RemoveChild(this);
            isValid = false;
        }

        /// <summary>
        /// Recursively deletes all the children of this node
        /// </summary>
        private void DeleteChildren()
        {
            foreach(Node child in children)
            {
                child.DeleteChildren();
                child.parent = null;
            }
            isValid = false;
            children.Clear();
        }

        /// <summary>
        /// A Node is valid as long as it has not been removed
        /// </summary>
        /// <returns>Whether this node is valid</returns>
        public bool IsValid()
        {
            return isValid;
        }

        /// <summary>
        /// Sets all the information for the children of this node
        /// For instance the name of the node, the name of its parent, and the reference to its parent
        /// </summary>
        public void RefreshChildren()
        {
            if(children != null)
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    children[i].siblingIndex = i;
                    children[i].name = name + (i + 1);
                    children[i].parentName = name;
                    children[i].parent = this;
                    children[i].RefreshChildren();
                }
            }
        }

        /// <summary>
        /// When a node is deserialized its valid value is false
        /// This method resets the value to true
        /// </summary>
        public void SetValid()
        {
            isValid = true;
        }

        /// <summary>
        /// The name of the node is a composition of the level of the node and its sibling index.
        /// For instance the node 013 is the third child of the first child of the root node
        /// The root node's name is always 0
        /// </summary>
        /// <returns>The name of this node</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Sets the name of this node.
        /// The name of a node can only be set by its parent
        /// </summary>
        /// <param name="name"></param>
        private void SetName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// A Leaf Node is one that has no children
        /// </summary>
        /// <returns>True if this node is a Leaf Node</returns>
        public bool IsLeaf()
        {
            return children == null || children.Count == 0;
        }

        /// <summary>
        /// Returns the parent of this node
        /// </summary>
        /// <returns></returns>
        public Node GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Sets the parent of this node to a new reference
        /// </summary>
        /// <param name="parent">The node to assign as parent</param>
        public void SetParent(Node parent)
        {
            this.parent = parent;
        }

        public void RebuildDialog()
        {
            if (dialog == null)
            {
                dialog = new Dialog();
            }
        }

        /// <summary>
        /// Serialices this node to a JSON string
        /// </summary>
        /// <returns>This node's information in the form of a JSON string</returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// Deserialices a node from a JSON string
        /// </summary>
        /// <param name="json">A JSON string with the information of a node</param>
        /// <returns>The deserialiced node</returns>
        public static Node FromJson(string json)
        {
            return JsonUtility.FromJson<Node>(json);
        }
    }

    [Serializable]
    public class HexToBuilding
    {
        public BuildingType type;
        public string hexName;

        public HexToBuilding(BuildingType type, string hexName)
        {
            this.type = type;
            this.hexName = hexName;
        }
    }


    public string planetName;
    public string planetDescription;
    public bool[] enabledBuildings;
    public double[] initialResources;

    public HexToBuilding[] initialBuildings;
    private List<HexToBuilding> _initialBuildings;

    private Node root;
    public Node[] nodeList;
    
    /// <summary>
    /// Default constructor for the Tree
    /// Creates a root node
    /// </summary>
    public LevelTree()
    {
        root = new Node(null);
        _initialBuildings = new List<HexToBuilding>();
    }

    public Node GetRoot()
    {
        return root;
    }

    // TODO
    /// <summary>
    /// When the tree is deserialiced some parts of the nodes are missing
    /// This method recursively rebuilds all the nodes to their final state
    /// </summary>
    /// <param name="node"></param>
    public void DeserializeNode(Node node, Node parent)
    {
        node.SetParent(parent);
        node.SetValid();
        node.RebuildTransitions();
        node.RefreshChildren();
        node.RebuildDialog();
        foreach(Node n in node.GetChildren())
        {
            DeserializeNode(n, node);
        }
    }

    /// <summary>
    /// Adds a new initial building structure to the tree
    /// </summary>
    /// <param name="type">the type of building to be added</param>
    /// <param name="hexName">the name of the tile where the building is added</param>
    public void AddInitialBuilding(string hexName, BuildingType type)
    {
        bool found = false;
        foreach(HexToBuilding htb in _initialBuildings)
        {
            if(htb.hexName == hexName)
            {
                htb.type = type;
                found = true;
            }
        }
        if(!found)
        {
            _initialBuildings.Add(new HexToBuilding(type, hexName));
        }
        initialBuildings = _initialBuildings.ToArray();
    }

    /// <summary>
    /// Removes an initial building from the tree
    /// </summary>
    /// <param name="hexName">the name of the hex to clear</param>
    public void RemoveInitialBuilding(string hexName)
    {
        HexToBuilding hex = null;
        foreach(HexToBuilding htb in _initialBuildings)
        {
            if(htb.hexName == hexName)
            {
                hex = htb;
            }
        }
        if(hex != null)
        {
            _initialBuildings.Remove(hex);
            initialBuildings = _initialBuildings.ToArray();
        }
    }

    public Node FindNodeByName(string name)
    {
        return FindNodeByName(root, name);
    }

    private Node FindNodeByName(Node node, string name)
    {
        if(node.GetName() == name)
        {
            return node;
        }
        else
        {
            Node n = null;
            List<Node> children = node.GetChildren();
            foreach(Node child in children)
            {
                n = FindNodeByName(child, name);
                if(n != null)
                {
                    return n;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Serialices the entire tree into a JSON string
    /// </summary>
    /// <returns>The tree's information in the form of a JSON string</returns>
    public string ToJson()
    {
        List<Node> nodes = new List<Node>();

        nodes.Add(root);
        for(int i = 0; i < nodes.Count; ++i)
        {
            nodes.AddRange(nodes[i].GetChildren());
        }
        nodeList = nodes.ToArray();

        return JsonUtility.ToJson(this, true);
    }

    /// <summary>
    /// Deserialices a tree from a JSON string 
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static LevelTree FromJson(string json)
    {
        LevelTree tree = JsonUtility.FromJson<LevelTree>(json);
        
        tree._initialBuildings = new List<HexToBuilding>();
        if (tree.initialBuildings != null)
        {
            tree._initialBuildings.AddRange(tree.initialBuildings);
        }

        if(tree.nodeList != null)
        {
            tree.root = tree.nodeList[0];

            List<Node> nodes = new List<Node>();
            nodes.AddRange(tree.nodeList);
            while (nodes.Count > 0)
            {
                Node node = nodes[0];
                nodes.RemoveAt(0);
                node.RebuildChildren();
                foreach (Node n in nodes)
                {
                    if(n.parentName == node.name)
                    {
                        node.AddChild(n);
                    }
                }
            }
            tree.DeserializeNode(tree.root, null);
        }
        return tree;
    }
}


