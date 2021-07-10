export interface Activity {
  id: string;
  title: string;
  number: string;
  startTime: number;
  endTime?: number;
  units: {
    id: string;
    knownUnitId: string;
    name: string;
  }[];
}