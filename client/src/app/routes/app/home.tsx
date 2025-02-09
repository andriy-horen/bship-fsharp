import { api } from '@/lib/api-client';
import * as signalR from '@microsoft/signalr';
import { useEffect, useState } from 'react';

interface AuthResponse {
  token: string;
}

export const Home: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection>();
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    api.post<{}, AuthResponse>('/auth/token').then((response) => {
      setToken(response.token);
    });
  }, []);

  useEffect(() => {
    if (!token) {
      return;
    }

    let connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:4000/game', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => token,
      })
      .build();

    setConnection(connection);
  }, [token]);

  const handleClick = async (): Promise<void> => {
    if (!connection) {
      return;
    }

    await connection.start();
    await connection.invoke('SendMessage', 'Hello');
  };

  return (
    <div>
      <h1>WebSocket Test App</h1>
      <div>
        <h3>SignalR</h3>
        <button onClick={handleClick}>Connect</button>
      </div>
    </div>
  );
};
