using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using RockPaperScissorsAPI.Models;

namespace RockPaperScissorsAPI.Hubs
{
    public class GameHub : Hub
    {
        private static ConcurrentQueue<string> PlayerQueue = new ConcurrentQueue<string>();
        private static ConcurrentDictionary<string, string> PlayerMoves = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> PlayerNames = new ConcurrentDictionary<string, string>();

        public async Task JoinGame(string playerName)
        {
            if (PlayerQueue.IsEmpty)
            {
                PlayerQueue.Enqueue(Context.ConnectionId);
                PlayerNames[Context.ConnectionId] = playerName;
                await Clients.Caller.SendAsync("WaitForOpponent");
            }
            else
            {
                if (PlayerQueue.TryDequeue(out var opponentId))
                {
                    var opponentName = PlayerNames[opponentId];
                    PlayerNames.TryRemove(Context.ConnectionId, out _);
                    PlayerNames.TryRemove(opponentId, out _);

                    await Groups.AddToGroupAsync(Context.ConnectionId, opponentId);
                    await Groups.AddToGroupAsync(opponentId, Context.ConnectionId);

                    await Clients.Client(Context.ConnectionId).SendAsync("StartGame", playerName, opponentName);
                    await Clients.Client(opponentId).SendAsync("StartGame", opponentName, playerName);
                }
            }
        }

        public async Task SendMove(string move)
        {
            PlayerMoves[Context.ConnectionId] = move;

            if (PlayerMoves.Count == 2)
            {
                var moves = PlayerMoves.ToArray();
                var player1Id = moves[0].Key;
                var player1Move = Enum.Parse<GameLogic.Move>(moves[0].Value);
                var player2Id = moves[1].Key;
                var player2Move = Enum.Parse<GameLogic.Move>(moves[1].Value);

                var result1 = GameLogic.GetResult(player1Move, player2Move);
                var result2 = GameLogic.GetResult(player2Move, player1Move);

                
                await Clients.Client(player1Id).SendAsync("ReceiveResult", player2Move.ToString(), result1.ToString());
                await Clients.Client(player2Id).SendAsync("ReceiveResult", player1Move.ToString(), result2.ToString());

                PlayerMoves.Clear(); 
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            PlayerQueue = new ConcurrentQueue<string>(PlayerQueue.Where(id => id != Context.ConnectionId));
            PlayerMoves.TryRemove(Context.ConnectionId, out _);
            PlayerNames.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
