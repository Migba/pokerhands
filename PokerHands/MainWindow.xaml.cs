using System;
using System.Windows;
using System.Linq;

namespace PokerHands
{
    public partial class MainWindow : Window
    {
        private readonly Solver.Solver _solver = new Solver.Solver();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var playerNames = new string[] {playerOneTextBox.Text, playerTwoTextBox.Text};
            if (playerNames.Any(s => s.Trim() == ""))
            {
                resultLabel.Content = "Player Names cannot be blank!";
                return;
            }
            var playerCardStrings = new string[][] {playerOneCardsTextBox.Text.Split(','), playerTwoCardsTextBox.Text.Split(',')};
            if (playerCardStrings.Any(a => a.Length == 0))
            {
                resultLabel.Content = "Player Hands cannot be blank!";
                return;
            }
            if (playerCardStrings.Any(a => a.Length != 5))
            {
                resultLabel.Content = "Player Hands must contain five cards and use commas as delimiters.";
                return;
            }
            try
            {
                var scoresByPlayer = _solver.GetScoreByPlayer(playerNames, playerCardStrings);

                playerOneScoreTextBox.Text = scoresByPlayer[playerNames[0]].HandType.ToString();
                playerTwoScoreTextBox.Text = scoresByPlayer[playerNames[1]].HandType.ToString();

                var winner = scoresByPlayer.OrderByDescending(kvp => kvp.Value).First();
                var loser = scoresByPlayer.OrderByDescending(kvp => kvp.Value).Last();

                resultLabel.Content = winner.Value.CompareTo(loser.Value) == 0 
                                      ? "Tie Game!" 
                                      : $"The winner is {winner.Key} with a {winner.Value.HandType.ToString()}";
            }
            catch (Exception ex)
            {
                resultLabel.Content = "Invalid Input!";
            }
        }
    }
}
