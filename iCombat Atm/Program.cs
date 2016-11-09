using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCombat_Atm
{
    class Program
    {

        class bill
        {        
            public int value;
            public int amount;
            public bill(int val, int n)
            {
                this.value = val;
                this.amount = n;
            }
        }
        static List<bill> stock = new List<bill>();

        static void Main(string[] args)
        {
            //start off with fresh machine
            stock.Add(new bill(100, 10));
            stock.Add(new bill(50, 10));
            stock.Add(new bill(20, 10));
            stock.Add(new bill(10, 10));
            stock.Add(new bill(5, 10));
            stock.Add(new bill(1, 10));

            //then route input
            string cmd = "";
            string[] options;
            while (cmd != "q")
            {                

                string[] input = Console.ReadLine().ToLower().Split(' ');               
                cmd = input[0];
                options = input.Skip(1).ToArray();

                switch (cmd)
                {                    
                    case "r":
                        {                            
                            Restock();
                            break;
                        }

                    case "w":
                        {
                            Withdrawl(options);
                            break;
                        }

                    case "i":
                        {                            
                            ShowInventory(options);
                            break;
                        }

                    case "q":
                        break;

                    default:
                        {
                            Console.WriteLine("Failure: Invalid Command \n");
                            break;
                        }
                }
            }
        }

        static void Withdrawl(string[] options) {
            if (!options.Any())
            {
                Console.WriteLine("Failure: Invalid Arguments \n");
                return;
            }

            //check if amount starts with $ then remove it.
            string amount = options[0];
            if (!amount.StartsWith("$"))
            {
                Console.WriteLine("Failure: Invalid Arguments \n");
                return;
            }

            int amountRemaining;
            if (!Int32.TryParse(amount.TrimStart('$'), out amountRemaining)) {
                Console.WriteLine("Failure: Invalid Arguments \n");
                return;
            }

            //Machine has finite funds, 
            //queue up action and verify before moving money
            List<bill> queue = new List<bill>();

            foreach (bill money in stock)
            {
                int count = (int)(amountRemaining / money.value);
                if (count > money.amount) {
                    count = money.amount;
                }

                if (count != 0) {
                    amountRemaining -= count * money.value;
                    queue.Add(new bill(money.value, count));
                }                
            }

            if (amountRemaining > 0) {
                Console.WriteLine("Failure: insufficient funds \n");
                return;
            }
            
            //apply queued up action against the current stock.
            foreach (bill money in queue)
            {
                bill item = stock.Find(x => x.value == money.value);
                item.amount -= money.amount;
            }

            Console.WriteLine("Success: Dispensed {0}", amount);
            DisplayBalance();

        }

        static void ShowInventory(string[] options)
        {
            if (!options.Any())
            {
                Console.WriteLine("Failure: Invalid Arguments \n");
                return;
            }

            var dict = stock.ToDictionary(item => item.value.ToString(), item => item.amount);
            string message = "";
            
            foreach (string amount in options)
            {            
                int denomination;
                //check if amount starts with $ then remove it.
                if (!amount.StartsWith("$"))
                {
                    Console.WriteLine("Failure: Invalid Arguments \n");
                    return;                    
                }
                //pulled out of next line so it's easier to read.
                var tempAmount = amount.TrimStart('$');
            
                //check if request is valid
                if (!dict.TryGetValue(tempAmount, out denomination)){
                    Console.WriteLine("Failure: Invalid Arguments \n");
                    return;
                }


                message += amount + " - " + denomination + "\n";
            }
            if (message == "") {
                Console.WriteLine("Failure: Invalid Arguments \n");
            }

            Console.Write(message + "\n");
        }

        static void Restock(bool ShowMessage = true)
        {                       
            foreach (bill money in stock)
            {
                money.amount = 10;                
            }
            DisplayBalance();
        }

        static void DisplayBalance()
        {
            Console.WriteLine("Machine Balance:");
            foreach (bill money in stock)
            {                
                Console.Write("${0} - {1} \n", money.value, money.amount);
            }
            Console.Write("\n");
        }
    }
}
