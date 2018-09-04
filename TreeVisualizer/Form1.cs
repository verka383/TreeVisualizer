using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeVisualizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Font font;
        Bitmap image;
        Graphics g;
        Node actual;
        int maxX = 1000;
        int maxY = 1000;
        int circleSize = 40; 

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Text = fileName;
                Node.maxDepth = -1;
                Run();
                //pictureBox1 = new PictureBox();
                nodes.Clear();
                Run();
            }catch(FileNotFoundException)
            {
                MessageBox.Show("Soubor " + fileName + " nebyl nalezen. Zkuste jiný soubor.");
            }
        }

        string fileName = "vstup.txt";

        private void Run()
        {
            StreamReader sr = new StreamReader(fileName);
            int vstup;
            actual = new Node(null, true);
            actual.from = 0;
            actual.to = 2* maxX;
            actual.x = maxX;
            actual.y = (int)(-0.5 * Node.height);
            actual.fake = true;
            actual.depth = -1;
            string number = "";

            while ((vstup = sr.Read()) != -1)
            {
                char c = (char)vstup;
                
                switch (c)
                {
                    case '(':
                        if (number != "")
                        {
                            setValue(number);
                            number = "";
                        }
                        if  (actual.doleva)
                        {
                            actual.left = new Node(actual, true);
                            actual.doleva = false;
                            actual = actual.left;
                            nodes.Add(actual);
                        }
                        else
                        {
                            actual.right = new Node(actual, false);
                            actual = actual.right;
                            nodes.Add(actual);
                        }
                        
                        break;
                    case ')':
                        if (number != "")
                        {
                            setValue(number);
                            number = "";
                        }
                        actual = actual.parent;
                        break;
                    default:
                        number += c;
                        break;
                }
            }
            actual = actual.left;
            sr.Close();
            maxX = ((int)(Math.Pow(2, Node.maxDepth) + 3)) * (Node.width + 2* circleSize);
            maxY = (Node.maxDepth + 2) * Node.height;

            InitializeBitmap();
            DrawNodes();
            pictureBox1.Invalidate();
        }

        Brush nodeColor = Brushes.DarkOrange;
        Pen lineColor = Pens.Black;

        // Draw one node
        private void setValue(string number)
        {
            
            actual.doleva = false;

            if (number.Length == 1)
            {
                number = "  " + number;
            }
            else if (number.Length == 2)
            {
                number = " " + number;
            }

            actual.value = number;

        }

        public void InitializeBitmap()
        {
            image = new Bitmap(maxX, maxY);
            pictureBox1.Width = maxX;
            pictureBox1.Height = maxY;
            g = Graphics.FromImage(image);
            pictureBox1.Image = image;

            FontFamily fontFamily = new FontFamily("Arial");
            font = new Font(
               fontFamily,
               20,
               FontStyle.Bold,
               GraphicsUnit.Pixel);
        }

        List<Node> nodes = new List<Node>();

        public void DrawNodes()
        {
            if (nodes.Count == 1)
            {
                Node n = nodes[0];
                g.FillEllipse(nodeColor, n.x, n.y, circleSize, circleSize);
                g.DrawString(n.value.ToString(), font, Brushes.White, new Point((int)(n.x + (0 * circleSize)), (int)(n.y + (0.24 * circleSize))));
            }
            else
            {
                foreach (Node n in nodes)
                {
                    if (!n.parent.fake)
                    {
                        g.DrawLine(lineColor, new Point((int)(n.x + (0.5 * circleSize)), (int)(n.y + (0.5 * circleSize))), new Point((int)(n.parent.x + (0.5 * circleSize)), (int)(n.parent.y + (0.5 * circleSize))));
                        g.FillEllipse(nodeColor, n.x, n.y, circleSize, circleSize);
                        g.DrawString(n.value.ToString(), font, Brushes.White, new Point((int)(n.x + (0 * circleSize)), (int)(n.y + (0.24 * circleSize))));
                        g.FillEllipse(nodeColor, n.parent.x, n.parent.y, circleSize, circleSize);
                        g.DrawString(n.parent.value.ToString(), font, Brushes.White, new Point((int)(n.parent.x + (0.0 * circleSize)), (int)(n.parent.y + (0.24 * circleSize))));
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
            }
        }
    }


    class Node
    {
        public static int height = 70;
        public static int width = 50;
        public static int maxDepth = -1;

        public Node(Node parent, bool left)
        {
            this.parent = parent;

            if (parent != null)
            {
                this.depth = parent.depth + 1;
                if (this.depth > maxDepth)
                {
                    maxDepth = this.depth;
                }

                if (left)
                {
                    this.from = parent.from;
                    this.to = parent.x;
                    this.x = this.from + ((this.to - this.from) / 2);
                    this.x -= 40;
                }
                else
                {
                    this.from = parent.x;
                    this.to = parent.to;
                    this.x = this.from + ((this.to - this.from) / 2);
                    this.x += 40;
                }

                
                this.y = parent.y + height;
            }
        }

        public string value;
        public Node left;
        public Node right;
        public Node parent;

        public float x;
        public float y;
        public float from;
        public float to;
        public bool doleva = true;
        public bool fake = false;
        public int depth;
    }
}
