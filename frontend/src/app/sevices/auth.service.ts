import { Injectable } from '@angular/core';
import { Auth, signInWithPopup, GoogleAuthProvider, signInWithPhoneNumber, ConfirmationResult } from '@angular/fire/auth';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private confirmationResult: ConfirmationResult | null = null;

  constructor(private auth: Auth) {}

  loginWithGoogle() {
    const provider = new GoogleAuthProvider();
    return signInWithPopup(this.auth, provider);
  }

  async loginWithPhoneNumber(phoneNumber: string, recaptchaVerifier: any): Promise<ConfirmationResult> {
    try {
      const result = await signInWithPhoneNumber(this.auth, phoneNumber, recaptchaVerifier);
      this.confirmationResult = result;
      console.log('OTP sent successfully');
      return result;
    } catch (error) {
      console.error('Error sending OTP:', error);
      throw error;
    }
  }

  async verifyOtp(otp: string): Promise<any> {
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
}