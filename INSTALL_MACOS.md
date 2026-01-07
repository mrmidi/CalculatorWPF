# Installing Calculator on macOS

## Quick Fix for "App is damaged" Error

If you see an error saying **"CalculatorWPF is damaged and can't be opened"**, this is macOS Gatekeeper blocking an unsigned app. Here's how to fix it:

### Method 1: Using Terminal (Recommended)

1. Open **Terminal** (Applications → Utilities → Terminal)
2. Navigate to your Downloads folder:
   ```bash
   cd ~/Downloads
   ```
3. Remove the quarantine attribute:
   ```bash
   xattr -cr CalculatorWPF.app
   ```
4. Double-click the app to run it

### Method 2: Right-Click Method

1. Locate **CalculatorWPF.app** in Finder
2. **Right-click** (or Control-click) on the app
3. Select **Open** from the menu
4. Click **Open** in the dialog that appears
5. The app will now run and be trusted for future launches

### Method 3: System Settings (macOS Ventura+)

1. Try to open the app (it will be blocked)
2. Go to **System Settings** → **Privacy & Security**
3. Scroll down to the **Security** section
4. Click **Open Anyway** next to the message about CalculatorWPF
5. Click **Open** when prompted

## Why This Happens

This app is not code-signed with an Apple Developer certificate, which costs $99/year. The app is completely safe - it's built from open source code, but macOS doesn't know that.

## Building from Source

If you prefer, you can build the app yourself:

```bash
git clone https://github.com/mrmidi/CalculatorWPF.git
cd CalculatorWPF
./build-ui.sh run
```

This will compile and run the app directly from source, bypassing all security warnings.
