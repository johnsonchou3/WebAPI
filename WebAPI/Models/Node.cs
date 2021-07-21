using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPI.Models
{
    /// <summary>
    /// 樹節點的class
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 節點本身的值(operand/operator)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 左邊Children的節點
        /// </summary>
        public Node Left { get; set; }

        /// <summary>
        /// 右邊Children的節點
        /// </summary>
        public Node Right { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">節點本身的值(operand/operator)</param>
        public Node(string c)
        {
            Value = c;
            Left = Right = null;
        }

        /// <summary>
        /// 以expression list 創建Tree, 在一開始先以stack儲存數字及+-*/, 並在list 前後加上"()"以判斷式子是否結束
        /// 在遇到下一個operator 時, 透過dictionary 判斷要先以前者或後者作節點
        /// 最後再回傳Tree的root, 也是StackNode的最上層(Peek)
        /// </summary>
        /// <param name="Expressionlist">需要Btn的ExpressionList, iterate 每一個operand/operator</param>
        /// <returns></returns>
        public static Node CreateTree(List<string> Expressionlist)
        {
            Stack<Node> StackNode = new Stack<Node>();
            Stack<string> StackString = new Stack<string>();
            Node t;
            Node t1;
            Node t2;
            Dictionary<string, int> AssociativityMap = new Dictionary<string, int>();
            AssociativityMap.Add("+", 1);
            AssociativityMap.Add("-", 1);
            AssociativityMap.Add("*", 2);
            AssociativityMap.Add("/", 2);
            AssociativityMap.Add(")", 0);
            Expressionlist.Insert(0, "(");
            Expressionlist.Add(")");
            foreach (string oper in Expressionlist)
            {
                if (oper == "(")
                {
                    StackString.Push(oper);
                }
                else if (!AssociativityMap.ContainsKey(oper))
                {
                    t = new Node(oper);
                    StackNode.Push(t);
                }
                else if (AssociativityMap[oper] > 0)
                {
                    while (StackString.Count != 0 && StackString.Peek() != "("
                        && (oper != "^" && AssociativityMap[StackString.Peek()] >= AssociativityMap[oper]))
                    {
                        t = new Node(StackString.Pop());
                        t1 = StackNode.Pop();
                        t2 = StackNode.Pop();
                        t.Left = t2;
                        t.Right = t1;
                        StackNode.Push(t);
                    }
                    StackString.Push(oper);
                }
                else if (oper == ")")
                {
                    while (StackString.Count != 0 && StackString.Peek() != "(")
                    {
                        t = new Node(StackString.Peek());
                        StackString.Pop();
                        t1 = StackNode.Peek();
                        StackNode.Pop();
                        t2 = StackNode.Peek();
                        StackNode.Pop();
                        t.Left = t2;
                        t.Right = t1;
                        StackNode.Push(t);
                    }
                    StackString.Pop();
                }
            }
            t = StackNode.Peek();
            return t;
        }
    }
}
