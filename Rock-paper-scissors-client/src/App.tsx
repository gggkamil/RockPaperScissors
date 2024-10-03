import React, { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import './App.css'; 

const App: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [playerName, setPlayerName] = useState<string>('');
  const [opponentName, setOpponentName] = useState<string>('');
  const [isWaiting, setIsWaiting] = useState<boolean>(false);
  const [resultMessage, setResultMessage] = useState<string>('');
  const [lastPlayerMove, setLastPlayerMove] = useState<'Rock' | 'Paper' | 'Scissors' | null>(null);
  const [moveDisabled, setMoveDisabled] = useState<boolean>(false);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://rockpaperscissors-kamil-ang8d2a5fjh8fufn.polandcentral-01.azurewebsites.net/gamehub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    newConnection.on("WaitForOpponent", () => {
      setIsWaiting(true);
    });

    newConnection.on("StartGame", (_player: string, opponent: string) => {
      setOpponentName(opponent);
      setIsWaiting(false);
      setResultMessage('');
      setMoveDisabled(false);
    });

    newConnection.on("ReceiveResult", (opponentMove: 'Rock' | 'Paper' | 'Scissors', result: 'Win' | 'Lose' | 'Draw') => {
      let message = `Your opponent chose ${opponentMove}. You ${result}!`;
      setResultMessage(message);
      setLastPlayerMove(null);
      setMoveDisabled(true);
    });

    const startConnection = async () => {
      try {
        await newConnection.start();
        setConnection(newConnection);
        console.log('SignalR connection established.');
      } catch (e) {
        console.error('Connection failed: ', e);
      }
    };

    startConnection();

    return () => {
      if (connection) {
        connection.stop().catch(_e => console.error('Error stopping connection: ', _e));
      }
    };
  }, []);

  const joinGame = async () => {
    if (connection && playerName) {
      try {
        await connection.send('JoinGame', playerName);
      } catch (e) {
        console.error('Error joining game: ', e);
      }
    }
  };

  const makeMove = async (move: 'Rock' | 'Paper' | 'Scissors') => {
    if (!moveDisabled && lastPlayerMove === null) {
      setLastPlayerMove(move);
      setResultMessage(`You selected ${move}. Waiting for opponent's move...`);

      if (connection) {
        try {
          await connection.send('SendMove', move);
          setMoveDisabled(true);
        } catch (e) {
          console.error('Error sending move: ', e);
        }
      }
    }
  };

  return (
    <div className="app-container">
      <h3>Rock-Paper-Scissors Game</h3>
      {!opponentName && !isWaiting && (
        <div className="join-section">
          <input
            className="input-field"
            value={playerName}
            onChange={e => setPlayerName(e.target.value)}
            placeholder="Enter your name"
          />
          <button className="join-button" onClick={joinGame}>Join Game</button>
        </div>
      )}
      {isWaiting && (
        <div className="spinner-container">
          <div className="spinner"></div>
          <p>Waiting for an opponent...</p>
        </div>
      )}
      {opponentName && (
        <div className="game-section">
          <p>You are playing against {opponentName}.</p>
          <p>Select your move:</p>
          <div className="button-group">
            <button className="move-button" onClick={() => makeMove("Rock")} disabled={moveDisabled}>Rock</button>
            <button className="move-button" onClick={() => makeMove("Paper")} disabled={moveDisabled}>Paper</button>
            <button className="move-button" onClick={() => makeMove("Scissors")} disabled={moveDisabled}>Scissors</button>
          </div>
          <p>{resultMessage}</p>
          {moveDisabled && resultMessage && (
            <button className="play-again-button" onClick={joinGame}>Play Again</button>
          )}
        </div>
      )}
    </div>
  );
};

export default App;
