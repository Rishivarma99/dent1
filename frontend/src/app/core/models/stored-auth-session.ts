export interface StoredAuthSession {
  accessToken: string;
  refreshToken: string;
  userId: string;
  role: string;
  accessTokenExpiresAtUtc: string;
}
