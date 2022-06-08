using Mosaic2.Entities;
using Mosaic2.Models;

namespace Mosaic2.Services
{
    public interface IRepository
    {
        void CreateOrder(OrderModel order);
        List<Schedule> GetSchedule();
        void RemoveOrder(Guid orderId);
        List<Order> GetAllOrders();
        List<Baker> GetBakers();
    }

    public class Repository : IRepository
    {
        //list of bakers that are working
        private List<Baker> bakerList = new List<Baker>() 
        { 
            new Baker{Name = "Jim", id = 1, hoursOfWork = 0},
            new Baker{Name = "Bob", id = 2, hoursOfWork = 0},
            new Baker{Name = "Amy", id = 3, hoursOfWork = 0},
        };

        //empty list that will hold incomming orders
        private List<Order> orderList = new List<Order>();

        public Repository()
        {

        }

        //takes an order model (model holds order name and duration)
        //adds an order to the orders list in round robin
        //whoever has the least amount of work gets the incoming order.
        public void CreateOrder(OrderModel order)
        {
            float leastAmntOfWork = float.MaxValue;
            int bakerId = 0;

            //find the least busy worker
            //normaly would do this with LINQ
            //or SQL but simplified for time
            foreach (Baker baker in bakerList)
            {
                if (baker.hoursOfWork < leastAmntOfWork)
                {
                    leastAmntOfWork = baker.hoursOfWork;
                    bakerId = baker.id;
                }
            }

            if (leastAmntOfWork + order.durationHours <= 8.0)
            {
                //create an order entity to store in "Database"
                Order dbOrder = new Order() 
                {
                    Id = Guid.NewGuid(),
                    durationHours = order.durationHours,
                    orderName = order.orderName,
                    bakerId = bakerId
                };
                orderList.Add(dbOrder);

                for (int i = 0; i < bakerList.Count; i++)
                {
                    if (bakerList[i].id == bakerId)
                    {
                        bakerList[i].hoursOfWork += order.durationHours;
                        break;
                    }
                }

                return;
            }

            //if everyone has a full day create an
            //unassigned order and store it
            //ASSUMPTION baker ids start at 1 and increment
            Order UnassignedOrder = new Order()
            {
                Id = Guid.NewGuid(),
                durationHours = order.durationHours,
                orderName = order.orderName,
                bakerId = 0
            };
            orderList.Add(UnassignedOrder);
            return;
        }


        //takes the Id of the order that is being removed 
        //finds the order and removes it from the list of orders
        //if there's an order that is unassigned and can now be completed
        //it gets added for the employee that just had a job removed.
        public void RemoveOrder(Guid orderId)
        {
            var order = orderList.Find(x => x.Id == orderId);
            orderList.Remove(order);
            //if no baker is assigned frees up no space
            //to be completed by another baker so return
            if (order.bakerId == 0)
            {
                return;
            }

            //remove hours of work from baker
            foreach (Baker baker in bakerList)
            {
                if (baker.id == order.bakerId)
                {
                    baker.hoursOfWork -= order.durationHours;
                }
            }

            var unassignedOrders = new List<Order>();

            foreach (Order ord in orderList)
            {
                if (ord.bakerId == 0)
                {
                    unassignedOrders.Add(ord);
                }
            }

            var lowestDuration = float.MaxValue;
            Order oldOrder = new Order();

            foreach (Order o in unassignedOrders)
            {
                if (o.durationHours < lowestDuration)
                {
                    lowestDuration = o.durationHours;
                    oldOrder = o;
                }
            }

            //remove the unassigned order and make a new one that will be assigned if there is time available
            CreateOrder(new OrderModel { durationHours = oldOrder.durationHours, orderName = oldOrder.orderName});
            orderList.Remove(oldOrder);
        }

        //takes no parameters
        //returns the list of orders that are to be completed
        public List<Schedule> GetSchedule()
        {
            var schedule = new List<Schedule>();

            //joining the baker and order "Tables"
            foreach (Order o in orderList)
            {
                var baker = bakerList.Find(baker => baker.id == o.bakerId);
                
                if (baker == null)
                {
                    continue;
                }

                schedule.Add(new Schedule { bakerName = baker.Name, orderId = o.Id, orderName = o.orderName });
            }
            return schedule;
        }


        //for testing
        public List<Order> GetAllOrders()
        {
            return orderList;
        }


        //for testing
        public List<Baker> GetBakers()
        {
            return bakerList;
        }
    }
}
