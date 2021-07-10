// import { DateTime } from 'luxon';
import { action, makeObservable, observable, runInAction } from "mobx";
import { MainStore } from '../../../store/main-store';
// import { GeoJsonFeature } from '../../../store/model/geoJsonFeature';
// import { Loadable } from '../../../store/model/loadable';

export class MyStatusUiStore {
  private readonly mainStore: MainStore;

  constructor(mainStore: MainStore) {
    this.mainStore = mainStore;
    //makeObservable(this)
  }
}