import { action, makeObservable, observable, runInAction } from "mobx";
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import { Activity } from "./model/activity";

export class MainStore {
  @observable activities: Activity[] = []
  @observable isOnline: boolean = false;
  @observable isConnecting: boolean = false;
  
  private hub: HubConnection

  constructor() {
    makeObservable(this)
    this.hub = new HubConnectionBuilder()
      .withUrl(`${process.env.PUBLIC_URL}/hub`)
      .withAutomaticReconnect([0,0,1000])
      .build();
  }

  @action
  wire() {
    this.hub.onclose(() => runInAction(() => {
      this.isOnline = false;
      this.isConnecting = false;
    }));
    this.hub.onreconnecting(() => runInAction(() => {
      this.isOnline = false;
      this.isConnecting = true;
    }));
    this.hub.onreconnected(() => runInAction(() => {
      this.isOnline = true;
      this.isConnecting = false;
    }));

    this.isConnecting = true;
    this.hub.start().then(() => runInAction(() => {
      this.isConnecting = false;
      this.isOnline = true;
      this.hub.on('ReceiveMessage', message => {
        console.log('received message ', message);
      });
    }))
    .catch(() => runInAction(() => {
      this.isConnecting = false;
      this.isOnline = false;
    }));

    return this;
  }
}