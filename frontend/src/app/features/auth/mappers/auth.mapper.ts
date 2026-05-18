import { StoredAuthSession } from '../../../core/models/stored-auth-session';
import { AuthResponseDto } from '../api/dtos/auth-response.dto';

export function mapAuthResponseDtoToStoredSession(dto: AuthResponseDto): StoredAuthSession {
  return {
    accessToken: dto.accessToken,
    refreshToken: dto.refreshToken,
    userId: dto.userId,
    role: dto.role,
    accessTokenExpiresAtUtc: dto.accessTokenExpiresAtUtc
  };
}
