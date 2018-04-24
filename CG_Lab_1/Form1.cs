using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CG_Lab_1
{
    public partial class Form1 : Form
    {
        public Graphics graphics;

        private Pen grid_pen = new Pen(Color.Blue, 0.0001f);
        private Pen coord_axis_pen = new Pen(Color.Red, 10);
        private Pen graph_pen = new Pen(Color.Black, 5);
        private Font font = new Font("Arial", 8);
        private Brush brush = new SolidBrush(Color.Black);
        
        private const double STEP = 0.1;
        private const int GRAPH_SIZE = (int)((Math.PI) / STEP + 2);
        private Point[] graph = new Point[GRAPH_SIZE];

        private double step_grid;
        private double a;

        private int graphics_top;
        private int graphics_left;

        // Создание формы
        public Form1()
        {
            InitializeComponent();
            graphics_top = this.Height;
            graphics_left = this.Width / 3;
            this.Resize += new EventHandler(this.FormResize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Обработчик нажатия на кнопку
        private void button1_Click(object sender, EventArgs e)
        { 
            ReadParams();
            Redraw();
            trackBar1.Visible = true;
        }

        // Перерисовка графика
        private void Redraw()
        {
            InitGraphics();
            graphics.Clear(Color.White);

            // Масштабирование графика в зависимости от состояния ползунка
            float m = trackBar1.Value;
            if (m > 0) graphics.ScaleTransform(m, m);
            else if (m < 0) graphics.ScaleTransform((float)(-1 / m), (float)(-1 / m));

            DrawGrid();
            graphics.RotateTransform(-90f);
            DrawGraph();
        }

        // Считывание введенных данных
        private void ReadParams()
        {
            a = Convert.ToDouble(textbox_a.Text);
            step_grid = Convert.ToDouble(textbox_step_grid.Text);
        }

        // Рисование графика
        private void DrawGraph()
        {
            double i = -Math.PI / 2;
            for (int p=0; p < GRAPH_SIZE; p++)
            {
                i += STEP; // делаем шаг
                double y = X(a, i); // просчитываем координату по y
                double x = Y(a, i); // просчитываем координату по x
                graph[p] = new Point(Convert.ToInt32(x), Convert.ToInt32(y)); // заносим в массив точек
            }

            graphics.DrawCurve(graph_pen, graph); // рисуем
        }

        // Инициализация объекта graphics
        private void InitGraphics()
        {
            graphics = this.CreateGraphics();

            // Изменение размера области для рисования в соответствии с размерами окна
            graphics.SetClip(new Rectangle(graphics_left, 0, this.Width - graphics_left, this.Height));

            // Изменение направления осей координат
            graphics.TranslateTransform((this.Width + graphics_left)/2, this.Height / 2);
        }

        // Рисование сетки
        private void DrawGrid()
        {
            int left = Convert.ToInt32(graphics.ClipBounds.Left);
            int right = Convert.ToInt32(graphics.ClipBounds.Right);
            int bottom = Convert.ToInt32(graphics.ClipBounds.Bottom);
            int top = Convert.ToInt32(graphics.ClipBounds.Top);

            // Рисуем координатные оси
            graphics.DrawLine(coord_axis_pen, new Point(left, 0), new Point(right, 0));
            graphics.DrawLine(coord_axis_pen, new Point(0, bottom), new Point(0, top));

            double x = 0;
            double y = 0;
            
            // Пока не заполним весь прямоугольник
            while (x < Math.Max(graphics.ClipBounds.Right, graphics.ClipBounds.Bottom))
            {
                // Делаем шаг
                x += step_grid;
                y += step_grid;

                // Рисуем вертикальные линии (идем вправо и влево от центра)
                graphics.DrawLine(grid_pen, new Point(Convert.ToInt32(x), bottom), new Point(Convert.ToInt32(x), top));
                graphics.DrawString(Convert.ToString(x), font, brush, 0, (float)-y);
                graphics.DrawLine(grid_pen, new Point(Convert.ToInt32(-x), bottom), new Point(Convert.ToInt32(-x), top));
                graphics.DrawString(Convert.ToString(-x), font, brush, 0, (float)y);

                // Рисуем горизонтальные линии (вверх и вниз от центра)
                graphics.DrawLine(grid_pen, new Point(left, Convert.ToInt32(y)), new Point(right, Convert.ToInt32(y)));
                graphics.DrawString(Convert.ToString(y), font, brush, (float)x, 0);
                graphics.DrawLine(grid_pen, new Point(left, Convert.ToInt32(-y)), new Point(right, Convert.ToInt32(-y)));
                graphics.DrawString(Convert.ToString(-y), font, brush, (float)-x, 0);
            }
        }

        // Высчитываем координату по Х
        private double X(double a, double t)
        {
            double tg = Math.Tan(Math.PI / 4 - t);
            double p;
            if (tg >= 0) p = Math.Sqrt(tg);
            else p = -Math.Sqrt(-tg);
            return a * Math.Sqrt(2) * ((p + p * p * p) / (1 + p * p * p * p));
        }

        // Высчитываем координату по Y
        private double Y(double a, double t)
        {
            double tg = Math.Tan(Math.PI / 4 - t);
            double p;
            if (tg >= 0) p = Math.Sqrt(tg);
            else p = -Math.Sqrt(-tg);
            return a * Math.Sqrt(2) * ((p - p * p * p) / (1 + p * p * p * p));
        }

        // Обработчик ползунка
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        // Обработчик изменения размера окна
        private void FormResize(object sender, EventArgs e)
        {
            Redraw();
        }
    }
}
