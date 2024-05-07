using System;
using System.Collections.Generic;
using System.Linq;

// Define suits and ranks for cards
enum Suit
{
    Clubs = '♣', 
    Diamonds = '♦', 
    Hearts = '♥', 
    Spades = '♠' 
}

// In a deck of cards, there are four suits: Clubs, Diamonds, Hearts, and Spades.
// Each suit is represented by a Unicode character (e.g., '♣' for Clubs).

enum Rank
{
    Three = 3, 
    Four, 
    Five, 
    Six, 
    Seven, 
    Eight, 
    Nine, 
    Ten, 
    Jack, 
    Queen, 
    King, 
    Ace, 
    Two 
}

// In a standard deck of cards, there are 13 ranks:
// 2, 3, 4, 5, 6, 7, 8, 9, 10, Jack, Queen, King, and Ace.
// Each rank is assigned a numerical value, with Jack = 11, Queen = 12, King = 13, and Ace = 1.

// Define a card
class Card
{
    public Suit Suit { get; set; } // The suit of the card (e.g., Clubs, Diamonds, Hearts, Spades)
    public Rank Rank { get; set; } // The rank of the card (e.g., 3, 4, 5, ..., Jack, Queen, King, Ace)

    public Card(Suit suit, Rank rank)
    {
        Suit = suit; 
        Rank = rank; 
    }

    public override string ToString()
    {
        string rankString = ""; // Initialize an empty string to store the rank representation

        // Use a switch statement to determine the string representation of the rank
        switch (Rank)
        {
            case Rank.Ace:
                rankString = "A"; 
                break;
            case Rank.Two:
                rankString = "2"; 
                break;
            case Rank.Jack:
                rankString = "J"; 
                break;
            case Rank.Queen:
                rankString = "Q"; 
                break;
            case Rank.King:
                rankString = "K"; 
                break;
            default:
                rankString = ((int)Rank).ToString(); // For other ranks, use their numerical value as a string
                break;
        }

        // Return a string representation of the card in the format: <Suit><Rank>
        // For example: '♣3' for the 3 of Clubs, '♥Q' for the Queen of Hearts
        return $"{(char)Suit}{rankString}";
    }
}

// Define a deck of cards
class Deck
{
    private List<Card> cards; // A list to store all the cards in the deck
    private Random random; // A random number generator for shuffling the deck
    public int RemainingCards => cards.Count; // A property that returns the number of cards remaining in the deck

    public Deck()
    {
        cards = new List<Card>(); // Initialize the list to store the cards
        random = new Random(); // Create a new instance of the random number generator
        InitializeDeck(); // Call the method to initialize the deck with all 52 cards
    }

    public void InitializeDeck()
    {
        cards.Clear(); // Clear the list of cards (in case it's not empty)

        // Iterate through all possible combinations of suits and ranks
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new Card(suit, rank)); // Create a new card with the current suit and rank, and add it to the deck
            }
        }
    }

    public void Shuffle()
    {
        // Shuffle the deck by randomly ordering the cards
        cards = cards.OrderBy(x => random.Next()).ToList();
    }

    public List<Card> DealCards(int numCards)
    {
        List<Card> dealtCards = cards.GetRange(0, numCards); // Get the specified number of cards from the top of the deck
        cards.RemoveRange(0, numCards); // Remove the dealt cards from the deck
        return dealtCards; // Return the list of dealt cards
    }
}

// Define a player
class Player
{
    public string Name { get; set; } // The name of the player
    public List<Card> Hand { get; set; } // The list of cards in the player's hand
    public List<Card> DiscardPile { get; set; } // The list of cards the player has discarded

    public Player(string name)
    {
        Name = name; // Set the player's name
        Hand = new List<Card>(); // Initialize the player's hand
        DiscardPile = new List<Card>(); // Initialize the player's discard pile
    }

    public void SortHand()
    {
        // Sort the player's hand first by rank (in ascending order), then by suit (in ascending order)
        Hand = Hand.OrderBy(x => x.Rank).ThenBy(x => x.Suit).ToList();
    }

    public void DiscardCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            Hand.Remove(card); // Remove the card from the player's hand
            DiscardPile.Add(card); // Add the card to the player's discard pile
        }
    }

    public void DrawCards(List<Card> cards)
    {
        Hand.AddRange(cards); // Add the drawn cards to the player's hand
    }
}

