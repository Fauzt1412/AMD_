# Custom Code Backend Setup Guide

## ✅ What I've Done

### Frontend Changes:
- ✅ **Restored custom code input field** - Always visible now
- ✅ **Restored custom badge functionality** - Shows "Custom" for custom codes
- ✅ **Updated API service** - Sends custom codes to backend
- ✅ **Enhanced URL creation** - Supports both localStorage and backend modes

### Backend Changes:
- ✅ **Added IsCustom field** to URL model
- ✅ **Enhanced GenerateShortenURL service** - Supports custom codes
- ✅ **Added custom code validation** - 3-20 characters, alphanumeric only
- ✅ **Updated controller** - Handles custom codes and validation errors
- ✅ **Improved error handling** - Clear error messages for validation failures

## 🚀 Setup Instructions

### Step 1: Update Database Schema
```bash
cd Test.Server
dotnet ef migrations add AddIsCustomField
dotnet ef database update
```

### Step 2: Start Backend Server
```bash
cd Test.Server
dotnet run
```

### Step 3: Enable Backend Mode in Frontend
Edit `test.client/src/config.js`:
```javascript
export const config = {
  USE_BACKEND_API: true,  // Change this to true
  // ... rest of config
}
```

### Step 4: Test the Functionality
```bash
# Test the custom code API
node test-custom-codes.js
```

## 🎯 Features Now Available

### Custom Code Support:
- ✅ **Custom short codes** - Users can specify their own codes
- ✅ **Auto-generation fallback** - Random codes when no custom code provided
- ✅ **Validation** - Ensures codes are 3-20 characters, alphanumeric only
- ✅ **Duplicate prevention** - Prevents duplicate custom codes
- ✅ **Custom badge** - Visual indicator for custom vs auto-generated codes

### API Endpoints:
- `POST /api/URLs` - Now accepts custom codes in `shortenedUrl` field
- `GET /api/URLs` - Returns `isCustom` field for each URL
- All other endpoints work as before

## 🧪 Testing Custom Codes

### Via API (using test script):
```bash
node test-custom-codes.js
```

### Via Frontend:
1. Enable backend mode in config.js
2. Sign in to the frontend
3. Enter a URL and custom code
4. Verify the "Custom" badge appears
5. Try duplicate codes (should fail)
6. Try invalid codes (should fail)

## 📋 Custom Code Rules

### Valid Custom Codes:
- ✅ 3-20 characters long
- ✅ Letters and numbers only
- ✅ Case sensitive
- ✅ Must be unique

### Examples:
- ✅ `mylink123`
- ✅ `ABC123`
- ✅ `shorturl`
- ❌ `ab` (too short)
- ❌ `this-is-too-long-for-custom-code` (too long)
- ❌ `my-link` (contains hyphen)
- ❌ `my link` (contains space)

## 🔄 How It Works

### Frontend to Backend Flow:
1. User enters URL and optional custom code
2. Frontend sends POST request with custom code in `shortenedUrl` field
3. Backend validates custom code (if provided)
4. Backend generates random code if no custom code provided
5. Backend saves URL with `IsCustom` flag
6. Frontend displays result with custom badge if applicable

### Database Schema:
```sql
URL Table:
- Id (int, primary key)
- OriginalUrl (nvarchar)
- ShortenedUrl (nvarchar) 
- CreatedAt (datetime2)
- metadata (nvarchar, nullable)
- IsCustom (bit) -- NEW FIELD
```

## 🛠 Troubleshooting

### Migration Issues:
```bash
# If migration fails, try:
cd Test.Server
dotnet ef database drop
dotnet ef database update
```

### Custom Code Validation Errors:
- Check code length (3-20 characters)
- Ensure only letters and numbers
- Verify code isn't already taken

### Frontend Not Showing Custom Features:
- Ensure `USE_BACKEND_API: true` in config.js
- Check browser console for API errors
- Verify backend is running on https://localhost:7282

## 🎉 Success Indicators

You'll know everything is working when:
- ✅ Custom code input field is visible
- ✅ Custom codes are accepted and saved
- ✅ "Custom" badge appears for custom codes
- ✅ Duplicate codes are rejected with clear error
- ✅ Invalid codes are rejected with clear error
- ✅ Auto-generated codes work when no custom code provided
- ✅ All URLs persist in database between sessions

The backend now fully supports the frontend's custom code functionality! 🚀