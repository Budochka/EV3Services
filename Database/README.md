# PostgreSQL Database Setup for EV3 Services Logger

## Quick Start

Run the setup script:

```powershell
cd Database
.\create_database.ps1
```

The script will prompt you for the PostgreSQL admin user password (default: `admin`).

Or provide parameters directly:

```powershell
# With default settings (storage2, admin user)
.\create_database.ps1 -AdminPassword "your_admin_password"

# With custom host and admin user
.\create_database.ps1 -DbHost "storage2" -AdminUser "admin" -AdminPassword "your_admin_password"

# All parameters
.\create_database.ps1 -DbHost "localhost" -AdminUser "postgres" -AdminPassword "password"
```

## What the Script Does

1. ✅ **Creates database** `ev3`
2. ✅ **Creates user** `ev3` with password `ev3`
3. ✅ **Creates schema**:
   - `Events` table with optimized indexes
   - Automatic `DataSize` calculation trigger
   - Full-text search index on `Topic`
   - `EventsText` view for text-based events
4. ✅ **Grants privileges** to `ev3` user

## Manual Setup (Alternative)

If you prefer to run SQL manually:

```bash
# Connect to PostgreSQL and create database first
psql -h storage2 -p 5432 -U postgres -d postgres
CREATE DATABASE ev3;

# Then connect to ev3 and run schema
\c ev3
\i ev3_postgresql.sql
```

## Verification

After setup, verify the database:

```sql
-- Connect to ev3 database
psql -h storage2 -p 5432 -U ev3 -d ev3

-- Check tables
\dt

-- Check indexes
\di

-- Check functions
\df

-- Test insert
INSERT INTO Events (Topic, Data) VALUES ('test.topic', E'\\x48656c6c6f');
SELECT * FROM Events ORDER BY Time DESC LIMIT 5;
```

## Connection String

After setup, use this connection string in `Logger/config.json`:

```
Host=storage2;Port=5432;Username=ev3;Password=ev3;Database=ev3
```

## Troubleshooting

### Connection Failed

- Verify PostgreSQL is running: `systemctl status postgresql` (on storage2)
- Check network connectivity: `ping storage2`
- Verify port: `telnet storage2 5432`
- Check firewall rules
- Verify admin user name and password

### Permission Denied

- Ensure admin user (default: `admin`) has CREATE DATABASE privilege
- Check `pg_hba.conf` for authentication settings
- Verify admin user exists: `psql -U admin -d postgres -c "\du"`

### Database Already Exists

The script handles existing databases gracefully. If you need to recreate:

```sql
-- Connect as postgres user
psql -h storage2 -p 5432 -U postgres -d postgres

-- Drop and recreate
DROP DATABASE IF EXISTS ev3;
-- Then run create_database.ps1 again
```

## Files

- `create_database.ps1` - Automated setup script (recommended)
- `ev3_postgresql.sql` - Schema SQL file (tables, indexes, functions, triggers, views)

