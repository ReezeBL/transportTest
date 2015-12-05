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
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {           
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        static Waypoint beg = new Program.Waypoint(11038.08464497, 8253.17542416);
        static Waypoint end = new Program.Waypoint(283.08479678, 163.45489494);

        static SortedDictionary<String, List<Waypoint>> data = new SortedDictionary<string, List<Waypoint>>();
        static List<Waypoint> selected = null;
        static void Main(string[] args)
        {
           
            String[] lines = File.ReadAllLines("train.txt");
            Console.WriteLine("Файл с данными усмпешно прочитан. Найдено {0} элементов.", lines.Length);           
            foreach (String line in lines)
            {
                String[] elems = line.Split('\t');
                Waypoint w = new Waypoint();
                w.X = Double.Parse(elems[1], CultureInfo.InvariantCulture);
                w.Y = Double.Parse(elems[2], CultureInfo.InvariantCulture);
                w.time = UnixTimeStampToDateTime(Double.Parse(elems[0], CultureInfo.InvariantCulture));
                if (data.ContainsKey(elems[3]))
                {
                    List<Waypoint> tmp = data[elems[3]];
                    tmp.Add(w);
                }
                else
                {
                    List<Waypoint> tmp = new List<Waypoint>();
                    tmp.Add(w);
                    data.Add(elems[3], tmp);
                }
            }
            foreach(List<Waypoint> l in data.Values)
            {
                l.Sort((a, b) => { return a.time.CompareTo(b.time); });
            }
            Console.WriteLine("Обработка данных завершена. Всего транспортных средств зарегестрировано: {0}", data.Keys.Count);
            foreach(String n in data.Keys)
            {
                Console.WriteLine("\t{0}:{1}", n, data[n].Count);
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
            List<Waypoint> tmp = new List<Waypoint>();
            Waypoint prev = null;
            foreach (Waypoint w in selected)
            {
                if (prev == null)
                {
                    prev = w;
                    continue;
                }

                //Градиентное отсеивание

                //double gradY = w.Y - prev.Y, gradX = w.X - prev.X;
                //if ((gradY < 50) && (gradX < 10) && w.Y <= beg.Y + 100.0)
                //    tmp.Add(w);

                //Дистанционнное отсеивание

                if (w.Distance(beg) > prev.Distance(beg) && w.Y < (beg.Y + 100))
                    tmp.Add(w);
                prev = w;
            }
            selected = tmp;
        }

        private static void StopPoints()
        {
            //(.)(.)
        }

        static void SelectStatGrid(String route)
        {
            List<Waypoint> tmp;
            if (route == "all")
            {
                tmp = new List<Waypoint>();
                foreach (List<Waypoint> l in data.Values)
                    tmp.AddRange(l);
                selected = tmp;
            }
            else
            {
                if (data.TryGetValue(route, out tmp))
                {
                    selected = tmp;
                }
                else
                {
                    Console.WriteLine("Неопознаный идентификатор транспортного средства.");
                    selected = null;
                }
            }
        }
        static void SelectStatGrid(String route, TimeSpan dateBegin, TimeSpan dateEnd)
        {
            List<Waypoint> tmp;
            if (data.TryGetValue(route, out tmp))
            {
                selected = tmp.Where(a => a.time.TimeOfDay <= dateEnd && a.time.TimeOfDay >= dateBegin).ToList();
            }
            else
            {
                Console.WriteLine("Неопознаный идентификатор транспортного средства.");
                selected = null;
            }
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
