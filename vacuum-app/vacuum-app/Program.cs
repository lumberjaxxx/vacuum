
namespace VacuumCleaner
{
     public class Map
     {
         private enum CellType { Empty, Dirt, Obstacle, Cleaned };
         private CellType[,] _grid;
         public int Width { get; private set; }
         public int Height { get; private set; }

         public Map(int width, int height)
            {
                this.Width = width;
                this.Height = height;
                _grid = new CellType[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        _grid[x, y] = CellType.Empty;
                    }
                }
            }

            public bool IsInBounds(int x, int y)
            {
                return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
            }

            public bool IsDirt(int x, int y)
            {
                return IsInBounds(x, y) && _grid[x, y] == CellType.Dirt;
            }

            public bool IsObstacle(int x, int y)
            {
                return IsInBounds(x, y) && _grid[x, y] == CellType.Obstacle;
            }

            public void AddObstacle(int x, int y)
            {
                _grid[x, y] = CellType.Obstacle;
            }
            public void AddDirt(int x, int y)
            {
                _grid[x, y] = CellType.Dirt;
            }
            public void Clean(int x, int y)
            {
                if (IsInBounds(x, y))
                {
                    _grid[x, y] = CellType.Cleaned;
                }
            }
            public void Display(int robotX, int robotY)
            {
                //display 2d grid, location xy of the robot
                Console.Clear();
                Console.WriteLine("Vacuum cleaner robot simulation");
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Legend: #=Obstacle, D=Dirt, .=Empty, R=RObot, C=Cleaned");

                //loop for display grid
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        if (x == robotX && y == robotY)
                        {
                            Console.Write("R ");
                        }
                        else
                        {
                            switch (_grid[x, y])
                            {
                                case CellType.Empty: Console.Write(". "); break;
                                case CellType.Dirt: Console.Write("D "); break;
                                case CellType.Obstacle: Console.Write("# "); break;
                                case CellType.Cleaned: Console.Write("C "); break;
                            }
                        }
                    }
                    Console.WriteLine();
                }//add delay
            Thread.Sleep(100);
            }
        }
    public interface IStrategy
    {
        void Clean(Robot robot);
    }

    public class DefaultStrategy: IStrategy
    {
        public void Clean(Robot robot)
        {
            Console.WriteLine("Start cleaning the room");
            //flag default direction
            int direction = 1; // -1 if left
            for (int y = 0; y < robot.Map.Height; y++)
            {
                int startX = (direction == 1) ? 0 : robot.Map.Width - 1;
                int endX = (direction == 1) ? robot.Map.Width : -1;

                for (int x = startX; x != endX; x += direction)
                {
                    robot.Move(x, y);
                    robot.CleanCurrentSpot();
                }
                direction *= -1;
            }
        }
        
    }

    public class ReverseStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
            Console.WriteLine("Start cleaning the room");
            //flag default direction
            int direction = -1; // -1 if left
            for (int y = robot.Map.Height; y >=0; y--)
            {
                int startX = (direction == 1) ? 0 : robot.Map.Width - 1;
                int endX = (direction == 1) ? robot.Map.Width : -1;

                for (int x = startX; x != endX; x += direction)
                {
                    robot.Move(x, y);
                    robot.CleanCurrentSpot();
                }
                direction *= 1;
            }
        }
    }

    public class VerticalStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
                Console.WriteLine("Start cleaning the room");
                //flag default direction
                int direction = 1; // -1 if left
                for (int x = 0; x < robot.Map.Width; x++)
                {
                    int startY = (direction == 1) ? 0 : robot.Map.Height - 1;
                    int endY = (direction == 1) ? robot.Map.Height : -1;

                    for (int y = startY; y != endY; y += direction)
                    {
                        robot.Move(x, y);
                        robot.CleanCurrentSpot();
                    }
                    direction *= -1;
                }

        }
    }
    public class SpiralStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
            Console.WriteLine("Start Cleaning the Room");

            int top = 0;
            int bottom = robot.Map.Height - 1;
            int left = 0;
            int right = robot.Map.Width - 1;

            while ( top <= bottom && left <= right)
            {
                //left to right accross top
                for (int x = left; x <= right; x++)
                {
                    robot.Move(x, top);
                    robot.CleanCurrentSpot();
                }
                top++;

                // top to bottom down right

                for (int y = top; y <= bottom; y++)
                {
                    robot.Move(right, y);
                    robot.CleanCurrentSpot();
                }
                right--;

                //right to left across bottom
                if (top <= bottom)
                {
                    for (int x = right; x >= left; x--)
                    {
                        robot.Move(x, bottom);
                        robot.CleanCurrentSpot();
                    }
                    bottom--;
                }

                //bottom to top up right
                if ( left <= right)
                {
                    for (int y = bottom; y >= top; y--)
                    {
                        robot.Move(left, y);
                        robot.CleanCurrentSpot();
                    }
                    left++;
                }
            }
        }
    }
    public class Robot
    {
        private readonly Map _map;
        private readonly IStrategy _strategy;
        public int X { get; set; }
        public int Y { get; set; }

        public Map Map { get { return _map; } }
        
        public Robot(Map map, IStrategy strategy)
        {
            this._map = map;
            this._strategy = strategy;
            this.X = 0;
            this.Y = 0;
        }
        public bool Move(int newX, int newY)
        {
            if(_map.IsInBounds(newX,newY) && !_map.IsObstacle(newX, newY))
            {
                //set new location
                
                this.X = newX;
                this.Y = newY;

                //display robot location on grid
                _map.Display(this.X, this.Y);
                   return true;
            }
            else
            {
                //cannot move
                return false;
            }
        }//move method
        public void CleanCurrentSpot()
        {
            if (_map.IsDirt(this.X, this.Y))
            {
                _map.Clean(this.X, this.Y);
                _map.Display(this.X, this.Y);
            }
        }
        public void StartCleaning()
        {
            _strategy.Clean(this);
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initialize Vacuum Cleaner!");

            Map myMap = new Map(10, 10);

            Console.WriteLine($"Grid Width is {myMap.Width}");
            Console.WriteLine($"Grid Height is {myMap.Height}");

            //random dirt and obstacle
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                int x = random.Next(0, myMap.Width);
                int y = random.Next(0, myMap.Height);
                myMap.AddObstacle(x, y);

            }
            for (int i = 0; i < 20; i++)
            {
                int x = random.Next(0, myMap.Width);
                int y = random.Next(0, myMap.Height);
                myMap.AddDirt(x, y);

            }

            myMap.Display(10,9);

            IStrategy strat = new VerticalStrategy();

            Robot rob = new Robot(myMap, strat);

            rob.StartCleaning();

            Console.WriteLine("Done");
             
        }

    }
}
