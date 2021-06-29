import { makeObservable, observable } from "mobx";

import { Activity } from "./model/activity";

export class MainStore {
  @observable activities: Activity[] = []
  @observable isOffline: boolean = false;

  constructor() {
    makeObservable(this)
  }

  wire() {
    return this;
  }
}