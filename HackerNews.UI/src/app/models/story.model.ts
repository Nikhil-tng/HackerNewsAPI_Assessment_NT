export interface Story {
  id: number;
  title: string;
  type: string;
  url?: string;
  by: string;
  descendants: number;
  score: number;
  time: number;
}
