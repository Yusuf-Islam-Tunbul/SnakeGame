using System;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

namespace SnakeGame
{
    class Program
    {
        static Program prog= new Program();
        static bool is_playing = false;

        int time;
        List<Node> nodes = new List<Node>();
        Point bait = new Point();
        int[] x_boundaries = new int[2];
        int[] y_boundaries = new int[2];
        int horizontal_interval;
        int vertical_interval;
        int interval;
        int score;

        Point up = new Point(0, -1);
        Point down = new Point(0, 1);
        Point left = new Point(-1, 0);
        Point right = new Point(1, 0);

        static void Main(string[] args)
        {            
            while (true)
            {
                while (!is_playing)
                {
                    Console.WriteLine("Press any key to start the game.");
                    Console.ReadKey();
                    is_playing = true;
                }

                prog.Initialize();

                while (is_playing)
                {
                    prog.Play();
                }

                prog.End();

                while (!is_playing)
                {
                    if (Console.ReadKey().Key == ConsoleKey.R)
                    {
                        is_playing = true;
                    }

                    else if (Console.ReadKey().Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            }           
        }

        void Initialize()
        {
            
            time = 0;
            nodes = new List<Node>();
            bait = new Point();
            x_boundaries[0] = 0;
            x_boundaries[1] = 111;
            y_boundaries[0] = 0;
            y_boundaries[1] = 27;
            horizontal_interval = 4000;
            vertical_interval = 6000;
            interval = horizontal_interval;
            score = 0;

            int width = x_boundaries[1] - x_boundaries[0];
            int height = y_boundaries[1] - y_boundaries[0];

            Console.SetWindowSize(width+1,height+1);
            Console.SetBufferSize(width+1,height+1);

            prog.CreateNode(new Point(1, 0), new Point(4, 12));
            prog.CreateNode();
            prog.CreateNode();
            prog.PlaceBait();
        }

        void Play()
        {
            if (Console.KeyAvailable)
            {
                prog.SelectDirection();
            }

            if (time >= interval)
            {
                prog.Move();
                prog.DetectCollision();
                prog.Draw();
                prog.CheckDeath();
                prog.ManageNodes();
                prog.EatBait();

                time = 0;
            }

            else
            {
                time++;
            }
        }

        void SelectDirection()
        {
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.W:

                    if (nodes[0].direction != down)
                    {
                        nodes[0].direction = up;
                        interval = vertical_interval;
                    }

                    break;

                case ConsoleKey.A:

                    if (nodes[0].direction != right)
                    {
                        nodes[0].direction = left;
                        interval = horizontal_interval;
                    }

                    break;

                case ConsoleKey.S:

                    if (nodes[0].direction != up)
                    {
                        nodes[0].direction = down;
                        interval = vertical_interval;
                    }

                    break;

                case ConsoleKey.D:

                    if (nodes[0].direction != left)
                    {
                        nodes[0].direction = right;
                        interval = vertical_interval;
                    }

                    break;

                default:

                    break;
            }
        }

        void Move()
        {
            foreach (Node node in nodes)
            {
                node.location = Point.Add(node.location,(Size)node.direction);
            }
        }

        void DetectCollision()
        {
            foreach (Node node in nodes)
            {
                if (node.location.X < x_boundaries[0])
                {
                    node.location.X = x_boundaries[1];
                }

                else if (node.location.X > x_boundaries[1])
                {
                    node.location.X = x_boundaries[0];
                }

                if (node.location.Y < y_boundaries[0])
                {
                    node.location.Y = y_boundaries[1];
                }

                else if (node.location.Y > y_boundaries[1])
                {
                    node.location.Y = y_boundaries[0];
                }
            }
        }

        void Draw()
        {
            Console.Clear();

            for (int i = y_boundaries[0]; i <= y_boundaries[1]; i++)
            {
                List<int> x_indexes = new List<int>();

                foreach (Node node in nodes)
                {
                    if (node.location.Y == i)
                    {
                        x_indexes.Add(node.location.X);
                    }
                }

                if (bait.Y == i)
                {
                    x_indexes.Add(bait.X);
                }

                if (x_indexes.Count == 0)
                {
                    Console.WriteLine(" ");
                }

                else
                {
                    string line = "";

                    for (int j = x_boundaries[0]; j <= x_boundaries[1]; j++)
                    {
                        if (!x_indexes.Contains(j))
                        {
                            line += " ";
                        }

                        else
                        {
                            line += "o";
                        }
                    }

                    Console.WriteLine(line);
                }
            }
        }

        void ManageNodes()
        {
            for (int i = nodes.Count - 1; i > 0; i--)
            {
                nodes[i].direction = nodes[i - 1].direction;
            }
        }

        void EatBait()
        {
            if (nodes[0].location==bait)
            {
                score++;
                CreateNode();
                PlaceBait();
            }
        }
        void CreateNode()
        {
            Node node = new Node();
            Point direction = nodes[nodes.Count - 1].direction;
            Point location = nodes[nodes.Count - 1].location;
            node.direction = direction;
            node.location = Point.Subtract(location, (Size)direction);
            nodes.Add(node);
        }

        void CreateNode(Point direction, Point location)
        {
            Node node = new Node();
            node.direction = direction;
            node.location = location;
            nodes.Add(node);
        }

        void PlaceBait()
        {
            bool available = false;
            Random rand = new Random();
            Point point = new Point();

            while (!available)
            {
                point = new Point(rand.Next(x_boundaries[0], x_boundaries[1]), rand.Next(y_boundaries[0], y_boundaries[1]));

                available = true;

                foreach (Node node in nodes)
                {
                    if (point == node.location)
                    {
                        available = false;
                    }
                }
            }

            bait = point;
        }

        void CheckDeath()
        {
            for(int i = 0; i <= nodes.Count - 2; i++)
            {
                for(int j = i+1; j <= nodes.Count - 1; j++)
                {
                    if (nodes[i].location == nodes[j].location)
                    {
                        is_playing = false;
                    }
                }
            }
        }

        void End()
        {
            Console.Clear();
            Console.WriteLine("You lost the game.");
            Console.WriteLine("Your score is " + score + ".");
            Console.WriteLine("Press R to restart.");
            Console.WriteLine("Press Esc to exit.");
        }
    }

    class Node
    {
        public Point location = new Point();
        public Point direction = new Point();
    }
}
