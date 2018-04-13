using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinNetCoreConsole
{
    class Program
    {
        const int MAX_BYTES = 1000000; 
        const double BASE_REWARD = 12.5;
        static void Main(string[] args)
        {

            int[] bytes = {57247, 98732, 134928, 77275, 29240, 15440, 70820, 139603, 63718, 143807, 190457, 40572, 51022, 62122, 99165};
            double[] fees = {0.0887, 0.1856, 0.2307, 0.1522, 0.0532, 0.0250, 0.1409, 0.2541, 0.1147, 0.2660, 0.2933, 0.0686, 0.1212, 0.0173, 0.1498};
            List <Transaction> trans = new List<Transaction>();

            // create list
            for (int index = 0; index < bytes.Length; index++)
            {
                var t = new Transaction()
                {
                    Id = index + 1,
                    Bytes = bytes[index],
                    Fee = fees[index]
                };

                trans.Add(t);
            }

            var reward = CalMaxReward(trans);
            var totalReward = reward > 0 ? BASE_REWARD + CalMaxReward(trans) : reward;

            Console.WriteLine($"Max reward: {totalReward} BTC");

            Console.ReadKey();
        }

        static double CalMaxReward(List<Transaction> trans)
        {
            double reward = 0f;

            var orderTrans = trans.OrderBy(t => t.Bytes);

            if(orderTrans.ElementAt(0).Bytes > MAX_BYTES)
            {
                return reward;
            }

            // create queue for BFS tracing
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(CreateNode(CloneTrans(orderTrans.ElementAt(0))));
            queue.Enqueue(CreateNode(orderTrans.ElementAt(0), 1));

            while(queue.Count > 0)
            {
                var node = queue.Dequeue();
                var curLevel = node.Level;

                reward = node.TotalFee > reward ? node.TotalFee : reward;
                var remainByte = MAX_BYTES - node.TotalBytes;
                if (curLevel < orderTrans.Count() - 1  && orderTrans.ElementAt(curLevel + 1).Bytes <= remainByte)
                {
                    // create child node
                    var nextTrans = orderTrans.ElementAt(curLevel + 1);
                    queue.Enqueue(CreateNode(CloneTrans(nextTrans), 0, node, curLevel + 1));
                    queue.Enqueue(CreateNode(nextTrans, 1, node, curLevel + 1));
                }

            }

            return reward;
        }

        static Node CreateNode(Transaction target, int isSelected = 0, Node parent = null, int level = 0)
        {
            var newNode = new Node()
            {
                TotalBytes = parent != null ? target.Bytes * isSelected + parent.TotalBytes : target.Bytes * isSelected,
                TotalFee = parent != null ? target.Fee * isSelected + parent.TotalFee : target.Fee * isSelected,
                Level = level,
                IsSelected = isSelected
                
            };

            return newNode;
        }

        static Transaction CloneTrans(Transaction target)
        {
            var newTrans = target.Clone() as Transaction;
            return newTrans;
        }

    }

    internal class Node
    {
        public int Level { get; set; }
        public int TotalBytes { get; set; }
        public double TotalFee { get; set; }

        // selected: 1, not selected: 0
        public int IsSelected { get; set; }

        public Node()
        {
            Level = 0;
        }
    }

    internal class Transaction : ICloneable
    {
        public int Id { get; set; }
        public int Bytes { get; set; }
        public double Fee { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
