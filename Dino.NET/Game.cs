
public class Game
{
    private const int GameWidth = 50;
    private const int GameHeight = 20;
    private const char GroundChar = '_';
    private bool _isRunning;
    private int _dinoPositionY;
    private int _dinoVelocityY;
    private bool _isJumping;
    private int _obstaclePositionX;
    private int _dinoFrame;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private readonly string[][] _dinoSprites = new[]
    {
        new[]
        {
            "  __ ",
            " / _)",
            "/ /  ",
            "\\_\\  "
        },
        new[]
        {
            "  __ ",
            " / _)",
            "/ /  ",
            "/_/  "
        }
    };

    private readonly string[][] _obstacleSprites = new[]
    {
        new[]
        {
            " __ ",
            "|  |",
            "|  |",
            "|__|"
        }
    };

    public Game()
    {
        _dinoPositionY = GameHeight - 5;
        _obstaclePositionX = GameWidth - 1;
        _isJumping = false;
        _dinoVelocityY = 0;
        _dinoFrame = 0;
    }

    public async Task StartAsync()
    {
        _isRunning = true;

        var gameLoopTask = Task.Run(GameLoop);
        var inputTask = Task.Run(HandleInput);

        await Task.WhenAny(gameLoopTask, inputTask);

        _isRunning = false;
        _cancellationTokenSource.Cancel();
        Console.Clear();
        Console.WriteLine("Game Over!");
    }

    private async Task GameLoop()
    {
        while (_isRunning)
        {
            Update();
            Render();
            await Task.Delay(100);
        }
    }

    private async Task HandleInput()
    {
        while (_isRunning)
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Spacebar && !_isJumping)
                {
                    _isJumping = true;
                    _dinoVelocityY = -5;
                }
            }
            await Task.Delay(10);
        }
    }

    private void Update()
    {
        _dinoFrame = (_dinoFrame + 1) % _dinoSprites.Length;

        if (_isJumping)
        {
            _dinoPositionY += _dinoVelocityY;
            _dinoVelocityY += 1; // Gravity

            if (_dinoPositionY >= GameHeight - 5)
            {
                _dinoPositionY = GameHeight - 5;
                _isJumping = false;
                _dinoVelocityY = 0;
            }
        }

        _obstaclePositionX--;
        if (_obstaclePositionX < 0)
        {
            _obstaclePositionX = GameWidth - 1;
        }

        if (CheckCollision())
        {
            _isRunning = false;
        }
    }

    private bool CheckCollision()
    {
        for (int dy = 0; dy < _dinoSprites[_dinoFrame].Length; dy++)
        {
            for (int dx = 0; dx < _dinoSprites[_dinoFrame][dy].Length; dx++)
            {
                int dinoX = 2 + dx;
                int dinoY = _dinoPositionY + dy;

                if (_dinoSprites[_dinoFrame][dy][dx] != ' ' && IsObstaclePosition(dinoX, dinoY))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Render()
    {
        Console.Clear();

        for (int y = 0; y < GameHeight; y++)
        {
            for (int x = 0; x < GameWidth; x++)
            {
                if (y == GameHeight - 1)
                {
                    Console.Write(GroundChar);
                }
                else if (IsDinoPosition(x, y))
                {
                    Console.Write(GetDinoChar(x, y));
                }
                else if (IsObstaclePosition(x, y))
                {
                    Console.Write(GetObstacleChar(x, y));
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
    }

    private bool IsDinoPosition(int x, int y)
    {
        return y >= _dinoPositionY && y < _dinoPositionY + _dinoSprites[0].Length && x >= 2 && x < 2 + _dinoSprites[0][0].Length;
    }

    private char GetDinoChar(int x, int y)
    {
        int localX = x - 2;
        int localY = y - _dinoPositionY;
        return _dinoSprites[_dinoFrame][localY][localX];
    }

    private bool IsObstaclePosition(int x, int y)
    {
        return y >= GameHeight - 5 && y < GameHeight - 1 && x >= _obstaclePositionX && x < _obstaclePositionX + _obstacleSprites[0][0].Length;
    }

    private char GetObstacleChar(int x, int y)
    {
        int localX = x - _obstaclePositionX;
        int localY = y - (GameHeight - 5);
        return _obstacleSprites[0][localY][localX];
    }
}
