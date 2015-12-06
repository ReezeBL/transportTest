using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace transportTest
{
    public partial class StatGrid : Form
    {
        List<Program.Waypoint> data;
        Program.Waypoint beg = new Program.Waypoint(11038.08464497, 8253.17542416);
        Program.Waypoint end = new Program.Waypoint(283.08479678, 163.45489494);
        private void clearArea()
        {
            foreach(var s in chart1.Series)
            {
                s.Points.Clear();
            }
        }

        public StatGrid()
        {
            InitializeComponent();
        }
        public StatGrid(List<Program.Waypoint> data)
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            this.data = data;
            infoBox1.Text = String.Format("Выборка за {0} - {1}", data.First().time, data.Last().time);
            chart1.Series[3].Color = Color.Red;
            chart1.Series[2].Color = Color.Green;
            clearArea();
        }

        private void StatGrid_SizeChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearArea();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series[1].Points.AddXY(beg.X, beg.Y);
            chart1.Series[1].Points.AddXY(end.X, end.Y);
            data.ForEach(a => chart1.Series[0].Points.AddXY(a.X, a.Y));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearArea();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;          
            data.ForEach(a => chart1.Series[0].Points.AddXY(a.time, a.Distance(beg)));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearArea();
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            Program.Waypoint prev = data.First();
            data.ForEach(a => { chart1.Series[0].Points.AddXY(prev.time, Program.Velocity.CalculateVelocity(prev, a).v); prev = a; });
        }
    }
}
