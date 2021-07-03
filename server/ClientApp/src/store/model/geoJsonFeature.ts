export type GeoJsonCoordinate = [number,number]

export interface GeoJsonFeature {
  id: string,
  geometry: {
    type: 'Point'|'LineString'|'Polygon',
    coordinates: GeoJsonCoordinate | GeoJsonCoordinate[] | GeoJsonCoordinate[][],
  },
  properties: {
    [key: string]: any,
  },
}