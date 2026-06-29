export interface Exercice {
  id: number;
  nom: string;
  type: string;
  description?: string | null;
  public: boolean;
  inspireSec: number;
  apneeSec: number;
  expireSec: number;
  apnee2Sec: number;
  cycles: number;
  dureeTotaleSec: number;
}