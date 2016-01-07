using System;

namespace TicTacToe
{
    class TicTacToeBoard
    {
        int[,] board;

        // Constructs empty board
        public TicTacToeBoard()
        {
            board = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = 0;
                }
            }
        }

        // Constructs board based on passed board
        public TicTacToeBoard(TicTacToeBoard otherBoard)
        {
            board = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i, j] = otherBoard.GetBoard()[i, j];
                }
            }
                
        }

        public void MakeMove(int move, int player)
        {
            board[(move-1) / 3, (move-1) % 3] = player;
        }

        public int[,] GetBoard()
        {
            return board;
        }

        // Returns tile on board of player, or 0 if no move taken
        public int GetMove(int move)
        {
            return board[(move-1) / 3, (move-1) % 3];
        }

        public void WipeBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    board[i,j] = 0;
                }
            }
        }

        // returns int val of player if win condition is met, otherwise 0
        public int CheckForWin()
        {
            int diagLeft = 1;
            int diagRight = 1;
            for (int i = 0; i < 3; i++)
            {
                int col = 1;
                int row = 1;
                for (int j = 0; j < 3; j++)
                {
                    row *= board[i,j];
                    col *= board[j,i];
                }
                if (col == 1 || row == 1)
                {
                    return 1;
                }
                else if (col == 8 || row == 8)
                {
                    return 2;
                }
                diagLeft *= board[i,i];
                diagRight *= board[2 - i,i];
            }
            if (diagLeft == 1 || diagRight == 1)
            {
                return 1;
            }
            else if (diagLeft == 8 || diagRight == 8)
            {
                return 2;
            }

            return 0;
        }

        public bool CheckForDraw()
        {
            bool draw = true;
            for (int i = 1; i <= 9; i++)
            {
                if (GetMove(i) == 0) draw = false; 
            }
            return draw;
        }

        public void DisplayBoard()
        {
            Console.WriteLine(" --- ");
            for (int i = 0; i < 3; i++)
            {
                Console.Write("|");
                for (int j = 0; j < 3; j++)
                {
                    switch (board[i,j])
                    {
                        case 1:
                            Console.Write("X");
                            break;
                        case 2:
                            Console.Write("O");
                            break;
                        default:
                            Console.Write(" ");
                            break;
                    }
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(" --- ");
        }
    }

    class TicTacToeBot
    {
        double gamma;

        public TicTacToeBot()
        {
            gamma = 0.98;
        }

        public double Score(TicTacToeBoard board)
        {
            int winner = board.CheckForWin();
            if (winner == 1)
            {
                return 10.0;
            }
            else if (winner == 2)
            {
                return -10.0;
            }
            else
            {
                return 0.0;
            }
        }

        public int GetBestMove(TicTacToeBoard board)
        {
            double max = -100000.0;
            int bestMoveSoFar = 0;
            for (int i = 1; i <= 9; i++)
            {
                if (board.GetMove(i) == 0)
                {
                    TicTacToeBoard testBoard = new TicTacToeBoard(board);
                    testBoard.MakeMove(i, 1);
                    double moveScore = AlphaBeta(testBoard, -10000.0, 10000.0, 2);
                    if (moveScore > max)
                    {
                        max = moveScore;
                        bestMoveSoFar = i;
                    }
                }
            }
            return bestMoveSoFar;
        }

        public double AlphaBeta(TicTacToeBoard oldBoard, double alpha, double beta, int player)
        {
            if (oldBoard.CheckForWin() != 0 || oldBoard.CheckForDraw())
            {
                return Score(oldBoard);
            }
            if (player == 1)
            {
                double val = -10000.0;
                for (int i = 1; i <= 9; i++)
                {
                    if (oldBoard.GetMove(i) == 0)
                    {
                        TicTacToeBoard newBoard = new TicTacToeBoard(oldBoard);
                        newBoard.MakeMove(i, player);
                        val = Math.Max(val, gamma * AlphaBeta(newBoard, alpha, beta, 2));
                        alpha = Math.Max(alpha, val);
                        if (beta <= alpha) break;
                    }
                }
                return alpha;
            }
            else
            {
                double val = 10000.0;
                for (int i = 1; i <= 9; i++)
                {
                    if (oldBoard.GetMove(i) == 0)
                    {
                        TicTacToeBoard newBoard = new TicTacToeBoard(oldBoard);
                        newBoard.MakeMove(i, player);
                        val = Math.Min(val, gamma * AlphaBeta(newBoard, alpha, beta, 1));
                        beta = Math.Min(val, beta);
                        if (beta <= alpha) break;
                    }
                }
                return beta;
            }
        }
    }

    class TicTacToeGame
    {
        static void Main()
        {
            TicTacToeGame game = new TicTacToeGame();
            game.start();
            
        }

        public void start()
        {
            Console.WriteLine("Welcome to TicTacToeBot!");
            bool quit = false;
            while (!quit)
            {
                PlayAGame();
                Console.WriteLine("Play another? (Y/N)");
                bool validInput = false;
                while (!validInput)
                {
                    string input = Console.ReadLine();
                    if (String.Equals("N", input.ToUpper()))
                    {
                        Console.WriteLine("Goodbye!");
                        quit = true;
                        validInput = true;
                    }
                    else if (String.Equals("Y", input.ToUpper()))
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
            }
        }

        public void PlayAGame()
        {
            TicTacToeBot ai = new TicTacToeBot();
            TicTacToeBoard board = new TicTacToeBoard();
            Random rng = new Random();
            bool yourTurn;
            Console.WriteLine("Randomizing who starts...");
            if (rng.Next(2) == 0)
            {
                Console.WriteLine("You start!");
                yourTurn = true;
            }
            else
            {
                Console.WriteLine("AI starts.");
                yourTurn = false;
            }

            int winner = 0;
            bool draw = false;
            while (winner == 0 && !draw)
            {
                if (yourTurn)
                {
                    TakeATurn(board);
                }
                else
                {
                    board.MakeMove(ai.GetBestMove(board), 1);
                }
                yourTurn = !yourTurn;
                winner = board.CheckForWin();
                draw = board.CheckForDraw();
            }
            board.DisplayBoard();
            if (winner == 1)
            {
                Console.WriteLine("AI wins.");
            }
            else if (winner == 2)
            {
                Console.WriteLine("You win!");
            }
            else
            {
                Console.WriteLine("Draw game.");
            }
        }

        public void TakeATurn(TicTacToeBoard board)
        {
            board.DisplayBoard();
            bool validMove = false;
            while (!validMove)
            {
                Console.WriteLine("Please make a move (1-9):");
                int move;
                if (Int32.TryParse(Console.ReadLine(), out move))
                {
                    if (board.GetMove(move) == 0)
                    {
                        board.MakeMove(move, 2);
                        validMove = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid move. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid syntax. Please try again.");
                }
            }
        }
    }





}