export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  userId: string;
  role: string;
  accessTokenExpiresAtUtc: string;
  permissions: string[];
}