// Define the game manager
class GameManager
{
    private Deck deck; // The deck of cards used in the game
    private Player human; // The human player
    private AIPlayer computer; // The computer player (AI)
    private int currentPlayerIndex; // The index of the current player (0 for human, 1 for computer)
    private List<Card> currentPlay; // The list of cards played in the current turn
    private bool canPlayAnyCard = false; // A flag indicating if any card can be played in the current turn

    public bool CanPlayAnyCard
    {
        get { return canPlayAnyCard; } 
        set { canPlayAnyCard = value; } 
    }

    public GameManager()
    {
        deck = new Deck(); // Create a new deck of cards
        human = new Player("Player"); // Create a new human player with the name "Player"
        computer = new AIPlayer("Computer"); // Create a new computer player (AI) with the name "Computer"
        currentPlayerIndex = 0; // Set the current player index to 0 (human player)
        currentPlay = new List<Card>(); // Initialize the list of cards played in the current turn
    }

    public void InitializeGame()
    {
        deck.InitializeDeck(); // Create a new deck of cards
        deck.Shuffle(); // Shuffle the deck

        List<Player> players = new List<Player> { human, computer }; // Create a list containing the human and computer players
        DealCards(players); // Deal cards to the players
        DetermineStartingPlayer(players); // Determine the starting player for the game
    }

    private void DealCards(List<Player> players)
    {
        foreach (Player player in players)
        {
            player.Hand = deck.DealCards(17); // Deal 17 cards to each player
            player.SortHand(); // Sort the player's hand by rank and suit
        }
    }

