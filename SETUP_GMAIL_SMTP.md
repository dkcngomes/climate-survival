# Setting Up Gmail SMTP for Contact Form Emails

The contact form forwards messages to `dkcngomes@gmail.com` via **Gmail SMTP**.

## Step 1: Generate a Google App Password

1. Enable **2-Step Verification** on your Google account:  
   https://myaccount.google.com/security → "2-Step Verification"

2. Generate an **App Password** for "Mail":  
   https://myaccount.google.com/apppasswords

   - Select app: **Mail**  
   - Select device: **Other** (name it "Climate Survival")  
   - Copy the 16-character password (looks like `abcd efgh ijkl mnop`)

## Step 2: Set the Password

### Option A — Environment variable (recommended for production)

```bash
# Windows (cmd)
set EMAIL__SMTPPASS=your-16-char-app-password

# Or set system-wide:
setx EMAIL__SMTPPASS "your-16-char-app-password"
```

Then restart the backend. The `appsettings.Development.json` already has the username set to `dkcngomes@gmail.com`.

### Option B — Edit appsettings.Development.json

Open `backend/appsettings.Development.json` and set:

```json
"Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "dkcngomes@gmail.com",
    "SmtpPass": "your-16-char-app-password",
    "ToAddress": "dkcngomes@gmail.com"
}
```

## How it works

- `POST /api/contact` → saves message in memory → fires email in background
- If SMTP is not configured, a warning is logged but the API **still returns success**
- Messages are always stored in-memory regardless of email delivery
