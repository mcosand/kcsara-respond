export interface Member {
  id: string;
  number: string;
  groups: { id: string, name: string }[];
  name: string;
  email: string;
}