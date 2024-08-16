Rock-Paper-Scissors Game
Welcome to the Rock-Paper-Scissors game project! This is a real-time, multiplayer Rock-Paper-Scissors game built with ASP.NET Core 8 and React. The game leverages SignalR to facilitate real-time communication, enabling players to connect over Wi-Fi, be paired randomly, and play a quick game of Rock-Paper-Scissors.

🚀 Features
Real-Time Gameplay: Utilizes SignalR to ensure instant updates and communication between players.
Random Pairing: Players are automatically paired with each other once they join the game.
Classic Game Logic: Implements the traditional Rock-Paper-Scissors rules to determine the winner.
Responsive UI: Dynamic and interactive user interface providing real-time feedback and game updates.
💡 How It Works
Join the Game: Players enter their name and wait for an opponent to join.
Make Your Move: Once paired, players choose between Rock, Paper, or Scissors.
Receive Results: The game determines the outcome based on the moves made and displays the result to both players.
Play Again: Players can start a new game once the previous game ends.
🎮 Play the Game
Open the React application in your browser: http://localhost:5173/.
Enter your player name and click "Join Game."
Wait for an opponent to join and then make your move.
View the results and optionally play again.
🛠 Development
Backend: Located in RockPaperScissors, using ASP.NET Core and SignalR for real-time communication.
Frontend: Located in Rock-Paper-Scissors-Client, built with React and SignalR client for handling game interactions
