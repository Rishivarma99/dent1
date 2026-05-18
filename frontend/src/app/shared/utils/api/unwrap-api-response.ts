import { ApiResponse } from '../../types/api/api-response';
import { ApiBusinessError } from './api-business-error';

export function unwrapApiResponse<T>(response: ApiResponse<T>): T {
  if (!response.success || response.data == null) {
    const message = response.error?.message ?? 'Unknown API error';
    throw new ApiBusinessError(response.error?.code ?? 'UNKNOWN', message);
  }
  return response.data;
}
