import { DateTime } from 'luxon';
import { action, makeObservable, observable, runInAction } from "mobx";
import { MainStore } from '../../../store/main-store';
import { GeoJsonFeature } from '../../../store/model/geoJsonFeature';
import { Loadable } from '../../../store/model/loadable';

interface Validatable<T> {
  error?: string;
  value: T;
}

export class ActivityCreateUIStore {
  private readonly mainStore: MainStore;

  @observable startTime: DateTime = DateTime.now();
  @observable title: Validatable<string> = { value: "" };
  @observable demNumber: Validatable<string|undefined> = { value: undefined };
  @observable locationText: Validatable<string> = {value: "" };
  get locationSearchResults(): Loadable<GeoJsonFeature[]> { return this.mainStore.locationSearchResults };
  @observable selectedLocation: GeoJsonFeature|null = null;

  @observable selectedUnits: Validatable<{[unitId:string]: boolean }> = { value: {} };
  @observable availableUnits: { name: string, id: string }[] = [ {name:'ESAR', id:'esr'}, { name:'SMR', id:'smx'}];
  @observable createMap: boolean = true;

  @observable saving: boolean = false;
  @observable saveError?: string;

  public validations = {
    units: action(() => {
      const values = Object.values(this.selectedUnits.value);
      this.selectedUnits.error = (values.length > 0 && values.filter(f => f).length > 0) ? undefined : 'Must select at least one unit';
      return Promise.resolve(!this.selectedUnits.error);
    }),
    location: action(() => {
      this.locationText.error = this.selectedLocation ? undefined : 'Required';
      return Promise.resolve(!this.locationText.error);
    }),
    title: action(() => {
      this.title.error = !this.title.value ? 'Required' : undefined;
      return Promise.resolve(!this.title.error);
    }),
    number: action(() => {
      this.demNumber.error = /^((\d{2}-\d{4})|(\d{2}-ES-\d{3})|(\d{2}-T-\d{4})|)$/.test(this.demNumber.value ?? '')
      ? undefined
      : 'Must be in form 20-1234, 20-ES-123, or 20-T-1234';
      return Promise.resolve(!this.demNumber.error);
    })
  }

  constructor(mainStore: MainStore) {
    this.mainStore = mainStore;
    makeObservable(this)
  }

  @action
  setDate(date: DateTime | null) {
    this.startTime = date ?? this.startTime;
  }

  @action
  setTitle(title: string) {
    this.title = { value: title, error: undefined };
  }

  @action
  setNumber(number: string) {
    this.demNumber = { value: number || undefined };
  }

  @action
  setLocationText(text: string) {
    this.locationText = { value: text, error: undefined };
    this.selectedLocation = null;
    this.mainStore.lookupLocationAsync(text);
  }
  
  @action
  setSelectedLocation(location: GeoJsonFeature|null) {
    this.selectedLocation = location;
    this.locationText = {value: location?.properties?.title ?? '', error: undefined };
  }

  areLocationsEqual(a: GeoJsonFeature, b: GeoJsonFeature) {
    if (a.id && a.id === b.id) return true;
    if (JSON.stringify(a.geometry.coordinates) !== JSON.stringify(b.geometry.coordinates)) return false;
    return a.properties.title === b.properties.title;
  }

  @action
  toggleUnit(unitId: string, selected: boolean) {
    this.selectedUnits.value[unitId] = selected;
    this.validations.units();
  }

  @action
  setCreateMap(create: boolean) {
    this.createMap = create;
  }

  async validateAsync() {
    const validationResults = await Promise.all(Object.values(this.validations).map(v => v()));
    const valid = validationResults.reduce((accum,cur) => accum && cur, true);
    return valid;
  }

  @action
  submit() {
    this.validateAsync().then(result => {
      if (!result) {
        return;
      }
      runInAction(() => {
        this.saving = true;
        this.saveError = undefined;
    
        return this.mainStore.createActivityAsync({
          number: this.demNumber.value,
          title: this.title.value,
          startTime: this.startTime.toMillis(),
          location: this.selectedLocation,
          createMap: this.createMap,
          units: Object.entries(this.selectedUnits.value).filter(([_, value]) => value).map(([key]) => key)
        }).then(result => runInAction(() => {
          console.log('finished saving', result);
          this.saving = false;
          this.saveError = undefined;
        })).catch(err => runInAction(() => {
          this.saving = false;
          this.saveError = err.message;
        }))
      });
    })
  }
}
