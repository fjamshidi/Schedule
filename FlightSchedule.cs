using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace Schedule
{
    class FlightSchedule
    {
        static List<Flight> flights = new List<Flight>();
        static List<Order> torontoOrders = new List<Order>();
        static List<Order> calgaryOrders = new List<Order>();
        static List<Order> vancouverOrders = new List<Order>();
        static List<Order> otherCities = new List<Order>();
        static int flightNumber = 1;
        static void Main(string[] args)
        {
            loadSchedule("schedule.txt");
            printSchedule();
            loadOrders("orders.json");
            setSchedule();
        }

        private static void loadSchedule(string fileName)
        {
            var lines = File.ReadLines(fileName);
            int day = 0;
            int flightnumber = 1;
            foreach (var line in lines)
            {
                if (line.Contains("Day"))
                {
                    day = Convert.ToInt32(Char.GetNumericValue(line[line.Length - 2]));
                }
                else
                {
                    Flight flight = new Flight();
                    flight.Day = day;
                    var result = line.Split().Where(x => x.StartsWith("(") && x.EndsWith(")"))
                     .Select(x => x.Replace("(", string.Empty).Replace(")", string.Empty))
                     .ToList();
                    flight.Departure = result[0];
                    flight.Arrival = result[1];
                    flight.Number = flightnumber;
                    flights.Add(flight);
                    flightnumber++;
                }

            }
        }
        private static void printSchedule()
        {
            foreach (var flight in flights)
            {
                Console.WriteLine("Flight: " + flight.Number + ", departure: " + flight.Departure + ", arrival: " + flight.Arrival + ", day: " + flight.Day);
            }
        }
        private static void loadOrders(string fileName)
        {
            var jsonText = File.ReadAllText(fileName);
            jsonText = Regex.Replace(jsonText, @"\t|\n|\r", "");
            var reults = jsonText.Split(',').ToList<string>();
            int priority = 1;
            foreach (var item in reults)
            {
                string order = Regex.Replace(item, @"\s+|{|}", "");
                string[] orderDeatils = order.Split(':');
                Order newOrder = new Order();
                newOrder.Departure = "YUL";
                newOrder.OrderID = orderDeatils[0].Replace("\"", "");
                newOrder.Arrival = orderDeatils[2].Replace("\"", "");
                newOrder.Priority = priority;
                newOrder.Day = 0;//unscheduled
                switch (newOrder.Arrival.Trim())
                {
                    case "YYZ":
                        torontoOrders.Add(newOrder); break;
                    case "YYC":
                        calgaryOrders.Add(newOrder); break;
                    case "YVR":
                        vancouverOrders.Add(newOrder); break;
                    default:
                        otherCities.Add(newOrder); break;
                }
                priority++;
            }            
        }
        private static void setSchedule()
        {
            torontoOrders.OrderBy(item => item.Priority);
            calgaryOrders.OrderBy(item => item.Priority);
            vancouverOrders.OrderBy(item => item.Priority);
            setFlightDetails(torontoOrders.Take(20).ToList(), 1);
            setFlightDetails(calgaryOrders.Take(20).ToList(), 1);
            setFlightDetails(vancouverOrders.Take(20).ToList(), 1);
            setFlightDetails(torontoOrders.Skip(20).Take(20).ToList(), 2);
            setFlightDetails(calgaryOrders.Skip(20).Take(20).ToList(), 2);
            setFlightDetails(vancouverOrders.Skip(20).Take(20).ToList(), 2);
            List<Order> allOrders = new List<Order>();
            allOrders.AddRange(torontoOrders);
            allOrders.AddRange(calgaryOrders);
            allOrders.AddRange(vancouverOrders);
            allOrders.AddRange(otherCities);
            allOrders.OrderBy(item => item.Priority);
            foreach (var item in allOrders)
            {
                if (item.FlightNumber > 0)
                    Console.WriteLine("order: " + item.OrderID + " ,flightNumber: " + item.FlightNumber + " ,departure: " + item.Departure + " ,arrival: " + item.Arrival + " ,day: " + item.Day);
                else
                    Console.WriteLine("order: " + item.OrderID + " flightNumber: not scheduled");
            }
            Console.ReadLine();
        }
        private static void setFlightDetails(List<Order> orders, int day)
        {
            if (orders.Count > 0)
            {
                foreach (var item in orders)
                {
                    item.FlightNumber = flightNumber;
                    item.Day = day;
                }
                flightNumber++;
            }
        }
    }

    class Flight
    {
        public int Day { get; set; }
        public int Number { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
    }

    class Order
    {
        public string OrderID { get; set; }
        public int FlightNumber { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public int Day { get; set; }
        public int Priority { get; set; }
    }
}