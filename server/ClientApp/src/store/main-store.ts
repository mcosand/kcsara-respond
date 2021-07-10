import { action, computed, makeObservable, observable, onBecomeObserved, runInAction, toJS } from "mobx";
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Campaign as ActivityIcon, AssignmentInd as MeIcon, Air as OtherIcon } from "@material-ui/icons";
import { createBrowserHistory } from 'history';

import { Activity } from "./model/activity";
import { User } from "./model/user";
import { Loadable } from "./model/loadable";
import { GeoJsonFeature } from "./model/geoJsonFeature";
import { Group } from "./model/group";
import { Branding } from "./model/branding";
import { Member } from "./model/member";

export interface RouteInfo {
  path: string,
  name: string
}

export class MainStore {
  @observable user: Loadable<User> = { loading: false, loaded: false };
  @observable member?: Member;
  @observable units: Loadable<Group[]> = { loading: false, loaded: false, obj: []};
  @observable activities: Loadable<Activity[]> = { loading: false, loaded: false };
  @observable isOnline: boolean = false;
  @observable isConnecting: boolean = false;
  @observable locationSearchResults: Loadable<GeoJsonFeature[]> = { loading: false, loaded: true, obj: []};
  @observable branding: Branding = { name: 'KCSARA', color: '#F36E20' }
  private hub: HubConnection
  @observable private routeMap: {[key: string]: RouteInfo} = {}
  @observable pathname: string;
  @observable parentSite: string = '/';

  history = createBrowserHistory();

  constructor() {
    makeObservable(this)
    this.hub = new HubConnectionBuilder()
      .withUrl(`${process.env.PUBLIC_URL}/hub`)
      .withAutomaticReconnect([0,0,1000])
      .build();
    onBecomeObserved(this, "activities", () => this.loadActivities());
    this.pathname = this.history.location.pathname;
  }

  @computed
  get breadcrumbs() {
    const parts = this.pathname.split('/').filter(f => f);

    const getName = (to: string) => {
      let name = this.routeMap[to] ? this.routeMap[to].name : to;
      if (parts.length === 1) name = this.branding.name + ' ' + name;
      return name;
    }

    return parts.map((_p, i) => {
      const to = `/${parts.slice(0, i + 1).join('/')}`;
      return { to, name: getName(to), isLast: i === parts.length - 1 };
    });
  }

  @computed get bottomBarActions() {
    return [
      { path: '/activity', name:'Activity', Icon: ActivityIcon, isPrimary: false },
      { path: '/me', name:'My Status', Icon: MeIcon, isPrimary: true },
      { path: '#', name:'??', Icon: OtherIcon, isPrimary: false },
    ].map(a => ({ ...a, isSelected: this.pathname.startsWith(a.path) }));
  }

  @computed get belongsToSite() {
    return !this.branding.id || (this.member?.groups ?? []).filter(g => g.id === this.branding.id).length > 0;
  }

  @action
  wire() {
    this.history.listen(location => runInAction(() => this.pathname = location.pathname));
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
    this.hub.on('Initialize', message => runInAction(() => this.handleStartupData(message)));
    this.hub.start().then(() => runInAction(() => {
      this.isConnecting = false;
      this.isOnline = true;
      console.log('wiring hub events')
    }))
    .catch(() => runInAction(() => {
      this.isConnecting = false;
      this.isOnline = false;
    }));

    return this;
  }

  @action
  handleStartupData(json: string) {
    const data = JSON.parse(json).data;
    console.log('handleStartupData', this, data)
    this.user = { loading: false, loaded: true, obj: data.user ?? undefined };
    this.member = data.member;
    this.units = { loading: false, loaded: true, obj: data.units ?? [] };
    this.branding = data.branding ?? this.branding;
    this.parentSite = data.parentSite;
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
    if (!fromText) {
      this.locationSearchResults = { loading: false, loaded: true, obj: [] };
      return;
    }

    this.locationSearchResults.loading = true;
    return await fetch(`/api/locations?category=command&q=${encodeURIComponent(fromText)}`)
        .then(response => response.json())
        .then(json => runInAction(() => {
          this.locationSearchResults.loading = false;
          this.locationSearchResults.obj = json.data.features;
        }));
  }

  @action
  public registerRoutes(list: { path: string, name: string }[]) {
    for (var i=0; i<list.length; i++) {
      this.routeMap[list[i].path] = list[i];
    }
    //console.log('register routes', list, JSON.parse(JSON.stringify(this.routeMap)));
  }
}