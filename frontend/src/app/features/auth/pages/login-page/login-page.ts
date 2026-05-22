import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnDestroy,
  computed,
  inject,
  signal
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthApiService } from '../../api/auth-api.service';
import { mapAuthResponseDtoToStoredSession } from '../../mappers/auth.mapper';
import { getDefaultLandingPath } from '../../../../core/constants/clinic-roles';
import { TokenStorageService } from '../../../../core/services/token-storage.service';
import { toUserFacingError } from '../../../../shared/utils/api/to-user-facing-error';
import {
  Auth,
  ConfirmationResult,
  GoogleAuthProvider,
  RecaptchaVerifier,
  signInWithPhoneNumber,
  signInWithPopup
} from '@angular/fire/auth';
import { HttpErrorResponse } from '@angular/common/http';
import { ButtonModule } from 'primeng/button';
import { InputOtpModule } from 'primeng/inputotp';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { startWith } from 'rxjs';

type LoginTab = 'credentials' | 'mobile';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, ButtonModule, InputOtpModule, InputTextModule, SelectModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginPage implements AfterViewInit, OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly auth = inject(Auth);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly authApi = inject(AuthApiService);
  private readonly tokenStorage = inject(TokenStorageService);

  protected readonly currentYear = new Date().getFullYear();
  protected readonly activeTab = signal<LoginTab>('credentials');
  protected readonly isSubmittingCredentials = signal(false);

  private recaptchaVerifier: RecaptchaVerifier | null = null;
  private recaptchaInitPromise: Promise<void> | null = null;
  private confirmationResult: ConfirmationResult | null = null;
  protected readonly recaptchaReady = signal(false);
  protected readonly statusMessage = signal('');
  protected readonly isCredentialsSubmitting = signal(false);
  protected readonly otpSent = signal(false);

  protected readonly countryCodeOptions = [
    { label: 'India (+91)', value: '+91' },
    { label: 'United States (+1)', value: '+1' },
    { label: 'United Kingdom (+44)', value: '+44' },
    { label: 'UAE (+971)', value: '+971' },
    { label: 'Singapore (+65)', value: '+65' }
  ];

  protected readonly credentialsForm = this.formBuilder.nonNullable.group({
    usernameOrPhone: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  protected readonly phoneForm = this.formBuilder.nonNullable.group({
    countryCode: ['+91', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    otp: ['']
  });

  private readonly countryCodeValue = toSignal(
    this.phoneForm.controls.countryCode.valueChanges.pipe(
      startWith(this.phoneForm.controls.countryCode.value)
    ),
    { initialValue: this.phoneForm.controls.countryCode.value }
  );

  private readonly phoneNumberValue = toSignal(
    this.phoneForm.controls.phoneNumber.valueChanges.pipe(
      startWith(this.phoneForm.controls.phoneNumber.value)
    ),
    { initialValue: this.phoneForm.controls.phoneNumber.value }
  );

  private readonly otpValue = toSignal(
    this.phoneForm.controls.otp.valueChanges.pipe(startWith(this.phoneForm.controls.otp.value)),
    { initialValue: this.phoneForm.controls.otp.value }
  );

  protected readonly isPhoneValid = computed(() => {
    const countryCode = this.countryCodeValue();
    const value = (this.phoneNumberValue() ?? '').replace(/\D/g, '');
    return !!countryCode && value.length === 10 && /^\d{10}$/.test(value);
  });

  protected readonly canVerifyOtp = computed(() => {
    this.otpValue();
    const control = this.phoneForm.controls.otp;
    return this.otpSent() && control.valid;
  });

  ngAfterViewInit(): void {
    void this.ensureRecaptchaReady();
  }

  ngOnDestroy(): void {
    if (this.recaptchaVerifier) {
      this.recaptchaVerifier.clear();
      this.recaptchaVerifier = null;
    }
    this.recaptchaReady.set(false);
    this.statusMessage.set('');
  }

  protected setActiveTab(tab: LoginTab): void {
    this.activeTab.set(tab);
    if (tab === 'mobile') {
      void this.ensureRecaptchaReady();
    }
  }

  private async ensureRecaptchaReady(): Promise<void> {
    if (this.recaptchaReady()) {
      return;
    }

    if (this.recaptchaInitPromise) {
      return this.recaptchaInitPromise;
    }

    this.recaptchaInitPromise = this.initRecaptcha();
    try {
      await this.recaptchaInitPromise;
    } finally {
      this.recaptchaInitPromise = null;
    }
  }

  private async initRecaptcha(): Promise<void> {
    if (!document.getElementById('recaptcha-container')) {
      return;
    }

    if (!this.recaptchaVerifier) {
      this.recaptchaVerifier = new RecaptchaVerifier(this.auth, 'recaptcha-container', {
        size: 'normal'
      });
    }

    try {
      await this.recaptchaVerifier.render();
      this.recaptchaReady.set(true);
      this.cdr.markForCheck();
    } catch (error) {
      console.error('Failed to render reCAPTCHA:', error);
      this.recaptchaReady.set(false);
      this.statusMessage.set('Failed to initialize reCAPTCHA. Disable ad-blockers and refresh.');
      this.cdr.markForCheck();
    }
  }

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
      const response = await firstValueFrom(
        this.authApi.login({ usernameOrPhone, password })
      );
      this.tokenStorage.saveSession(mapAuthResponseDtoToStoredSession(response));
      this.statusMessage.set('Signed in successfully. Redirecting...');
      await this.router.navigateByUrl(getDefaultLandingPath(this.tokenStorage.getRole()));
    } catch (error) {
      console.error('Credentials sign-in failed:', error);
      this.statusMessage.set(toUserFacingError(error));
    } finally {
      this.isCredentialsSubmitting.set(false);
      this.cdr.markForCheck();
    }
  }

  protected onPhoneInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    if (value.length > 10) {
      value = value.slice(0, 10);
    }
    this.phoneForm.controls.phoneNumber.setValue(value);
  }

  protected changePhoneNumber(): void {
    this.otpSent.set(false);
    this.confirmationResult = null;
    this.phoneForm.controls.otp.reset();
    this.phoneForm.controls.otp.clearValidators();
    this.phoneForm.controls.otp.updateValueAndValidity();
    this.statusMessage.set('');
  }

  private setOtpValidators(): void {
    this.phoneForm.controls.otp.setValidators([
      Validators.required,
      Validators.minLength(6),
      Validators.maxLength(6)
    ]);
    this.phoneForm.controls.otp.updateValueAndValidity();
  }

  protected loginWithGoogleHandle(): void {
    const provider = new GoogleAuthProvider();
    signInWithPopup(this.auth, provider)
      .then(() => {
        void this.router.navigateByUrl(getDefaultLandingPath(this.tokenStorage.getRole()));
      })
      .catch((err) => {
        console.error('Google sign-in failed:', err);
        this.statusMessage.set('Google sign-in failed. Please try again.');
      });
  }

  protected async sendOtp(): Promise<void> {
    if (!this.isPhoneValid()) {
      this.phoneForm.controls.countryCode.markAsTouched();
      this.phoneForm.controls.phoneNumber.markAsTouched();
      this.statusMessage.set('Enter a valid 10-digit phone number with country code.');
      return;
    }

    await this.ensureRecaptchaReady();

    if (!this.recaptchaVerifier || !this.recaptchaReady()) {
      this.statusMessage.set('reCAPTCHA is not ready yet. Complete the checkbox above and try again.');
      return;
    }

    this.statusMessage.set('Sending OTP...');
    const countryCode = this.phoneForm.value.countryCode!;
    const localPhoneNumber = this.phoneForm.value.phoneNumber!.replace(/\D/g, '');
    const phoneNumber = `${countryCode}${localPhoneNumber}`;

    signInWithPhoneNumber(this.auth, phoneNumber, this.recaptchaVerifier)
      .then((result) => {
        this.confirmationResult = result;
        this.otpSent.set(true);
        this.setOtpValidators();
        this.statusMessage.set(`OTP sent to ${phoneNumber}`);
        this.cdr.markForCheck();
      })
      .catch((error) => {
        console.error('Error sending OTP:', error);
        this.statusMessage.set(this.getFirebaseErrorMessage(error));
      });
  }

  protected async verifyOtpAndLogin(): Promise<void> {
    if (!this.canVerifyOtp()) {
      this.phoneForm.controls.otp.markAsTouched();
      return;
    }

    if (!this.confirmationResult) {
      this.statusMessage.set('No OTP confirmation pending. Send OTP first.');
      return;
    }

    try {
      const otp = this.phoneForm.value.otp!;
      await this.confirmationResult.confirm(otp);
      this.confirmationResult = null;
      this.statusMessage.set('OTP verified successfully. Redirecting...');
      void this.router.navigateByUrl(getDefaultLandingPath(this.tokenStorage.getRole()));
    } catch (error) {
      console.error('OTP verification failed:', error);
      this.phoneForm.controls.otp.reset();
      this.statusMessage.set(this.getFirebaseErrorMessage(error));
    }
  }

  private getFirebaseErrorMessage(error: unknown): string {
    const code =
      typeof error === 'object' && error !== null && 'code' in error
        ? String((error as { code?: string }).code)
        : '';

    switch (code) {
      case 'auth/invalid-phone-number':
        return 'Invalid phone number format. Use country code + number.';
      case 'auth/too-many-requests':
        return 'Too many attempts. Please wait and try again later.';
      case 'auth/quota-exceeded':
        return 'SMS quota exceeded for this project. Use Firebase test numbers or try later.';
      case 'auth/captcha-check-failed':
        return 'reCAPTCHA verification failed. Refresh page and try again.';
      case 'auth/invalid-app-credential':
        return 'App credential invalid. Verify Firebase authorized domain and project config.';
      case 'auth/code-expired':
        return 'OTP expired. Request a new OTP.';
      case 'auth/invalid-verification-code':
        return 'Incorrect OTP. Please enter the latest code.';
      default:
        return `OTP request failed${code ? ` (${code})` : ''}. Check Firebase Phone Auth settings and authorized domains.`;
    }
  }
}
