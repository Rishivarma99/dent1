import { HttpErrorResponse } from '@angular/common/http';
import { ApiBusinessError } from './api-business-error';
import { parseApiErrorBody } from './parse-api-error-body';

export function toUserFacingError(err: unknown): string {
  if (err instanceof ApiBusinessError) {
    return err.message;
  }

  if (err instanceof HttpErrorResponse) {
    const envelopeError = parseApiErrorBody(err.error);
    if (envelopeError) {
      return envelopeError.message;
    }

    if (typeof err.error === 'string' && err.error.trim()) {
      return err.error;
    }

    return 'Something went wrong. Please try again.';
  }

  if (err instanceof Error && err.message) {
    return err.message;
  }

  return 'Unexpected error. Please try again.';
}
