// Small, Medium and Large
// Amaroks
// Getting Armed
// Getting Help

var game = new FountainOfObjectsGame();
var player = new Player();
var world = new World(player);
game.showStartScreen();
world.showPickWorldSize();
while (game.GameIsRunning)
{
    Console.ForegroundColor = ConsoleColor.White;
    ConsolePrinter.printCurrentStatus(world, player, game);
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
        Console.WriteLine("You enter the Cavern of Objects, a maze of in search for the Fountain of Objects");
        Console.WriteLine("Amaroks roam the caverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.");
        Console.WriteLine("You carry with you a bow and a quiver of arrows. You can shoot the monsters in the cavern but beware you only have a limited supply");
        Console.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the caverns");
        Console.WriteLine("You must navigate the caverns with your other senses");
        Console.WriteLine("Find the Fountain of Objects, activate it, and return to the entrance.");
    }

    public void showHelp()
    {
        Console.WriteLine("Available Actions");
        Console.WriteLine("W/move north: move north");
        Console.WriteLine("S: Move south");
        Console.WriteLine("A: Move west");
        Console.WriteLine("D: Move east");
        Console.WriteLine("enable fountain: Activates the fountain");
        Console.WriteLine("shoot north: shoot arrow to the north");
        Console.WriteLine("shoot south: shoot arrow to the south");
        Console.WriteLine("shoot west: shoot arrow to the west");
        Console.WriteLine("shoot east: shoot arrow to the east");
    }
}

public class ConsolePrinter
{
    public static void printCurrentStatus(World world, Player player, FountainOfObjectsGame game)
    {
        var room = world.Rooms[player.currentPosition.Row, player.currentPosition.Column];
        room.Visited = true;
        world.printRooms(player);
        Console.WriteLine(room.getRoomTypeDescription());
        var action = player.getAction();
        if (action == "move north" && player.currentPosition.Row != 0)
        {
            player.currentPosition = new Position(player.currentPosition.Row - 1, player.currentPosition.Column);
        }
        else if (action == "move south" && player.currentPosition.Row != world.Rooms.GetLength(0) - 1)
        {
            player.currentPosition = new Position(player.currentPosition.Row + 1, player.currentPosition.Column);

        } else if (action == "move west" && player.currentPosition.Column != 0)
        {
            player.currentPosition = new Position(player.currentPosition.Row, player.currentPosition.Column -1);
        } else if (action == "move east" && player.currentPosition.Column != world.Rooms.GetLength(1) - 12)
        {
            player.currentPosition = new Position(player.currentPosition.Row, player.currentPosition.Column + 1);
        } else if (action == "enable fountain" && room.RoomType == RoomType.Fountain)
        {
            world.fountainActivated = true;
            Console.WriteLine("Fountain activated, now lets get out of here!");
        }
        else if (action == "help")
        {
            Console.Clear();
            game.showHelp();
        }
        else
        {
            Console.WriteLine("You cannot do this right now..");
        }
    }
}
public class World
{
    public Room[,] Rooms;
    public bool fountainActivated;
    public Player Player { get; }

    public World(Player player)
    {
        Player = player;
    }
    public void MakeWorld(WorldSize size)
    {
        Rooms = size switch
        {
            WorldSize.Small => new Room[4, 4],
            WorldSize.Medium => new Room[6, 6],
            WorldSize.Large => new Room[8, 8]
        };

        var rnd = new Random();
        int entranceLocation = rnd.Next(0, (int)size);
        Player.currentPosition = new Position(0, entranceLocation);
        
        for (int row = 0; row < Rooms.GetLength(0); row++)
        {
            for (int column = 0; column < Rooms.GetLength(1); column++)
            {
                if (row == 0 && column == entranceLocation)
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
                Console.Write(room.GetVisualRepresentation());
            }
            Console.WriteLine();
        }
    }

    public void showPickWorldSize()
    {
        Console.WriteLine("What game size do you want to play on?");
        Console.WriteLine("1. Small 2. Medium 3. Large");
        int choice = 0;
        while (choice == 0)
        {
            string? input =Console.ReadLine();
            if (Int32.Parse(input) >= 1 && Int32.Parse(input) <= 3)
            {
                choice = Int32.Parse(input);
                if (choice == 1)
                {
                    MakeWorld(WorldSize.Small);
                }
                else if(choice == 2)
                {
                    MakeWorld(WorldSize.Medium);
                }
                else
                {
                    MakeWorld(WorldSize.Large);
                }
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Please pick a number between 1 and 3");
            }
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

    public string GetVisualRepresentation()
    {
        string room;
        if (RoomType == RoomType.Entrance && Visited)
        {
            room = "[E]";
        } else if (RoomType == RoomType.Fountain && Visited)
        {
            room ="[F]";
        }
        else
        {
            room = "[ ]";
        }

        return room;
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
        Console.Clear();
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

public enum RoomType
{
    Empty,
    Fountain,
    Entrance
};
public enum WorldSize {Small = 4, Medium = 6, Large = 8};