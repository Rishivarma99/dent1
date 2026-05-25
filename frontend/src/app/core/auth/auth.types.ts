export interface AuthUser {
  id: string;
  name: string;
  email: string;
  roles: string[];
}

export interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

export interface LoginRequest {
  usernameOrPhone: string;
  password: string;
}

export interface AuthSessionResponse extends TokenPair {
  user: AuthUser;
}
