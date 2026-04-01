import { AfterViewInit, ChangeDetectionStrategy, Component, OnDestroy, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { InputOtpModule } from 'primeng/inputotp';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { Auth, RecaptchaVerifier, signInWithPopup, GoogleAuthProvider, signInWithPhoneNumber, ConfirmationResult } from '@angular/fire/auth';
import { startWith } from 'rxjs';

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    CardModule,
    DividerModule,
    InputOtpModule,
    InputTextModule,
    SelectModule
  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginPage implements AfterViewInit, OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly auth = inject(Auth);

  private recaptchaVerifier: RecaptchaVerifier | null = null;
  private confirmationResult: ConfirmationResult | null = null;
  protected readonly recaptchaReady = signal(false);
  protected readonly otpStatus = signal('');

  constructor() {
    console.log('Firebase Auth:', this.auth);
  }

  async ngAfterViewInit(): Promise<void> {
    if (this.recaptchaVerifier) {
      return;
    }

    this.recaptchaVerifier = new RecaptchaVerifier(this.auth, 'recaptcha-container', {
      size: 'normal'
    });

    try {
        console.log("recapche rendering ");
        
      await this.recaptchaVerifier.render();
      this.recaptchaReady.set(true);
      this.otpStatus.set('reCAPTCHA ready. You can send OTP now.');
    } catch (error) {
      console.error('Failed to render reCAPTCHA:', error);
      this.recaptchaReady.set(false);
      this.otpStatus.set('Failed to initialize reCAPTCHA. Disable ad-blockers and refresh.');
    }
  }

  ngOnDestroy(): void {
    if (this.recaptchaVerifier) {
      this.recaptchaVerifier.clear();
      this.recaptchaVerifier = null;
    }
    this.recaptchaReady.set(false);
    this.otpStatus.set('');
  }

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
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    otp: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
  });

//   VALUES  
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
    this.phoneForm.controls.otp.valueChanges.pipe(
      startWith(this.phoneForm.controls.otp.value)
    ),
    { initialValue: this.phoneForm.controls.otp.value }
  );

  protected readonly canSendOtp = computed(() => {
    const control = this.phoneForm.controls.phoneNumber;

    const countryCode = this.countryCodeValue();
    const value = (this.phoneNumberValue() ?? '').replace(/\D/g, ''); // Strip non-digits
    return this.recaptchaReady() && !!countryCode && value.length === 10 && /^\d{10}$/.test(value);
  });

  onPhoneInput(event: any): void {
    const input = event.target;
    let value = input.value.replace(/\D/g, ''); // Remove non-digits
    if (value.length > 10) {
      value = value.slice(0, 10);
    }
    this.phoneForm.controls.phoneNumber.setValue(value);
  }

  protected readonly canVerifyOtp = computed(() => {
    this.otpValue();
    const control = this.phoneForm.controls.otp;
    return this.otpSent() && control.valid;
  });

  protected loginWithGoogleHandle(): void {
    this.loginWithGoogle().then(res => {
      console.log('User:', res.user);
        void this.router.navigate(['/home']);
    })
    .catch(err => {
      console.error('Error:', err);
    });
  }

  private loginWithGoogle() {
    const provider = new GoogleAuthProvider();
    return signInWithPopup(this.auth, provider);
  }

  protected sendOtp(): void {
    if (!this.recaptchaVerifier) {
      console.error('reCAPTCHA is not initialized yet.');
      this.otpStatus.set('reCAPTCHA is not ready yet. Please wait a moment and retry.');
      return;
    }

    if (!this.canSendOtp()) {
      this.phoneForm.controls.countryCode.markAsTouched();
      this.phoneForm.controls.phoneNumber.markAsTouched();
      this.otpStatus.set('Enter a valid phone number with selected country code.');
      return;
    }

    this.otpStatus.set('Sending OTP...');
    const countryCode = this.phoneForm.value.countryCode!;
    const localPhoneNumber = this.phoneForm.value.phoneNumber!.replace(/\D/g, '');
    const phoneNumber = `${countryCode}${localPhoneNumber}`;

    this.loginWithPhoneNumber(phoneNumber).then(() => {
      console.log('OTP sent successfully');
      this.otpSent.set(true);
      this.otpStatus.set(`OTP sent to ${phoneNumber}`);
    }).catch((error) => {
      console.error('Error sending OTP:', error);
      this.otpStatus.set(this.getFirebaseErrorMessage(error));
    });
  }

  private async loginWithPhoneNumber(phoneNumber: string): Promise<void> {
    try {
      const result = await signInWithPhoneNumber(this.auth, phoneNumber, this.recaptchaVerifier!);
      this.confirmationResult = result;
      console.log('OTP sent successfully');
    } catch (error) {
      console.error('Error sending OTP:', error);
      throw error;
    }
  }

  protected async verifyOtpAndLogin(): Promise<void> {
    if (!this.canVerifyOtp()) {
      this.phoneForm.controls.otp.markAsTouched();
      return;
    }

    try {
      const otp = this.phoneForm.value.otp!;
      await this.verifyOtp(otp);
      console.log('OTP verified, navigating to home');
      this.otpStatus.set('OTP verified successfully. Redirecting...');
      void this.router.navigate(['/home']);
    } catch (error) {
      console.error('OTP verification failed:', error);
      this.phoneForm.controls.otp.reset();
      this.otpStatus.set(this.getFirebaseErrorMessage(error));
    }
  }

  private async verifyOtp(otp: string): Promise<any> {
    if (!this.confirmationResult) {
      throw new Error('No OTP confirmation pending. Send OTP first.');
    }
    try {
      const userCredential = await this.confirmationResult.confirm(otp);
      console.log('OTP verified successfully:', userCredential.user);
      this.confirmationResult = null;
      return userCredential;
    } catch (error) {
      console.error('Error verifying OTP:', error);
      throw error;
    }
  }

  private getFirebaseErrorMessage(error: unknown): string {
    const code = typeof error === 'object' && error !== null && 'code' in error
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