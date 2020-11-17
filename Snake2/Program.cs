using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Snake2
{
    internal class Game
    {
        private enum Direction
        { Up, Down, Left, Right };

        private struct PassageCoordinates
        {
            public PassageCoordinates(int x1, int x2, int y1, int y2)
            {
                X1 = x1;
                X2 = x2;
                Y1 = y1;
                Y2 = y2;
            }

            public int X1 { get; }
            public int X2 { get; }
            public int Y1 { get; }
            public int Y2 { get; }

        };

        public Game(int _Size)
        {
            this.Size = _Size + _Size%2;
            this.Snake = new Queue<Vector2>();
            ActualClaster = 1;

            this.Snake.Enqueue(new Vector2(1,1));
            this.Snake.Enqueue(new Vector2(1,2));
            this.Snake.Enqueue(new Vector2(1,3));

            LoadMap();
            SetPassages();
            DrawPassage(Claster[ActualClaster]);
            Fruit();
            SnakeRendering();
        }

        private static readonly Dictionary<char, Direction> DirectionType = new Dictionary<char, Direction>()
        {
            { 'W', Direction.Up },
            { 'S', Direction.Down },
            { 'A', Direction.Left },
            { 'D', Direction.Right }
        };

        private static readonly Dictionary<char, Direction> DirectionTypeReverse = new Dictionary<char, Direction>()
        {
            { 'S', Direction.Up },
            { 'W', Direction.Down },
            { 'D', Direction.Left },
            { 'A', Direction.Right }
        };

        private static readonly Dictionary<int, int> Claster = new Dictionary<int, int>()
        {
            { 1, 31 },
            { 2, 30 },
            { 3, 21 },
            { 4, 20 }
        };

        public bool ShiftControl()
        {
            DirectionType.TryGetValue(c, out var dir);
            DirectionTypeReverse.TryGetValue(c, out var rdir);

            if(this.direction != rdir)
                this.direction = dir;


            if (Snake.Peek().X >= 0 && Snake.Peek().X < 2 * Size && Snake.Peek().Y >= 0 && Snake.Peek().Y < Size)
            {
                Console.SetCursorPosition((int)Snake.Peek().X, (int)Snake.Peek().Y);
                Console.Write(' ');
            }
            Snake.Dequeue();

            if (ActualClaster == fruit.Z)
            {
                if (head.X == fruit.X && head.Y == fruit.Y)
                {
                    Fruit();
                    Console.SetCursorPosition((int)head.X, (int)head.Y); 
                    Snake.Enqueue(new Vector2(head.X, head.Y));
                }
                
            }


            switch (this.direction)
            {
                case Direction.Up: return MoveUp();
     
                case Direction.Down: return MoveDown();                    
    
                case Direction.Left: return MoveLeft();
                   
                case Direction.Right: return MoveRight();                    
            }

            return false;
        }

        private bool MoveUp()
        {
            bool HasPassed = false;

            if (ActualClaster == 3 || ActualClaster == 4)
            {
                if ((this.head.Y - 1 == 0 && this.head.X == Passages[2].X1)
                    || (this.head.Y - 1 == 0 && this.head.X == Passages[2].X2))
                {
                    EraseFruit();

                    if (ActualClaster == 3)
                    {
                        ErasePassage(Claster[ActualClaster]);
                        
                        ActualClaster = 1;                
                        DrawPassage(Claster[ActualClaster]);
                    }
                    else
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 2;                       
                        DrawPassage(Claster[ActualClaster]);
                    }

                    DrawFruit();

                    foreach (var segment in Snake)
                    {
                        Console.SetCursorPosition((int)segment.X, (int)segment.Y);
                        Console.Write(' ');
                    }

                    this.head.Y += (Size - 1);

                    HasPassed = true;

                }
            }

            if (!HasPassed && this.head.Y - 1 == 0)
            {
                GameOn = false;
                return false;
            }
            else
            {
                Snake.Enqueue(new Vector2(head.X, --head.Y));
                Console.SetCursorPosition((int)head.X, (int)head.Y);
                Console.Write('o');
            }

            return true;
        }

        private bool MoveDown()
        {
            bool HasPassed = false;

            if(ActualClaster == 1 || ActualClaster == 2)
            {
                if ((this.head.Y + 1 == Size - 1 && this.head.X == Passages[3].X1)
                     || (this.head.Y + 1 == Size - 1 && this.head.X == Passages[3].X2))
                {
                    EraseFruit();

                    if (ActualClaster == 1)
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 3;
                        DrawPassage(Claster[ActualClaster]);
                    }
                    else
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 4;
                        DrawPassage(Claster[ActualClaster]);
                    }

                    DrawFruit();

                    foreach (var segment in Snake)
                    {
                        Console.SetCursorPosition((int)segment.X, (int)segment.Y);
                        Console.Write(' ');
                    }

                    this.head.Y -= (Size-1);

                    HasPassed = true;
                }
            }
           
            
            if (!HasPassed && this.head.Y + 1 == Size-1)
            {
                GameOn = false;
                return false;
            }
            else
            {
                Snake.Enqueue(new Vector2(head.X, ++head.Y));
                Console.SetCursorPosition((int)head.X, (int)head.Y);
                Console.Write('o');
            }

            return true;
        }

        private bool MoveLeft()
        {
            bool HasPassed = false;

            if (ActualClaster == 2 || ActualClaster == 4)
            {
                if ((this.head.Y == Passages[0].Y1 && this.head.X - 1 == 0)
                   || (this.head.Y == Passages[0].Y2 && this.head.X - 1 == 0))
                {
                    EraseFruit();

                    if (ActualClaster == 2)
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 1;
                        DrawPassage(Claster[ActualClaster]);
                    }
                    else
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 3;
                        DrawPassage(Claster[ActualClaster]);
                    }

                    DrawFruit();

                    foreach (var segment in Snake)
                    {
                        Console.SetCursorPosition((int)segment.X, (int)segment.Y);
                        Console.Write(' ');
                    }

                    this.head.X += (2*Size - 1);

                    HasPassed = true;
                }
            }


            if (!HasPassed && this.head.X - 1 == 0)
            {
                GameOn = false;
                return false;
            }
            else
            {
                Snake.Enqueue(new Vector2(--head.X, head.Y));
                Console.SetCursorPosition((int)head.X, (int)head.Y );
                Console.Write('o');
            }

            return true;
        }

        private bool MoveRight()
        {
            bool HasPassed = false;

            if (ActualClaster == 1 || ActualClaster == 3)
            {
                if (this.head.Y == Passages[1].Y1 && this.head.X + 1 == 2 * Size - 1
                || this.head.Y == Passages[1].Y2 && this.head.X + 1 == 2 * Size - 1)
                {
                    EraseFruit();

                    if (ActualClaster == 1)
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 2;
                        DrawPassage(Claster[ActualClaster]);
                    }
                    else
                    {
                        ErasePassage(Claster[ActualClaster]);

                        ActualClaster = 4;
                        DrawPassage(Claster[ActualClaster]);
                    }

                    DrawFruit();

                    foreach (var segment in Snake)
                    {
                        Console.SetCursorPosition((int)segment.X, (int)segment.Y);
                        Console.Write(' ');
                    }

                    this.head.X -= (2 * Size - 1);

                    HasPassed = true;
                }
            }

            if (!HasPassed && this.head.X + 1 == 2*Size-1)
            {
                GameOn = false;
                return false;
            }
            else
            {
                Snake.Enqueue(new Vector2(++head.X, head.Y));
                Console.SetCursorPosition((int)head.X, (int)head.Y);
                Console.Write('o');
            }

            return true;
        }

        private void SetPassages()
        { 
            Passages = new List<PassageCoordinates>();

            Passages.Add(new PassageCoordinates(0, 0, Size / 2 - 1, Size / 2));   //Left0
            Passages.Add(new PassageCoordinates(2*Size - 1, 2*Size - 1, Size / 2 - 1, Size / 2));   //Right1
            Passages.Add(new PassageCoordinates(Size - 1, Size, 0, 0)); //Up2
            Passages.Add(new PassageCoordinates(Size - 1, Size, Size - 1, Size - 1)); //Dow3
        }

        private void DrawPassage(int clasterid)
        {
            while(clasterid != 0)
            {
                Console.SetCursorPosition(Passages[clasterid % 10].X1, Passages[clasterid % 10].Y1);
                Console.Write(' ');
                Console.SetCursorPosition(Passages[clasterid % 10].X2, Passages[clasterid % 10].Y2);
                Console.Write(' ');

                clasterid /= 10;
            }
        }

        private void ErasePassage(int clasterid)
        {
            while (clasterid != 0)
            {
                Console.SetCursorPosition(Passages[clasterid % 10].X1, Passages[clasterid % 10].Y1);
                Console.Write('#');
                Console.SetCursorPosition(Passages[clasterid % 10].X2, Passages[clasterid % 10].Y2);
                Console.Write('#');

                clasterid /= 10;
            }
        }

        private void Fruit()
        {
            var rnd = new Random();

            fruit.X = rnd.Next(1, 2 * Size - 1);
            fruit.Y = rnd.Next(1, Size - 1);
            fruit.Z = rnd.Next(1, 4);
        }

        private void EraseFruit()
        {
            if(ActualClaster == fruit.Z)
            {
                Console.SetCursorPosition((int)fruit.X, (int)fruit.Y);
                Console.Write(' ');
            }
        }

        private void DrawFruit()
        {
            if (ActualClaster == fruit.Z)
            {
                Console.SetCursorPosition((int)fruit.X, (int)fruit.Y);
                Console.Write('F');
            }
        }

        private void LoadMap()
        {
            for(int j=0; j<2*Size; j++)
            {
                Console.SetCursorPosition(j, 0);
                Console.Write('#');
            }

            for(int i=1; i<Size; i++)
            {
                for(int j=0; j<=2*Size-1; j+=(2*Size-1))
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write('#');
                }
            }

            for (int j = 0; j < 2*Size; j++)
            {
                    Console.SetCursorPosition(j, Size - 1 );
                    Console.Write('#');
            }


        }
        
        private void SnakeRendering()
        {
            foreach(var element in Snake)
            {
                Console.SetCursorPosition((int)element.X, (int)element.Y);
                Console.Write('o');
            }
        }

        public void SnakeThread()
        {            
            while(GameOn = ShiftControl())
            {
                Console.SetCursorPosition(0, Size+1);

                if(this.direction == Direction.Up || this.direction == Direction.Down)
                    Thread.Sleep(130);
                else
                    Thread.Sleep(90);
            }

        }

        private int ActualClaster;
        private Vector2 head = new Vector2(1, 3);
        public bool GameOn = true;
        public char c = 'S';

        private Queue<Vector2> Snake;
        private List<PassageCoordinates> Passages { get; set; }
        public int Size { get; }

        private Vector3 fruit = new Vector3(0,0,0);

        private Direction direction = Direction.Down;
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            var GameOb = new Game(10);

            
            Thread t = new Thread(new ThreadStart(GameOb.SnakeThread));

            t.Start();

            while(GameOb.GameOn)
            {
                Console.SetCursorPosition(0, GameOb.Size + 1);
                GameOb.c = char.ToUpper(Console.ReadKey().KeyChar);
            }

            t.Join();        

            Console.SetCursorPosition(0, GameOb.Size + 1);

 

        }
    }
}
