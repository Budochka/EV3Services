# PowerShell script to create PostgreSQL database for EV3 Services Logger
# Usage: .\create_database.ps1 [-DbHost "storage2"] [-AdminUser "admin"] [-AdminPassword "password"]

param(
    [string]$DbHost = "storage2",
    [string]$AdminUser = "admin",
    [string]$AdminPassword = ""
)

$ErrorActionPreference = "Stop"

# Configuration from Logger config.json
$Port = 5432
$Database = "ev3"
$DatabaseUser = "ev3"
$DatabasePassword = "ev3"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "EV3 Services - PostgreSQL Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Host: $DbHost`:$Port" -ForegroundColor Gray
Write-Host "  Admin User: $AdminUser" -ForegroundColor Gray
Write-Host "  Database: $Database" -ForegroundColor Gray
Write-Host "  Database User: $DatabaseUser" -ForegroundColor Gray
Write-Host ""

# Prompt for admin password if not provided
if ([string]::IsNullOrEmpty($AdminPassword)) {
    $securePassword = Read-Host "Enter PostgreSQL '$AdminUser' user password" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $AdminPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

if ([string]::IsNullOrEmpty($AdminPassword)) {
    Write-Host "✗ Password is required" -ForegroundColor Red
    exit 1
}

$env:PGPASSWORD = $AdminPassword

# Test connection
Write-Host "Testing PostgreSQL connection..." -ForegroundColor Yellow
try {
    $version = "SELECT version();" | psql -h $DbHost -p $Port -U $AdminUser -d postgres -t -A 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Connection failed"
    }
    Write-Host "✓ Connected to PostgreSQL" -ForegroundColor Green
    Write-Host "  $($version.Trim())" -ForegroundColor Gray
} catch {
    Write-Host "✗ Failed to connect to PostgreSQL" -ForegroundColor Red
    Write-Host "  Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please verify:" -ForegroundColor Yellow
    Write-Host "  - PostgreSQL is running on $DbHost`:$Port" -ForegroundColor Gray
    Write-Host "  - $AdminUser user password is correct" -ForegroundColor Gray
    Write-Host "  - Network connectivity to $DbHost" -ForegroundColor Gray
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    exit 1
}

Write-Host ""

# Step 1: Create database
Write-Host "Step 1: Creating database '$Database'..." -ForegroundColor Yellow
$createDbSql = @"
SELECT 'Database already exists' as result
WHERE EXISTS (SELECT FROM pg_database WHERE datname = '$Database')
UNION ALL
SELECT 'Database created successfully' as result
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$Database')
AND (SELECT 1 FROM (SELECT 1) AS x WHERE false);  -- Never true, just for syntax

CREATE DATABASE $Database
    WITH 
    OWNER = $AdminUser
    ENCODING = 'UTF8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;
"@

