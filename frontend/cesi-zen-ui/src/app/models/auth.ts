export interface AuthUser {
  id: number;
  nom: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  user: AuthUser;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  nom: string;
  email: string;
  password: string;
  consentement: boolean; // [SÉCU 4] consentement RGPD
}
