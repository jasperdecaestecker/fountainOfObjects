// Small, Medium and Large
// Amaroks
// Getting Armed
// Getting Help

var game = new FountainOfObjectsGame();
var player = new Player();
var world = new World(player, true);
game.showStartScreen();
world.showPickWorldSize();
while (game.GameIsRunning)
{
    var room = world.Rooms[player.currentPosition.Row, player.currentPosition.Column];
    world.printRooms(player);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(room.getRoomTypeDescription());
    player.determineAction(world, room, game);
    game.checkLoseCondition(world, player);
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
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }

    public void checkLoseCondition(World world, Player player)
    {
        foreach (var monsterPosition in world.MonsterPositions)
        {
            if (player.currentPosition.ToString() == monsterPosition.ToString())
            {
                world.printRooms(player);
                GameIsRunning = false;
                Console.WriteLine("You have entered a room with an Amorak... You died.");
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }
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
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();
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

public class World
{
    public Room[,] Rooms;
    public bool fountainActivated;
    public bool RevealAll { get; }
    public Player Player { get; }
    private WorldSize Size { get; set; }
    public Position[] MonsterPositions { get; set; }

    public World(Player player, bool revealAll)
    {
        Player = player;
        RevealAll = revealAll;
    }
    public void MakeWorld(WorldSize size)
    {
        Size = size;
        Rooms = size switch
        {
            WorldSize.Small => new Room[4, 4],
            WorldSize.Medium => new Room[6, 6],
            WorldSize.Large => new Room[8, 8]
        };

        var rnd = new Random();
        int entranceLocation = rnd.Next(0, (int)size);
        Position fountainPosition = new Position(rnd.Next(0, (int)size), rnd.Next(0, (int)size));
        for (int row = 0; row < Rooms.GetLength(0); row++)
        {
            for (int column = 0; column < Rooms.GetLength(1); column++)
            {
                var roomPosition = new Position(row, column);
                if (row == 0 && column == entranceLocation)
                {
                    Rooms[row, column] = new Room(RoomType.Entrance, new Position(row, column));
                    Player.currentPosition = new Position(row, column);
                } else if (roomPosition.ToString() == fountainPosition.ToString())
                {
                    Rooms[row, column] = new Room(RoomType.Fountain, new Position(row, column));
                }
                else
                {
                    Rooms[row, column] = new Room(RoomType.Empty, new Position(row, column));
                }

                if (RevealAll)
                {
                    Rooms[row,column].Revealed = true;
                }
            } 
        }

        insertAmarakosIntoWorld();
    }

    public void printRooms(Player player)
    {
        Rooms[player.currentPosition.Row, player.currentPosition.Column].Revealed = true;
        for (int row = 0; row < Rooms.GetLength(0); row++)
        {
            for (int column = 0; column < Rooms.GetLength(1); column++)
            {
                var room = Rooms[row, column];

                if (player.currentPosition.Column == column && player.currentPosition.Row == row)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(room.GetVisualRepresentation(player));
            }
            Console.WriteLine();
        }
    }

    public void insertAmarakosIntoWorld()
    {
        MonsterPositions = Size switch
        {
            WorldSize.Small => new Position[1],
            WorldSize.Medium => new Position[2],
            WorldSize.Large => new Position[3]
        };
        
        var rnd = new Random();
        for (int monster = 0; monster < MonsterPositions.GetLength(0); monster++)
        {
            bool generateCorrectPos = true;
            while (generateCorrectPos)
            {
                Position randomPos = new Position(rnd.Next(0, (int)Size), rnd.Next(0, (int)Size));
                if (Rooms[randomPos.Row, randomPos.Column].RoomType == RoomType.Empty)
                {
                    Rooms[randomPos.Row, randomPos.Column].RoomType = RoomType.Amarok;
                    generateCorrectPos = false;
                    MonsterPositions[monster] = randomPos;
                }
            }
        }
    }

    public void showPickWorldSize()
    {

        WorldSize choice = WorldSize.NotDetermined;
        int selectedUIOption = 1;
        while (choice == WorldSize.NotDetermined)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("What game size do you want to play on?");
            
            if (selectedUIOption == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Small World: 4 x 4");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Medium World: 6 x 6");
                Console.WriteLine("Large World: 8 x 8");
            }
            
            if (selectedUIOption == 2)
            {
                Console.WriteLine("Small World: 4 x 4");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Medium World: 6 x 6");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Large World: 8 x 8");
            }
            if (selectedUIOption == 3)
            {
                Console.WriteLine("Small World: 4 x 4");
                Console.WriteLine("Medium World: 6 x 6");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Large World: 8 x 8");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Up/Down to change. Enter to select");
            var pressedKey = Console.ReadKey();
            if (pressedKey.Key == ConsoleKey.S || pressedKey.Key == ConsoleKey.DownArrow)
            {
                if (selectedUIOption == 3)
                {
                    selectedUIOption = 1;
                }
                else
                {
                    selectedUIOption++;
                }
            } else if (pressedKey.Key == ConsoleKey.W || pressedKey.Key == ConsoleKey.UpArrow)
            {
                if (selectedUIOption == 1)
                {
                    selectedUIOption = 3;
                }
                else
                {
                    selectedUIOption--;
                }
            } else if (pressedKey.Key == ConsoleKey.Enter)
            {
                choice = selectedUIOption switch
                {
                    1 => WorldSize.Small,
                    2 => WorldSize.Medium,
                    3 => WorldSize.Large,
                    _ => WorldSize.NotDetermined
                };
                Console.Clear();
                MakeWorld(choice);
            }
        }
    }
}
public class Room
{
    public RoomType RoomType { get; set; }
    public bool Revealed;
    public Position Position;

