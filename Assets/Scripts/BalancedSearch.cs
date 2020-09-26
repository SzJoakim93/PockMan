using System;
using System.Collections.Generic;
using UnityEngine;

public class BalancedSearch
{
    class Node {
        public int Cost;
        public int prev_direction;
        public int x;
        public int y;
        public Node(int _x, int _y, int _cost, int _prev_dir) {
            x = _x;
            y = _y;
            Cost = _cost;
            prev_direction = _prev_dir;
        }
    }

   class Way {
       public Way(Node _node) {
           nodes = new List<Node>();
           nodes.Add(_node);
           Cost = 0;
       }

       Way(List<Node> _nodes, int _cost) {
           this.nodes = new List<Node>();
           this.nodes.AddRange(_nodes);
           this.Cost = _cost;
       }

       public Way CloneAndAdd(Node _node) {
           Way new_way = new Way(this.nodes, this.Cost);
           new_way.nodes.Add(_node);
           new_way.Cost += _node.Cost;
           return new_way;
       }

       public Node GetLastNode() {
            if (nodes.Count > 0)
                return nodes[nodes.Count-1];
            else
                return null;
       }

       public int Cost { get; set; }
       public List<Node> nodes;
   }

    List<Way> ways = new List<Way>();
    Way min_way;
    int start_x;
    int start_y;
    int goal_x;
    int goal_y;
    int range;
    float begin_time;

    public void search(int _start_x, int _start_y, int _goal_x, int _goal_y, int _range = 0) {
        begin_time = Time.time;
        start_x = _start_x;
        start_y = _start_y;
        goal_x = _goal_x;
        goal_y = _goal_y;
        range = _range;
        ways.Clear();

        Node start = new Node(start_x, start_y, 0, -1);
        Way new_way = new Way(start);
        ways.Add(new_way);
        while(!GetLowestNodeWays());
    }

    public int NextWayX() {
        return (min_way.nodes.Count > 1 ? min_way.nodes[1].x : min_way.nodes[0].x);
    }

    public int NextWayY() {
        return (min_way.nodes.Count > 1 ? min_way.nodes[1].y : min_way.nodes[0].y);
    }

    List<Node> GetChildren(Node node)
    {
        List<Node> children = new List<Node>();

        int current_pos = Global.levelmatrix[node.y, node.x];
        if (node.prev_direction != 1 && (current_pos == 10 || current_pos == 9 || current_pos == 7 || current_pos == 6 || current_pos == 5 || current_pos == 2))
            children.Add(GetHorizontSide(node, -1));
        if (node.prev_direction != 0 && (current_pos == 10 || current_pos == 8 || current_pos == 7 || current_pos == 6 || current_pos == 4 || current_pos == 3))
            children.Add(GetHorizontSide(node, 1));
        if (node.prev_direction != 2 && (current_pos == 10 || current_pos == 9 || current_pos == 8 || current_pos == 6 || current_pos == 5 || current_pos == 4))
            children.Add(GetVerticalSide(node, -1));
        if (node.prev_direction != 3 && (current_pos == 10 || current_pos == 9 || current_pos == 8 || current_pos == 7 || current_pos == 3 || current_pos == 2))
            children.Add(GetVerticalSide(node, 1));

        return children;
    }

    Node GetHorizontSide(Node node, int dir)
    {
        int y = node.y;
        int i = node.x + dir;
        int cost = 1;

        while(Global.levelmatrix[y, i] == 0)
        {
            i += dir;
            cost++;
        }

        return new Node(i, y, cost, (dir == -1 ? 0 : 1));
    }

    Node GetVerticalSide(Node node, int dir)
    {
        int i = node.y + dir;
        int x = node.x;
        int cost = 1;

        while(Global.levelmatrix[i, x] == 1)
        {
            i += dir;
            cost++;
        }

        return new Node(x, i, cost, (dir == -1 ? 3 : 2));
    }

    bool GetLowestNodeWays() {
        min_way = ways[0];

        foreach (var way in ways) {
            if (way.Cost < min_way.Cost)
                min_way = way;
        }

        if (min_way.GetLastNode().x == goal_x && min_way.GetLastNode().y == goal_y ||
            (min_way.GetLastNode().x < goal_x + range && min_way.GetLastNode().x > goal_x - range &&
                min_way.GetLastNode().y < goal_y + range && min_way.GetLastNode().y > goal_y - range) ||
            Time.time - begin_time > 3.0f) {
            return true;
        }
            

        foreach (var node in GetChildren(min_way.GetLastNode())) {
            ways.Add(min_way.CloneAndAdd(node));
        }

        ways.Remove(min_way);

        return false;
    }

}
