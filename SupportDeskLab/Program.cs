using System;
using System.Collections.Generic;
using static SupportDeskLab.Utility;


namespace SupportDeskLab
{
   
     
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary

       static Dictionary<String, Customer> Customers = new Dictionary<String , Customer>();
        //Create Ticket Queue
       static Queue <Ticket> Queue = new Queue <Ticket>();
        //Create UndoEvent stack
        static Stack <UndoEvent> Stack = new Stack <UndoEvent>();
        static void Main()
        {
            initCustomer();

            while (true)
            {
                Console.WriteLine("\n=== Support Desk ===");
                Console.WriteLine("[1] Add customer");
                Console.WriteLine("[2] Find customer");
                Console.WriteLine("[3] Create ticket");
                Console.WriteLine("[4] Serve next ticket");
                Console.WriteLine("[5] List customers");
                Console.WriteLine("[6] List tickets");
                Console.WriteLine("[7] Undo last action");
                Console.WriteLine("[0] Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                //create switch cases and then call a reletive method 
                //for example for case 1 you need to have a method named addCustomer(); or case 2 add a method name findCustomer

                switch (choice)
                {
                    case "1": AddCustomer(); break;
                    case "2": FindCustomer(); break;
                    case "3": CreateTicket(); break;
                    case "4": ServeNext(); break;
                    case "5": ListCustomers(); break;
                    case "6": ListTickets(); break;
                    case "7": Undo(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
        /*
         * Do not touch initCustomer method. this is like a seed to have default customers.
         */
        static void initCustomer()
        {  
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            Console.WriteLine("New Customer id (e.g., C004): ");
            string id = Console.ReadLine();

            if (Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer id already exists.");
                return;
            }

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Customer c = new Customer(id, name, email);
            Customers[id] = c;

            Console.WriteLine("Customer added: " + c);
            Stack.Push(new UndoAddCustomer(c));
        }

        static void FindCustomer()
        {
            Console.Write("Enter customer id: ");
            string id = Console.ReadLine();

            if (Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer found: " + Customers[id]);
            }
            else
            {
                Console.WriteLine("Customer not found.");
            }

        }

        static void CreateTicket()
        {
           Console.Write("Enter customer id: ");
            string id = Console.ReadLine();
            if (!Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer not found.");
                return;
            }
            Console.Write("Enter subject: ");
            string subject = Console.ReadLine();
            Ticket t = new Ticket(NextTicketId++, id, subject);
            Queue.Enqueue(t);
            Console.WriteLine("Ticket created: " + t);
            Stack.Push(new UndoCreateTicket(t));
        }

        static void ServeNext()
        {
            if (Queue.Count == 0)
            {
                Console.WriteLine("No tickets to serve.");
                return;
            }
            Ticket t = Queue.Dequeue();
            Console.WriteLine("Serving ticket: " + t);
            Stack.Push(new UndoServeTicket(t));

        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");
            if (Customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }
            foreach (var customer in Customers.Values)
            {
                Console.WriteLine(customer);
            }

        }

        static void ListTickets()
        {
           
            Console.WriteLine("-- Tickets (front to back) --");
            if (Queue.Count == 0)
            {
                Console.WriteLine("No tickets in the queue.");
                return;
            }
            foreach (var ticket in Queue)
            {
                Console.WriteLine(ticket);
            }

        }

        static void Undo()
        {
            Console.WriteLine("-- Undo Last Action --");
            if (Stack.Count == 0)
            {
                Console.WriteLine("No actions to undo.");
                return;
            }
            UndoEvent ue = Stack.Pop();

            if (ue is UndoAddCustomer uac)
            {               
                Customers.Remove(uac.Customer.CustomerId);
                Console.WriteLine($"Undo: Removed customer {uac.Customer.CustomerId}");
            }
            else if (ue is UndoCreateTicket uct)
            {     
               Queue <Ticket> temp = new Queue <Ticket>();
                while (Queue.Count > 0)
                {
                    Ticket current = Queue.Dequeue();
                    if(current.TicketId != uct.Ticket.TicketId) 
                        temp.Enqueue(current);
                    }
                Queue = temp;
                Console.WriteLine($"Undo: Removed ticket #{uct.Ticket.TicketId}");

            }
            
            else if (ue is UndoServeTicket ust)
            {
               Queue<Ticket> newQueue = new Queue <Ticket>();
                newQueue.Enqueue(ust.Ticket);
                foreach (var t in Queue)
                  newQueue.Enqueue(t);
                Queue = newQueue;
                Console.WriteLine($"Undo: Re-added served ticket #{ust.Ticket.TicketId}");

            }

        }
    }
}