    private void DetermineStartingPlayer(List<Player> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Hand.Any(x => x.Rank == Rank.Three && x.Suit == Suit.Spades))
            {
                currentPlayerIndex = i; // If a player has the 3 of Spades, set them as the starting player
                break;
            }
        }

        // If no player has the 3 of Spades, randomly select a starting player
        if (currentPlayerIndex == 0)
        {
            currentPlayerIndex = new Random().Next(players.Count);
        }
    }

    public void PlayGame()
    {
        int consecutivePasses = 0; // Keep track of consecutive passes by players

        while (!CheckGameOver()) // Loop until the game is over
        {
            DisplayGameState(); // Display the current game state
            Player currentPlayer = currentPlayerIndex == 0 ? human : computer; // Get the current player (human or computer)

            if (currentPlayer is AIPlayer) // If the current player is the computer
            {
                AIPlayer aiPlayer = (AIPlayer)currentPlayer;
                List<Card> play = aiPlayer.PlayTurn(currentPlay); // Get the cards the AI wants to play

                if (play != null && (CanPlayAnyCard || IsValidPlay(play, currentPlay))) // If the AI's play is valid
                {
                    currentPlayer.DiscardCards(play); // Discard the cards played by the AI
                    currentPlay = play; // Update the current play
                    CanPlayAnyCard = false; // Reset the flag for playing any card
                    consecutivePasses = 0; // Reset the consecutive pass count
                    Console.WriteLine($"{currentPlayer.Name} played:"); // Print the cards played by the AI
                    foreach (Card card in play)
                    {
                        Console.WriteLine(card);
                    }
                }
                else
                {
                    consecutivePasses++; // Increment the consecutive pass count
                    Console.WriteLine($"{currentPlayer.Name} passed."); // Print that the AI passed
                }
            }
            else // If the current player is the human
            {
                PlayerAction action = GetPlayerMove(currentPlayer); // Get the human player's action

                if (action == PlayerAction.PlayCards)
                {
                    CanPlayAnyCard = false; // Reset the flag for playing any card
                    consecutivePasses = 0; // Reset the consecutive pass count
                }
                else if (action == PlayerAction.Pass)
                {
                    consecutivePasses++; // Increment the consecutive pass count
                }
                else if (action == PlayerAction.DiscardAndDraw)
                {
                    CanPlayAnyCard = false; // Reset the flag for playing any card (new round)
                    consecutivePasses = 0; // Reset the consecutive pass count (new round)
                }
                else if (action == PlayerAction.RestartGame)
                {
                    return; // Restart the game
                }
                else if (action == PlayerAction.ExitGame)
                {
                    return; // Exit the game
                }
            }

            if (consecutivePasses == 1) // If both players have passed once
            {
                currentPlay.Clear(); // Clear the current play
                consecutivePasses = 0; // Reset the consecutive pass count
                canPlayAnyCard = true; // Allow the next player to play any card
                Console.WriteLine("All players passed. Next player can play any card.");
            }

            currentPlayerIndex = (currentPlayerIndex + 1) % 2; // Switch to the next player
            Console.WriteLine("Press Enter to continue..."); // Prompt the user to continue
            Console.ReadLine();
        }

        DetermineWinner();
    }

    // Define the player actions
    enum PlayerAction
    {
        PlayCards,
        Pass, // Pass turn
        DiscardAndDraw,
        RestartGame, 
        ExitGame 
    }

    private PlayerAction GetPlayerMove(Player player)
    {
        int choice;
        while (true)
        {
            Console.WriteLine($"\n{player.Name}, please choose an action:"); // Prompt the player to choose an action
            Console.WriteLine("1. Play cards");
            Console.WriteLine("2. Pass turn");
            Console.WriteLine("3. Discard and draw");
            Console.WriteLine("4. Restart game");
            Console.WriteLine("5. Exit game");

            if (int.TryParse(Console.ReadLine(), out choice)) // Read the player's choice
            {
                switch (choice)
                {
                    case 1:
                        PlayCards(player); 
                        return PlayerAction.PlayCards;
                    case 2:
                        PassTurn(player); 
                        return PlayerAction.Pass;
                    case 3:
                        DiscardAndDraw(player); 
                        return PlayerAction.DiscardAndDraw;
                    case 4:
                        RestartGame(); 
                        return PlayerAction.RestartGame;
                    case 5:
                        Environment.Exit(0); 
                        return PlayerAction.ExitGame;
                    default:
                        Console.WriteLine("Invalid choice, please try again."); // Invalid choice
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input, please enter a number."); // Invalid input
            }
        }
    }

    private void PlayCards(Player player)
    {
        while (true)
        {
            Console.WriteLine("Enter the indices of the cards you want to play, separated by spaces (e.g., 0 2 4), or 'p' to pass this turn:");
            string input = Console.ReadLine();

            if (input.ToLower() == "p") // If the player enters 'p' (case-insensitive), they want to pass their turn
            {
                PassTurn(player);
                return;
            }

            List<int> indices = new List<int>(); // Create a list to store the indices of the cards to play
            bool validInput = true; // Assume the input is valid initially

            foreach (string indexStr in input.Split(' ')) // Split the input string by spaces to get individual indices
            {
                if (int.TryParse(indexStr, out int index) && index >= 0 && index < player.Hand.Count) // Check if the index is valid
                {
                    indices.Add(index); // Add the valid index to the list
                }
                else
                {
                    validInput = false; // If any index is invalid, mark the input as invalid
                    break; // Stop processing further indices
                }
            }

            if (validInput) // If all indices are valid
            {
                List<Card> play = indices.Select(x => player.Hand[x]).ToList(); // Create a list of cards to play based on the indices

                if (IsValidPlay(play, currentPlay)) // Check if the selected cards form a valid play
                {
                    player.DiscardCards(play); // Discard the played cards from the player's hand
                    currentPlay = play; // Update the current play with the played cards
                    return; // Exit the method after a successful play
                }
                else
                {
                    Console.WriteLine("Invalid play, please try again."); // If the play is invalid, prompt the player to try again
                }
            }
            else
            {
                Console.WriteLine("Invalid input, please enter valid indices."); // If any index is invalid, prompt the player to enter valid indices
            }
        }
    }

    private void PassTurn(Player player)
    {
        Console.WriteLine($"{player.Name} passed their turn."); // Display a message indicating that the player passed their turn
    }

    private void DiscardAndDraw(Player player)
    {
        Console.Write("Enter the number of cards to discard (0 - 9): ");
        int numCardsToDiscard = int.Parse(Console.ReadLine()); // Get the number of cards the player wants to discard

        if (numCardsToDiscard >= 0 && numCardsToDiscard <= 9) // Check if the input is valid (between 0 and 9)
        {
            Console.WriteLine($"Select {numCardsToDiscard} cards to discard:");
            List<Card> cardsToDiscard = GetCardsFromPlayer(player, numCardsToDiscard); // Get the cards to discard from the player

            player.DiscardCards(cardsToDiscard); // Discard the selected cards from the player's hand
            player.DrawCards(deck.DealCards(numCardsToDiscard)); // Draw new cards from the deck to replace the discarded cards
            player.SortHand(); // Sort the player's hand after drawing new cards
        }
        else
        {
            Console.WriteLine("Invalid input, defaulting to 0 cards."); // If the input is invalid, no cards will be discarded
        }
    }

    private List<Card> GetCardsFromPlayer(Player player, int numCards)
    {
        List<Card> selectedCards = new List<Card>(); // Create a list to store the selected cards
        List<int> indices = new List<int>(); // Create a list to store the indices of the selected cards

        while (selectedCards.Count < numCards) // Loop until the desired number of cards is selected
        {
            DisplayHand(player); // Display the player's hand
            Console.Write($"Enter the index of card {selectedCards.Count + 1} (0 - {player.Hand.Count - 1}): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < player.Hand.Count && !indices.Contains(index)) // Check if the input is a valid index and not already selected
            {
                selectedCards.Add(player.Hand[index]); // Add the selected card to the list
                indices.Add(index); // Add the index to the list of selected indices
            }
            else
            {
                Console.WriteLine("Invalid input, please try again."); // If the input is invalid, prompt the player to try again
            }
        }

        return selectedCards; // Return the list of selected cards
    }

    private void DisplayHand(Player player)
    {
        Console.WriteLine($"\n{player.Name}'s hand:");
        for (int i = 0; i < player.Hand.Count; i++)
        {
            Console.WriteLine($"[{i}] {player.Hand[i]}"); // Display each card in the player's hand with its index
        }
    }

    private void DisplayGameState()
    {
        Console.Clear(); // Clear the console screen
        Console.WriteLine("Current game state:");
        Console.WriteLine($"Current player: {(currentPlayerIndex == 0 ? human.Name : computer.Name)}"); // Display the name of the current player
        Console.WriteLine($"Remaining cards: {deck.RemainingCards}"); // Display the number of remaining cards in the deck

        // Display the last plays of both players
        Console.WriteLine("\nLast Plays:");
        Console.WriteLine($"{human.Name}: {(human.DiscardPile.Count > 0 ? string.Join(" ", human.DiscardPile.Select(c => c.ToString())) : "None")}");
        Console.WriteLine($"{computer.Name}: {(computer.DiscardPile.Count > 0 ? string.Join(" ", computer.DiscardPile.Select(c => c.ToString())) : "None")}");

        // Display the hands of both players
        DisplayHand(human);
        DisplayHand(computer);

        // Display the current play (if any)
        if (currentPlay.Count > 0)
        {
            string lastPlayer = (currentPlayerIndex + 1) % 2 == 0 ? human.Name : computer.Name; // Determine the name of the player who made the current play
            Console.WriteLine($"\nCurrent Play by {lastPlayer}:");
            Console.WriteLine(string.Join(" ", currentPlay.Select(c => c.ToString()))); // Display the cards in the current play
        }
    }

    public bool IsValidPlay(List<Card> play, List<Card> currentPlay)
    {
        if (currentPlay.Count == 0) // If no cards have been played yet, any play is valid
            return true;

        if (play.Count != currentPlay.Count) // The number of cards played must match the number of cards in the current play
            return false;

        if (play.Count == 1) // If playing a single card
            return play[0].Rank > currentPlay[0].Rank; // The rank of the played card must be higher than the rank of the current play

        if (play.Count == 2 && play[0].Rank == play[1].Rank) // If playing a pair
            return play[0].Rank > currentPlay[0].Rank; // The rank of the pair must be higher than the rank of the current play

        if (play.Count == 3 && play[0].Rank == play[1].Rank && play[1].Rank == play[2].Rank) // If playing a triple
            return play[0].Rank > currentPlay[0].Rank; // The rank of the triple must be higher than the rank of the current play

        if (play.Count == 4 && play.Select(x => x.Rank).Distinct().Count() == 1) // If playing a bomb (four cards of the same rank)
            return true; // A bomb is always a valid play

        var playRanks = play.Select(x => x.Rank).OrderBy(x => x); // Get the ranks of the played cards in ascending order
        var currentPlayRanks = currentPlay.Select(x => x.Rank).OrderBy(x => x); // Get the ranks of the current play in ascending order

        // If the played cards form a sequence and the sequence is higher than the current play, it's a valid play
        return playRanks.SequenceEqual(currentPlayRanks) && playRanks.First() > currentPlayRanks.First();
    }

    private bool CheckGameOver()
    {
        return human.Hand.Count == 0 || computer.Hand.Count == 0; // The game is over when either the human or the computer has no cards left in their hand
    }

    private void DetermineWinner()
    {
        if (human.Hand.Count == 0)
        {
            Console.WriteLine($"{human.Name} wins!"); // If the human has no cards left, they win
        }
        else
        {
            Console.WriteLine($"{computer.Name} wins!"); // If the computer has no cards left, it wins
        }
    }

    private void RestartGame()
    {
        deck.InitializeDeck(); // Create a new deck of cards
        deck.Shuffle(); 
        human.Hand.Clear(); 
        human.DiscardPile.Clear();
        computer.Hand.Clear(); 
        computer.DiscardPile.Clear(); 
        currentPlay.Clear(); 
        currentPlayerIndex = 0; // Reset the current player index
        DealCards(new List<Player> { human, computer }); 
        DetermineStartingPlayer(new List<Player> { human, computer }); 
        PlayGame(); 
    }
}

    // Define the AI player
    class AIPlayer : Player
{
    public AIPlayer(string name) : base(name)
    {
        // Call the base class constructor to initialize the player's name
    }

    public List<Card> PlayTurn(List<Card> currentPlay)
    {
        // If no cards have been played yet, play the lowest card(s)
        if (currentPlay.Count == 0)
        {
            // If the current play is empty, return the first card from the AI player's hand
            return Hand.Take(1).ToList();
        }

        // Attempt to play a bomb (four cards of the same rank)
        var bomb = Hand.GroupBy(x => x.Rank) // Group the cards in the hand by rank
                       .Where(g => g.Count() == 4) // Find the group with exactly 4 cards (a bomb)
                       .FirstOrDefault(); // Get the first group (if any)
        if (bomb != null)
        {
            // If a bomb is found, return it as the play
            return bomb.ToList();
        }

        // Attempt to play a single card higher than the current play
        var sameCountCards = Hand.GroupBy(x => x.Rank) // Group the cards in the hand by rank
                                 .Where(g => g.Count() == currentPlay.Count) // Find groups with the same number of cards as the current play
                                 .SelectMany(g => g) // Flatten the groups into a single list
                                 .ToList();
        if (sameCountCards.Count >= currentPlay.Count)
        {
            var higherCards = sameCountCards.Where(card => card.Rank > currentPlay.Max(x => x.Rank)) // Find cards with a higher rank than the current play
                                            .Take(currentPlay.Count) // Take the same number of cards as the current play
                                            .ToList();
            if (higherCards.Count == currentPlay.Count)
            {
                // If a valid higher play is found, return it
                return higherCards;
            }
        }

        // Attempt to play a sequence (e.g., 3, 4, 5, 6, 7)
        var sortedHand = Hand.OrderBy(x => x.Rank).ToList(); // Sort the hand by rank
        for (int i = 0; i <= sortedHand.Count - currentPlay.Count; i++)
        {
            var sequence = sortedHand.Skip(i).Take(currentPlay.Count).ToList(); // Get a sequence of cards with the same length as the current play
            if (IsSequenceValid(sequence, currentPlay))
            {
                // If the sequence is valid and higher than the current play, return it
                return sequence;
            }
        }

        // If no valid play is found, return an empty list
        return new List<Card>();
    }

    private bool IsSequenceValid(List<Card> sequence, List<Card> currentPlay)
    {
        // Check if the sequence and current play have the same number of cards
        if (sequence.Count != currentPlay.Count)
        {
            return false;
        }

        // Sort the sequence and current play by rank
        var sortedSequence = sequence.OrderBy(x => x.Rank).ToList();
        var sortedCurrentPlay = currentPlay.OrderBy(x => x.Rank).ToList();

        // Check if the sequence is in consecutive order
        for (int i = 0; i < sortedSequence.Count - 1; i++)
        {
            if (sortedSequence[i].Rank + 1 != sortedSequence[i + 1].Rank)
            {
                return false;
            }
        }

        // Check if the sequence is higher than the current play
        return sortedSequence[0].Rank > sortedCurrentPlay[0].Rank;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Set the console output encoding to UTF-8 to display Unicode characters correctly
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        GameManager gameManager = new GameManager();
        gameManager.InitializeGame();
        gameManager.PlayGame();
    }
}