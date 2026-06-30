import { Exercice } from './exercice';

export interface Enregistre {
  id: number;
  historiqueId: number;
  exerciceId: number;
  dateDebut: string;
  dureeEffectiveSec: number;
  exercice?: Exercice;
}

export interface Historique {
  id: number;
  utilisateurId: number;
  date: string;
  dureeSec: number;
  exercicesEnregistres: Enregistre[];
}