import * as signalr from "@microsoft/signalr";
import { messageService } from './messageService';


class Hub {

  public connection?: signalr.HubConnection;

  constructor() {
    this.connection = new signalr.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hub")
      .configureLogging(signalr.LogLevel.Debug)
      .build();

  }

  async initialize() {
    if (this.connection)
      try {
        await this.connection.start();
        console.log('che figo sono collegato')
      } catch {
        console.log('che brutta mattina...')
      }
  }

  async sendMessage(messageName: string, ...model: any[]) {
    if (this.connection)
      await this.connection.send(messageName, ...model)
  }

  async buongiorno() {
    await this.sendMessage('buongiorno');
  }
}

class HubClient {
  constructor(connection: signalr.HubConnection) {
    for (const key in this) {
      connection.on(key, this[key] as any);
    }
  }

  greetings = (m: string) => {
    console.log(m);
  }
}

export const hub = new Hub();
async function init() {
  await hub.initialize();
  if (hub.connection)
    new HubClient(hub.connection);

  hub.buongiorno();
}

init();
