# Brewora Café & Kitchen

Premium café ordering and table reservation app: **Brewora — Café & Kitchen**.

The mobile UI follows the provided HTML reference (cream canvas, forest green, muted gold, Cormorant Garamond + DM Sans + Parisienne). Do not treat this as a generic food-delivery template.

## Layout

```
Brewora/
├── mobile/          React + Vite + Capacitor (Android: com.brewora.cafe)
├── backend/         ASP.NET Core Web API (.NET 10)
│   ├── Brewora.API
│   ├── Brewora.Application
│   ├── Brewora.Domain
│   └── Brewora.Infrastructure
├── database/        SQL Server tables, stored procedures, seed
└── README.md
```

## Run locally

### API

```bash
cd backend
dotnet run --project Brewora.API
```

Listens on `http://127.0.0.1:17421`.

Default catalog and demo users live in an in-memory store so the app runs without SQL Server. To use Microsoft SQL Server:

1. Run `database/Tables.sql`, `StoredProcedures.sql`, then `SeedData.sql` against `BreworaDB`.
2. Set `Data:UseSqlServer` to `true` and `ConnectionStrings:SqlServer` in `appsettings.json` or environment variables.
3. Register demo users (passwords are hashed with BCrypt, never stored in plain text):
   - Customer: `customer@email.com` / `Customer@123`
   - Admin: `nina.v@example.com` / `Admin@123`

There is **no Entity Framework**. Repositories use `SqlConnection` / `SqlCommand` / `SqlDataReader` and stored procedures (`Microsoft.Data.SqlClient`).

### Mobile web (Windows)

API chalu rakho, phir `D:\Brewora\run-mobile.bat` double-click karo.

Ya:

```bash
cd mobile
cp .env.example .env
npm install
npm run dev
```

Dev server: `http://127.0.0.1:43123` (proxies `/api` to the backend).

### Android (Capacitor)

```bash
cd mobile
npm run build
npx cap add android   # first time
npx cap sync android
npx cap open android
```

- Application ID: `com.brewora.cafe`
- App name: Brewora Café & Kitchen

## Payments (Razorpay test mode)

Flow: React → `POST /api/payment/create-order` → Razorpay Orders API → Checkout → `POST /api/payment/verify` (HMAC SHA256 of `order_id|payment_id`) → persist payment → confirm order.

Configure **only on the server**:

```json
"Razorpay": {
  "KeyId": "rzp_test_...",
  "KeySecret": "..."
}
```

The React app may hold `VITE_RAZORPAY_KEY_ID` (public test key). The secret never ships to the client. Order totals are calculated from database/menu prices, not from the cart total sent by the phone.

## API envelope

Success:

```json
{ "success": true, "message": "Operation successful", "data": {} }
```

Error:

```json
{ "success": false, "message": "Unable to process request", "errors": [] }
```

## Auth

JWT Bearer. Customer: menu, orders, reservations, profile, favourites. Admin (`/api/admin/*`): menu, categories, orders, reservations, gallery, events.

## Clone on Windows (D: drive)

After the GitHub repository exists:

```bat
D:
git clone <your-repo-url> D:\Brewora
cd D:\Brewora
git checkout dev
```

Use the `dev` branch for ongoing work.

## Configuration

| Setting | Where |
|---|---|
| `VITE_API_BASE_URL` | `mobile/.env` |
| `VITE_RAZORPAY_KEY_ID` | `mobile/.env` (public key only) |
| `ConnectionStrings:SqlServer` | backend appsettings / env |
| `Jwt:Key` | backend (keep secret) |
| `Razorpay:KeyId` / `KeySecret` | backend (keep secret) |
