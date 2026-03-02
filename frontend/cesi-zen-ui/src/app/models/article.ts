export interface Article {
  id: number;
  titre: string;
  contenu: string;
  datePublication?: string | null;
  public: boolean;
  categorieId: number;
}