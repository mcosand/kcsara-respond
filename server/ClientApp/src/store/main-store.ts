import { action, makeObservable, observable, onBecomeObserved, runInAction } from "mobx";
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

import { Activity } from "./model/activity";
import { User } from "./model/user";
import { Loadable } from "./model/loadable";

export class MainStore {
  @observable user: Loadable<User> = { loading: false, loaded: false };
  @observable activities: Loadable<Activity[]> = { loading: false, loaded: false };
  @observable isOnline: boolean = false;
  @observable isConnecting: boolean = false;
  
  private hub: HubConnection

  constructor() {
    makeObservable(this)
    this.hub = new HubConnectionBuilder()
      .withUrl(`${process.env.PUBLIC_URL}/hub`)
      .withAutomaticReconnect([0,0,1000])
      .build();
    onBecomeObserved(this, "activities", () => this.loadActivities());
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

  @action
  loadUser() {
    this.user.loading = true;
    this.user.loaded = false;
    fetch('/api/me').then(response => response.json()).then(json => runInAction(() => {
      this.user = { loading: false, loaded: true, obj: json.data ?? undefined };
      this.user.loading = false;
      this.user.loaded = true;
    }))
    .catch(() => {
      this.user.loading = false;
      this.user.loaded = false;
    })
    return this;
  }

  @action
  loadActivities() {
    this.activities = { loading: true, loaded: false };
    fetch('/api/activity').then(response => response.json()).then(json => runInAction(() => {
      this.activities = { loading: false, loaded: true, obj: json.data }
    }))
  }

  @action
  async createActivityAsync(activity: any) {
    const result = await fetch('/api/activity', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(activity)
    }).then(response => response.json())
    .catch(_ => { throw new Error('Network error') })
    return result;
  }

  @action
  public async lookupLocationAsync(fromText: string) {
    return Promise.resolve({ list: [
      { text: 'Alpental Maintenance Lot', coords: [47.123, -121.5234], wkid: 'abc' }
    ]})
  }
}