    public Room(RoomType roomType, Position position)
    {
        RoomType = roomType;
        Position = position;
        Revealed = false;
    }

    public string GetVisualRepresentation(Player player)
    {
        string room;
        Console.ForegroundColor = Revealed ? ConsoleColor.White : ConsoleColor.Gray;

        if (player.currentPosition.Row == Position.Row && player.currentPosition.Column == Position.Column)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        
        if (RoomType == RoomType.Entrance && Revealed)
        {
            room = "[E]";
        } else if (RoomType == RoomType.Fountain && Revealed)
        {
            room ="[F]";
        }
        else if (RoomType == RoomType.Amarok && Revealed)
        {
            room = "[M]";
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

    public void determineAction(World world, Room room,FountainOfObjectsGame game)
    {
        Console.Write("What do you want to do? ");
        string action = Console.ReadLine();
        
        if (action == "move north" && currentPosition.Row != 0)
        {
            currentPosition = new Position(currentPosition.Row - 1, currentPosition.Column);
            checkAmarokProximity(world);
        }
        else if (action == "move south" && currentPosition.Row != world.Rooms.GetLength(0) - 1)
        {
            currentPosition = new Position(currentPosition.Row + 1, currentPosition.Column);
            checkAmarokProximity(world);
        } else if (action == "move west" && currentPosition.Column != 0)
        {
            currentPosition = new Position(currentPosition.Row, currentPosition.Column -1);
            checkAmarokProximity(world);
        } else if (action == "move east" && currentPosition.Column != world.Rooms.GetLength(1) - 1)
        {
            currentPosition = new Position(currentPosition.Row, currentPosition.Column + 1);
            checkAmarokProximity(world);
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

    public void checkAmarokProximity(World world)
    {
        foreach (var monsterPosition in world.MonsterPositions)
        {
            if ((currentPosition.Row - 1 == monsterPosition.Row && currentPosition.Column == monsterPosition.Column) || 
                (currentPosition.Row + 1 == monsterPosition.Row && currentPosition.Column == monsterPosition.Column) ||
                (currentPosition.Column - 1 == monsterPosition.Column && currentPosition.Row == monsterPosition.Row) ||
                (currentPosition.Column + 1 == monsterPosition.Column && currentPosition.Row == monsterPosition.Row))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You smell something foul, an amarok is in a nearby room. Get your bow ready!!");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            } 
        }
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
    Entrance,
    Amarok,
    DeadAmarok,           
};
public enum WorldSize {NotDetermined = 0, Small = 4, Medium = 6, Large = 8};