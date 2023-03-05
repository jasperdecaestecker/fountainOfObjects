var game = new FountainOfObjectsGame();
var world = new World();
var player = new Player();
bool gameRunning = true;
bool fountainActivated = false;
game.showStartScreen();
while (game.GameIsRunning)
{
    // Console.Clear();
    Console.ForegroundColor = ConsoleColor.White;

    ConsolePrinter.printCurrentStatus(world, player);
    game.checkWinCondition(world, player);
}

public class FountainOfObjectsGame
{
    public bool GameIsRunning;
    public FountainOfObjectsGame()
    {
        GameIsRunning = true;
    }

    public void checkWinCondition(World world, Player player)
    {
        var roomWherePlayerIs = world.Rooms[player.currentPosition.Row, player.currentPosition.Column];
        if (roomWherePlayerIs.RoomType == RoomType.Entrance && world.fountainActivated)
        {
            GameIsRunning = false;
            Console.WriteLine("You made it out alive");
        }
    }

    public void showStartScreen()
    {
        Console.WriteLine("You are at the entrance of this cave. Find the fountain of objects and activate it!");
    }
}

public class ConsolePrinter
{
    public static void printCurrentStatus(World world, Player player)
    {
        // Console.WriteLine($"You are in the room at {player.currentPosition.ToString()}");
 
        var room = world.Rooms[player.currentPosition.Row, player.currentPosition.Column];
        room.Visited = true;
        world.printRooms(player);
        Console.WriteLine(room.getRoomTypeDescription());
        var action = player.getAction();
        if (action == "move north" && player.currentPosition.Row != 0)
        {
            player.currentPosition = new Position(player.currentPosition.Row - 1, player.currentPosition.Column);
        }
        else if (action == "move south" && player.currentPosition.Row != 3)
        {
            player.currentPosition = new Position(player.currentPosition.Row + 1, player.currentPosition.Column);

        } else if (action == "move west" && player.currentPosition.Column != 0)
        {
            player.currentPosition = new Position(player.currentPosition.Row, player.currentPosition.Column -1);
        } else if (action == "move east" && player.currentPosition.Column != 3)
        {
            player.currentPosition = new Position(player.currentPosition.Row, player.currentPosition.Column + 1);
        } else if (action == "enable fountain" && room.RoomType == RoomType.Fountain)
        {
            world.fountainActivated = true;
            Console.WriteLine("Fountain activated, now lets get out of here!");
        }
        else
        {
            Console.WriteLine("You cannot do this right now..");
        }
    }
}
public class World
{
    public Room[,] Rooms = new Room[4,4];
    public bool fountainActivated;
    public World()
    {
        MakeWorld();
    }

    public void MakeWorld()
    {
        for (int row = 0; row < Rooms.GetLength(0); row++)
        {
            for (int column = 0; column < Rooms.GetLength(1); column++)
            {
                if (row == 0 && column == 0)
                {
                    Rooms[row, column] = new Room(RoomType.Entrance);
                } else if (row == 0 && column == 2)
                {
                    Rooms[row, column] = new Room(RoomType.Fountain);
                }
                else
                {
                    Rooms[row, column] = new Room(RoomType.Empty);
                }
            } 
        }
    }

    public void printRooms(Player player)
    {
        for (int row = 0; row < Rooms.GetLength(0); row++)
        {
            for (int column = 0; column < Rooms.GetLength(1); column++)
            {
                var room = Rooms[row, column];

                if (player.currentPosition.Column == column && player.currentPosition.Row == row)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;

                } 
                
                if (room.RoomType == RoomType.Entrance && room.Visited)
                {
                    Console.Write("[E]");
                } else if (room.RoomType == RoomType.Fountain && room.Visited)
                {
                    Console.Write("[F]");
                }
                else if (room.RoomType == RoomType.Empty && room.Visited)
                {
                    Console.Write("[X]");
                }
                else
                {
                    Console.Write("[ ]");
                }
            }
            Console.WriteLine();
        }
    }
}
public class Room
{
    public RoomType RoomType { get; }
    public bool Visited;

    public Room(RoomType roomType)
    {
        RoomType = roomType;
        Visited = false;
    }

    public string getRoomTypeDescription()
    {
        return RoomType switch
        {
            RoomType.Entrance => "You see light coming from the cavern entrance",
            RoomType.Fountain => "You hear water dripping in this room. The Fountain of Objects is here!",
            _ => ""
        };
    }
}

public class Player
{
    public Position currentPosition = new Position(0,0);

    public string getAction()
    {
        Console.Write("What do you want to do? ");
        string action = Console.ReadLine();
        return action;
    }
}

public readonly struct Position
{
    public Position(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public int Row { get; }
    public int Column { get; }
    public override string ToString() => $"(Row={Row}, Column={Column})";
}

public enum RoomType {Empty, Fountain, Entrance}