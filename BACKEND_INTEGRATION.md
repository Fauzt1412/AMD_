# Backend Integration Guide

The frontend is now configured to work with both localStorage (default) and the backend API.

## Current Status

- **Frontend Mode**: localStorage (working)
- **Backend Mode**: Available but disabled by default

## How to Enable Backend Integration

### Step 1: Start the Backend Server

1. Open a terminal and navigate to the backend directory:
   ```bash
   cd Test.Server
   ```

2. Restore packages and update database:
   ```bash
   dotnet restore
   dotnet ef database update
   ```

3. Start the backend server:
   ```bash
   dotnet run
   ```

   The server should start on:
   - HTTPS: https://localhost:7282
   - Swagger: https://localhost:7282/swagger

### Step 2: Enable Backend Mode in Frontend

1. Open `test.client/src/config.js`

2. Change the configuration:
   ```javascript
   export const config = {
     // Change this from false to true
     USE_BACKEND_API: true,
     
     API_BASE_URL: '/api',
     APP_NAME: 'LinkShort',
     DEFAULT_SHORT_DOMAIN: 'https://short.ly'
   }
   ```

3. Save the file

### Step 3: Start the Frontend

1. Open another terminal and navigate to the frontend directory:
   ```bash
   cd test.client
   ```

2. Install dependencies (if not already done):
   ```bash
   npm install
   ```

3. Start the frontend development server:
   ```bash
   npm run dev
   ```

   The frontend should start on: https://localhost:58242

## Features in Each Mode

### localStorage Mode (Default)
- ✅ Custom short codes
- ✅ Click tracking (frontend only)
- ✅ User-specific URL storage
- ✅ Works offline
- ❌ No persistent database
- ❌ No server-side features

### Backend API Mode
- ✅ Persistent database storage
- ✅ Server-side URL generation
- ✅ Redis caching (with fallback)
- ✅ RESTful API endpoints
- ❌ Custom short codes (not implemented yet)
- ❌ Click tracking (not implemented yet)

## Troubleshooting

### Backend Won't Start
- Ensure .NET 8 SDK is installed
- Check if SQL Server LocalDB is running
- Verify connection strings in appsettings.json

### Frontend API Calls Failing
- Verify backend is running on https://localhost:7282
- Check browser console for CORS errors
- Ensure `USE_BACKEND_API: true` in config.js

### Database Errors
- Run `dotnet ef database update` in Test.Server directory
- Check SQL Server LocalDB is installed and running

## API Endpoints (Backend Mode)

When backend mode is enabled, the following endpoints are available:

- `GET /api/URLs` - Get all shortened URLs
- `POST /api/URLs` - Create a new shortened URL
- `GET /api/URLs/redirect/{shortCode}` - Redirect to original URL
- `GET /api/URLs/validate/{shortCode}` - Validate a short code
- `DELETE /api/URLs/{id}` - Delete a shortened URL

## Switching Between Modes

You can easily switch between localStorage and backend modes by changing the `USE_BACKEND_API` setting in `config.js`. The application will automatically:

- Use the appropriate data source
- Show/hide features based on the mode
- Fallback to localStorage if backend is unavailable

## Future Enhancements

To fully utilize the backend, the following features could be added:

1. **Custom Short Codes**: Modify backend to accept custom codes
2. **Click Tracking**: Implement server-side click analytics
3. **User Authentication**: Add proper user management
4. **Bulk Operations**: Support for bulk URL operations
5. **Analytics Dashboard**: Detailed usage statistics