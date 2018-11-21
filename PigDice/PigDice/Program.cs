using System;
using System.Collections.Generic;
using System.Threading;

namespace PigDice
{
    class Program
    {
        private static Random _random = new Random();
        private static Dictionary<bool, Player> _playerDictionary = new Dictionary<bool, Player>();
        private static bool _player1Turn = true;
        private static int _temporaryPoints = 0;
        private static int _pointsRequired = 20;
        static void Main(string[] args)
        {
            _playerDictionary.Add(true, new Player());
            _playerDictionary.Add(false, new Player());
            Console.WriteLine("Are you ready for an awesome game!? How about PIG DICE!!!");
            Console.WriteLine();
            _pointsRequired = _random.Next(20, 50);
            Console.WriteLine($"***For this battle, you must get to {_pointsRequired} to win!***");
            Console.WriteLine();
            Console.WriteLine("What is your name, first player?");
            var firstPlayerName = Console.ReadLine();
            _playerDictionary[true].Name = firstPlayerName;
            Console.WriteLine();
            Console.WriteLine($"Welcome, {firstPlayerName}!");
            Console.WriteLine("It looks like there is a challenger! What is your name, challenger?");
            Console.WriteLine();

            var secondPlayerName = Console.ReadLine();
            _playerDictionary[false].Name = secondPlayerName;
            Console.WriteLine($"Welcome, {secondPlayerName}! Prepare for battle!");
            
            var currentPlayer = _playerDictionary[_player1Turn];
            Console.WriteLine($"{currentPlayer.Name}, It's your turn! Press any key and roll the die and test your luck:");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine($"{currentPlayer.Name}: <press any key to roll>:");
                Console.ReadKey();
                Console.WriteLine();
                Thread.Sleep(500);
                
                var dieRoll = _random.Next(1, 6);
                if (dieRoll == 1)
                {
                    Console.WriteLine(
                        $"********* Oh no! You rolled a 1! {currentPlayer.Name} just lost all of their points this round. They have {currentPlayer.TotalPoints} total points. *********");
                    Console.WriteLine();

                    currentPlayer = SwitchPlayerTurns();
                    continue;
                }

                _temporaryPoints += dieRoll;

                Console.WriteLine(
                    $"You rolled a {dieRoll}! You now have {_temporaryPoints} temporary points.");
                Console.WriteLine($"{currentPlayer.Name}, do you want to test your luck and roll again? Y/N:");
                var rollAgain = Console.ReadKey();
                Thread.Sleep(500);

                Console.WriteLine();
                if (rollAgain.Key.ToString().Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine(GetContinueMessage(currentPlayer));
                    Console.WriteLine();

                    continue;
                }

                currentPlayer.TotalPoints += _temporaryPoints;

                CheckForWinner(currentPlayer);

                EndTurnByChoice(currentPlayer, _temporaryPoints);
                
                Console.WriteLine();

                currentPlayer = SwitchPlayerTurns();
            }
        }

        private static void EndTurnByChoice(Player currentPlayer, int temporaryPoints)
        {
            Console.WriteLine(GetTurnEndedByChoiceMessage(currentPlayer));
            Console.WriteLine($"You accumulated { _temporaryPoints} points this round, "
                + $"and now have {currentPlayer.TotalPoints} total Points!");
        }

        private static List<string> _continueMessages = new List<string>
        {
            "You are a risk-taker, eh {0}? Let's see if you're lucky!",
            "You want to play with fire, huh? Good luck {0}!",
            "Do you like to battle dangerously, {0}? Give it a shot!"
        };

        private static string GetContinueMessage(Player currentPlayer)
        {
            return GetRandomMessage(currentPlayer, _continueMessages);
        }

        private static List<string> _turnEndedByChoiceMessages = new List<string>
        {
            "You are very wise, {0}.",
            "{0}, you are a chicken!",
            "That is a very smart move, {0}!"
        };

        private static string GetTurnEndedByChoiceMessage(Player currentPlayer)
        {
            return GetRandomMessage(currentPlayer, _turnEndedByChoiceMessages);
        }

        private static string GetRandomMessage(Player currentPlayer, List<string> messages)
        {
            int numberOfMessages = messages.Count;
            var randomInt = _random.Next(0, numberOfMessages - 1);
            return string.Format(messages[randomInt], currentPlayer.Name);
        }

        private static Player SwitchPlayerTurns()
        {
            _temporaryPoints = 0;
            _player1Turn = !_player1Turn;
            var currentPlayer = _playerDictionary[_player1Turn];
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine($"It's now your turn, {currentPlayer.Name}! The score is: ");
            Console.WriteLine(_playerDictionary[true].Name + ": " + _playerDictionary[true].TotalPoints + " points");
            Console.WriteLine(_playerDictionary[false].Name + ": " + _playerDictionary[false].TotalPoints + " points");
            Console.WriteLine();
            return currentPlayer;
        }

        private static void CheckForWinner(Player currentPlayer)
        {
            if (currentPlayer.TotalPoints >= _pointsRequired)
            {
                const string winnerBorder =
                    "!***************************************************************************************************!";
                Console.WriteLine();
                Console.WriteLine(winnerBorder);
                Console.WriteLine($"Congratulations {currentPlayer.Name}, you got {currentPlayer.TotalPoints} points and won the game!");
                Console.WriteLine(winnerBorder);
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }

    internal class Player
    {
        public string Name { get; set; }
        public int TotalPoints { get; set; }
    }
}
