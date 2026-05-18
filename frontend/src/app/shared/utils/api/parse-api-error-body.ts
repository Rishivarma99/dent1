import { ApiError, ApiResponse } from '../../types/api/api-response';

export function parseApiErrorBody(body: unknown): ApiError | null {
  if (typeof body !== 'object' || body === null) {
    return null;
  }

  const envelope = body as Partial<ApiResponse<unknown>>;

  if (envelope.success !== false || !envelope.error) {
    return null;
  }

  const { code, message } = envelope.error;

  if (typeof code !== 'string' || typeof message !== 'string') {
    return null;
  }

  return { code, message };
}
