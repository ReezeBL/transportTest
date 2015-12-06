using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace transportTest
{
    public class Program
    {
        public class Waypoint
        {
            public double X, Y;
            public DateTime time;
            public int RouteID;
            public Waypoint(double X = 0, double Y = 0)
            {
                this.X = X;
                this.Y = Y;
            }
            public double Distance(Waypoint w)
            {
                return Math.Sqrt((w.X - X) * (w.X - X) + (w.Y - Y) * (w.Y - Y));
            }
            public override bool Equals(object obj)
            {
                if (obj is Waypoint)
                {
                    Waypoint tmp = (Waypoint)obj;
                    return tmp.Distance(this) <= 1;
                }
                else
                    return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        private class Velocity
        {

        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {           
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        #region Variables&Consts
        static Waypoint beg = new Program.Waypoint(11038.08464497, 8253.17542416);
        static Waypoint end = new Program.Waypoint(283.08479678, 163.45489494);

        static List<Waypoint> data = new List<Waypoint>();
        static Dictionary<String, int> routeIdMap = new Dictionary<string, int>();

        static List<Waypoint> selected = null;
        #endregion

        static void Main(string[] args)
        {
           
            String[] lines = File.ReadAllLines("train.txt");
            Console.WriteLine("Файл с данными усмпешно прочитан. Найдено {0} элементов.", lines.Length);
            int ID = 0;         
            foreach (String line in lines)
            {
                String[] elems = line.Split('\t');
                Waypoint w = new Waypoint();
                w.X = Double.Parse(elems[1], CultureInfo.InvariantCulture);
                w.Y = Double.Parse(elems[2], CultureInfo.InvariantCulture);
                w.time = UnixTimeStampToDateTime(Double.Parse(elems[0], CultureInfo.InvariantCulture));
                if (routeIdMap.ContainsKey(elems[3])) {
                    w.RouteID = routeIdMap[elems[3]];
                }
                else
                {
                    w.RouteID = ID++;
                    routeIdMap.Add(elems[3], ID);
                }
                data.Add(w);
                
            }          
            Console.WriteLine("Обработка данных завершена. Всего транспортных средств зарегестрировано: {0}", routeIdMap.Count);
            foreach(String n in routeIdMap.Keys)
            {
                Console.WriteLine("\t{0}:{1}", n, data.Where(a => a.RouteID == routeIdMap[n]).Count());
            }
            Console.WriteLine("Введите идентификатор транспортного средства, для построения тестовой выборки: ");
            String command = "";
            do
            {
                command = Console.ReadLine();
            } while (ParseCommand(command));
        }
        static bool ParseCommand(String command)
        {
            String[] subs = command.ToLower().Split(' ');
            switch (subs[0])
            {
                case "exit":
                    return false;
                case "select":
                    if (subs.Length == 2)
                        SelectStatGrid(subs[1]);
                    else if (subs.Length == 4)
                        SelectStatGrid(subs[1], TimeSpan.Parse(subs[2]), TimeSpan.Parse(subs[3]));
                    return true;
                case "show":
                    ShowGrid();
                    return true;
                case "weed":
                    WeedSelection();
                    return true;
                default:
                    Console.WriteLine("Неизвестная комманда!");
                    return true;
            }
        }

        private static void WeedSelection()
        {
            selected.Sort((a, b) => a.RouteID == b.RouteID ? a.time.CompareTo(b.time) : a.RouteID.CompareTo(b.RouteID));
            List<Waypoint> tmp;
            double dprev = 0;
            tmp = selected.Where(w => { bool ans = w.Distance(beg) > dprev; dprev = w.Distance(beg); return ans && w.Y < beg.Y + 100; }).ToList();
            selected = tmp;
        }

        private static void StopPoints()
        {
            //(.)(.)
        }

        static void SelectStatGrid(String route)
        {
            if (route == "all")
            {
                selected = data.ToList();
            }
            else
            {
                int id = routeIdMap[route];
                selected = data.Where(e => e.RouteID == id).ToList();
            }
        }
        static void SelectStatGrid(String route, TimeSpan dateBegin, TimeSpan dateEnd)
        {
            int id = routeIdMap[route];
            selected = data.Where(a => a.time.TimeOfDay <= dateEnd && a.time.TimeOfDay >= dateBegin && a.RouteID == id).ToList();
        }

        static void ShowGrid()
        {
            if (selected != null || selected.Count == 0)
                new StatGrid(selected).ShowDialog();
            else
                Console.WriteLine("Текущая выборка пуста");
        }

    }
}
