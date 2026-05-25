import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  inject,
  signal
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { toUserFacingError } from '../../../../shared/utils/api/to-user-facing-error';
import { AuthService } from '../../../../core/auth/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly authService = inject(AuthService);

  protected readonly currentYear = new Date().getFullYear();
  protected readonly statusMessage = signal('');
  protected readonly isCredentialsSubmitting = signal(false);

  protected readonly credentialsForm = this.formBuilder.nonNullable.group({
    usernameOrPhone: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  protected async onCredentialsSubmit(): Promise<void> {
    if (this.credentialsForm.invalid) {
      this.credentialsForm.markAllAsTouched();
      return;
    }

    if (this.isCredentialsSubmitting()) {
      return;
    }

    this.isCredentialsSubmitting.set(true);
    this.statusMessage.set('');

    const { usernameOrPhone, password } = this.credentialsForm.getRawValue();

    try {
      await firstValueFrom(this.authService.login({ usernameOrPhone, password }));
      this.statusMessage.set('Signed in successfully. Redirecting...');
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/patients';
      await this.router.navigateByUrl(returnUrl);
    } catch (error) {
      console.error('Credentials sign-in failed:', error);
      this.statusMessage.set(toUserFacingError(error));
    } finally {
      this.isCredentialsSubmitting.set(false);
      this.cdr.markForCheck();
    }
  }
}

/*
Legacy Firebase login flow intentionally kept commented out for future reactivation.
It must not be enabled again until it ends in the same backend JWT/AuthService path as
credential login; otherwise it bypasses the guarded session model.

Previous imports used by this flow:
  Auth,
  ConfirmationResult,
  GoogleAuthProvider,
  RecaptchaVerifier,
  signInWithPhoneNumber,
  signInWithPopup

Previous state/methods:

  protected readonly activeTab = signal<LoginTab>('credentials');
  private recaptchaVerifier: RecaptchaVerifier | null = null;
  private recaptchaInitPromise: Promise<void> | null = null;
  private confirmationResult: ConfirmationResult | null = null;
  protected readonly recaptchaReady = signal(false);
  protected readonly otpSent = signal(false);
  protected readonly countryCodeOptions = [
    { label: 'India (+91)', value: '+91' },
    { label: 'United States (+1)', value: '+1' },
    { label: 'United Kingdom (+44)', value: '+44' },
    { label: 'UAE (+971)', value: '+971' },
    { label: 'Singapore (+65)', value: '+65' }
  ];

  protected readonly phoneForm = this.formBuilder.nonNullable.group({
    countryCode: ['+91', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\\d{10}$/)]],
    otp: ['']
  });

  protected loginWithGoogleHandle(): void { ... }
  protected async sendOtp(): Promise<void> { ... }
  protected async verifyOtpAndLogin(): Promise<void> { ... }
  protected changePhoneNumber(): void { ... }
  protected onPhoneInput(event: Event): void { ... }
  private async ensureRecaptchaReady(): Promise<void> { ... }
  private async initRecaptcha(): Promise<void> { ... }
  private setOtpValidators(): void { ... }
  private getFirebaseErrorMessage(error: unknown): string { ... }
*/
