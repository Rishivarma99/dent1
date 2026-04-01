# Netlify Deployment Guide for Dent1 Angular App

## 📋 Quick Steps

### Step 1: Push to GitHub
```bash
git push origin main
```

### Step 2: Connect to Netlify
1. Go to [netlify.com](https://netlify.com)
2. Click "Add new site" → "Import an existing project"
3. Select GitHub
4. Authorize Netlify to access your repo
5. Select your `dent1` repository
6. Click "Deploy site"

### Step 3: Set Environment Variables in Netlify
1. Go to your site in Netlify Dashboard
2. Click **Site Settings** → **Build & Deploy** → **Environment**
3. Click **Add environment variable**
4. Add these variables:

```
NG_APP_FIREBASE_API_KEY = AIzaSyDVXsVNL3YN_BUn8ojk-m1MsTYfZI5IEdI
NG_APP_FIREBASE_AUTH_DOMAIN = dent1-c9b86.firebaseapp.com
NG_APP_FIREBASE_PROJECT_ID = dent1-c9b86
NG_APP_FIREBASE_STORAGE_BUCKET = dent1-c9b86.firebasestorage.app
NG_APP_FIREBASE_MESSAGING_SENDER_ID = 84647432096
NG_APP_FIREBASE_APP_ID = 1:84647432096:web:7a4b759d2151814d581ec5
```

### Step 4: Update Authorized Domains in Firebase
1. Go to [Firebase Console](https://console.firebase.google.com)
2. Select your project `dent1-c9b86`
3. Go to **Authentication** → **Settings**
4. Scroll to **Authorized domains**
5. Click **Add domain**
6. Add your Netlify domain (e.g., `your-site-name.netlify.app`)
7. Save

### Step 5: Verify Deployment
1. Visit your Netlify domain
2. Test Google login
3. Test Phone OTP with a Firebase test number (set in Auth → Phone)
4. Check browser Network tab (F12) for any errors

---

## 🔧 Build Configuration

### Verify `angular.json`
Your build command should be:
```json
{
  "projects": {
    "frontend": {
      "architect": {
        "build": {
          "builder": "@angular-devkit/build-angular:browser",
          "options": {
            "outputPath": "dist/frontend",
            "index": "src/index.html",
            "main": "src/main.ts",
            ...
          }
        }
      }
    }
  }
}
```

### Add `netlify.toml` (Optional but Recommended)

Create `frontend/netlify.toml`:

```toml
[build]
command = "npm run build"
publish = "dist/frontend"

[build.environment]
NODE_VERSION = "20"

[[redirects]]
from = "/*"
to = "/index.html"
status = 200
```

This enables:
- SPA routing (important for Angular)
- Sets correct Node version
- Specifies build command

---

## 🔐 Security Best Practices

### ✅ DO:
- Store sensitive keys in **Netlify Environment Variables** (not in code)
- Add `.env` to `.gitignore` ✅ (Already done)
- Use different Firebase projects for dev/prod if needed
- Enable Firebase security rules:
  ```javascript
  // For development only (CHANGE for production)
  rules_version = '2';
  service cloud.firestore {
    match /databases/{database}/documents {
      match /{document=**} {
        allow read, write: if request.auth != null;
      }
    }
  }
  ```

### ❌ DON'T:
- Commit `.env` file to GitHub ❌ (Won't happen - it's in `.gitignore`)
- Hardcode API keys in source files (Your environment.ts now reads from env vars ✅)
- Expose Firebase config in plain JavaScript (Firebase SDK handles this safely ✅)

---

## 🚨 Troubleshooting

### "Environment variables not loading"
- Check Netlify Dashboard → **Deployments** → Trigger redeploy
- Wait 2-3 minutes for env vars to take effect
- Clear browser cache (Ctrl+Shift+Delete)

### "Phone Auth not working on Netlify"
1. Check authorized domains in Firebase
2. Check browser Network tab (F12) for reCAPTCHA errors
3. Verify Firebase project has Phone Auth enabled
4. Check Netlify env vars are set correctly:
   - Go to Deployments → Click latest build → Expand "Build log"
   - Search for your env vars to confirm they're injected

### "Google login fails on Netlify"
1. Firebase Console → Authentication → Settings
2. Add your Netlify domain to **Authorized domains**
3. Check that OAuth redirect URI is correct
4. Try incognito mode (clear cached redirects)

---

## 📊 Environment Variables Used

| Variable | Value | Purpose |
|----------|-------|---------|
| `NG_APP_FIREBASE_API_KEY` | `AIzaSy...` | Firebase authentication API key |
| `NG_APP_FIREBASE_AUTH_DOMAIN` | `dent1-c9b86.firebaseapp.com` | Auth domain for redirects |
| `NG_APP_FIREBASE_PROJECT_ID` | `dent1-c9b86` | Firebase project ID |
| `NG_APP_FIREBASE_STORAGE_BUCKET` | `dent1-c9b86.firebasestorage.app` | Storage bucket for files |
| `NG_APP_FIREBASE_MESSAGING_SENDER_ID` | `84647432096` | For push notifications |
| `NG_APP_FIREBASE_APP_ID` | `1:84647432096:web:...` | Firebase app ID |

**Note:** All `NG_APP_*` variables are baked into the build and visible in browser DevTools. Only add sensitive data like API keys that are public-facing anyway (Firebase is designed to work with public keys).

---

## 🎯 Local Development

To test locally with the same env vars:

```bash
# Windows PowerShell
$env:NG_APP_FIREBASE_API_KEY="AIzaSyDVXsVNL3YN_BUn8ojk-m1MsTYfZI5IEdI"
npm start

# macOS/Linux bash
export NG_APP_FIREBASE_API_KEY="AIzaSyDVXsVNL3YN_BUn8ojk-m1MsTYfZI5IEdI"
npm start

# Or use .env file (easier)
npm start
```

The `.env` file in the repo root will be automatically loaded by Angular CLI.

---

## 🚀 Deployment Checklist

- [ ] Push code to GitHub
- [ ] Connect GitHub repo to Netlify
- [ ] Set 6 environment variables in Netlify Dashboard
- [ ] Add Netlify domain to Firebase authorized domains
- [ ] Verify Phone Auth is enabled in Firebase
- [ ] Test Google login on Netlify domain
- [ ] Test Phone OTP with Firebase test number
- [ ] Check Network tab (F12) for any 401/403 errors
- [ ] Monitor build logs for any issues

---

## Questions?

If deployment fails:
1. Check Netlify build logs (Deployments → Build log)
2. Check browser console (F12 → Console)
3. Check browser network (F12 → Network) for failed requests
4. Verify Firebase console settings match your domain