try {
    $result = $createDbSql | psql -h $DbHost -p $Port -U $AdminUser -d postgres -t -A 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database '$Database' created/verified" -ForegroundColor Green
    } else {
        # Check if it's because database already exists
        $checkExists = "SELECT 1 FROM pg_database WHERE datname = '$Database';" | psql -h $DbHost -p $Port -U $AdminUser -d postgres -t -A 2>&1
        if ($checkExists -match "1") {
            Write-Host "✓ Database '$Database' already exists" -ForegroundColor Green
        } else {
            Write-Host "⚠ Database creation returned exit code: $LASTEXITCODE" -ForegroundColor Yellow
            Write-Host "  Output: $result" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "⚠ Error: $_" -ForegroundColor Yellow
}

Write-Host ""

# Step 2: Create user
Write-Host "Step 2: Creating user '$DatabaseUser'..." -ForegroundColor Yellow
$createUserSql = @"
DO `$`$`$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = '$DatabaseUser') THEN
        CREATE USER $DatabaseUser WITH PASSWORD '$DatabasePassword';
        RAISE NOTICE 'User $DatabaseUser created';
    ELSE
        RAISE NOTICE 'User $DatabaseUser already exists';
        ALTER USER $DatabaseUser WITH PASSWORD '$DatabasePassword';
    END IF;
END
`$`$`$;
"@

try {
    $result = $createUserSql | psql -h $DbHost -p $Port -U $AdminUser -d postgres 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ User '$DatabaseUser' created/updated" -ForegroundColor Green
    } else {
        Write-Host "⚠ User creation returned exit code: $LASTEXITCODE" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠ Error: $_" -ForegroundColor Yellow
}

Write-Host ""

# Step 3: Grant database privileges
Write-Host "Step 3: Granting database privileges..." -ForegroundColor Yellow
$grantDbSql = "GRANT ALL PRIVILEGES ON DATABASE $Database TO $DatabaseUser;"
try {
    $grantDbSql | psql -h $DbHost -p $Port -U $AdminUser -d postgres 2>&1 | Out-Null
    Write-Host "✓ Database privileges granted" -ForegroundColor Green
} catch {
    Write-Host "⚠ Error: $_" -ForegroundColor Yellow
}

Write-Host ""

# Step 4: Create schema
Write-Host "Step 4: Creating schema..." -ForegroundColor Yellow
$schemaFile = Join-Path $PSScriptRoot "ev3_postgresql.sql"
if (-not (Test-Path $schemaFile)) {
    Write-Host "✗ Schema file not found: $schemaFile" -ForegroundColor Red
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    exit 1
}

# Read schema file and skip CREATE DATABASE and \c commands (handled by script)
try {
    $schemaContent = Get-Content $schemaFile | Where-Object { 
        $_ -notmatch '^CREATE DATABASE' -and 
        $_ -notmatch '^\\c ' -and
        $_ -notmatch '^--.*CREATE DATABASE'
    }
    $output = $schemaContent | psql -h $DbHost -p $Port -U $AdminUser -d $Database 2>&1
    $hasErrors = $output | Where-Object { $_ -match "ERROR" }
    
    if ($hasErrors) {
        Write-Host "⚠ Some errors occurred (objects may already exist):" -ForegroundColor Yellow
        $hasErrors | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    }
    
    if ($LASTEXITCODE -eq 0 -or $output -match "already exists") {
        Write-Host "✓ Schema created successfully" -ForegroundColor Green
    } else {
        Write-Host "⚠ Schema creation returned exit code: $LASTEXITCODE" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Error creating schema: $_" -ForegroundColor Red
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
    exit 1
}

Write-Host ""

# Step 5: Grant table privileges
Write-Host "Step 5: Granting table privileges..." -ForegroundColor Yellow
$grantTableSql = @"
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $DatabaseUser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO $DatabaseUser;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO $DatabaseUser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO $DatabaseUser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO $DatabaseUser;
"@

try {
    $grantTableSql | psql -h $DbHost -p $Port -U $AdminUser -d $Database 2>&1 | Out-Null
    Write-Host "✓ Table privileges granted" -ForegroundColor Green
} catch {
    Write-Host "⚠ Error: $_" -ForegroundColor Yellow
}

Write-Host ""

# Verify
Write-Host "Verifying setup..." -ForegroundColor Yellow
$verifySql = @"
SELECT 
    'Tables' as type, COUNT(*) as count FROM information_schema.tables WHERE table_schema = 'public'
UNION ALL
SELECT 
    'Indexes', COUNT(*) FROM pg_indexes WHERE schemaname = 'public'
UNION ALL
SELECT 
    'Functions', COUNT(*) FROM pg_proc p JOIN pg_namespace n ON p.pronamespace = n.oid WHERE n.nspname = 'public';
"@

try {
    $verify = $verifySql | psql -h $DbHost -p $Port -U $AdminUser -d $Database -t -A 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database objects:" -ForegroundColor Green
        $verify | ForEach-Object {
            if ($_ -match "\|") {
                $parts = $_ -split "\|"
                Write-Host "  $($parts[0].Trim()): $($parts[1].Trim())" -ForegroundColor Gray
            }
        }
    }
} catch {
    Write-Host "⚠ Could not verify: $_" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✓ Database setup complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Connection string (for Logger/config.json):" -ForegroundColor Yellow
Write-Host "Host=$DbHost;Port=$Port;Username=$DatabaseUser;Password=$DatabasePassword;Database=$Database" -ForegroundColor White
Write-Host ""

# Clean up
Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue

Write-Host "You can now start the Logger service!" -ForegroundColor Green

