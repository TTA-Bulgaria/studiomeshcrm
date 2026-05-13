#!/usr/bin/env bash
# Deploy to staging.studiomeshcrm.com
# Run this BEFORE deploying to production.
set -e

STAGING=/var/www/agency-crm-staging
API_OUT=$STAGING/Crm.Api/out
WEB=$STAGING/web

echo "==> [Staging] Pulling latest code..."
cd $STAGING
git pull

echo "==> [Staging] Building backend..."
cd $STAGING/backend
dotnet publish Crm.Api/Crm.Api.csproj -c Release -o "$API_OUT"

echo "==> [Staging] Restarting backend..."
sudo systemctl restart crmapi-staging.service

echo "==> [Staging] Building frontend..."
cd $WEB
npm ci --omit=dev
npm run build

echo "==> [Staging] Copying static assets..."
cp -r "$WEB/.next/static"  "$WEB/.next/standalone/web/.next/"
cp -r "$WEB/public"        "$WEB/.next/standalone/web/"

echo "==> [Staging] Restarting frontend..."
pm2 restart agencycrm-staging

echo ""
echo "✓ Staging deploy complete — https://staging.studiomeshcrm.com"
echo "  Test thoroughly before running deploy-production.sh"
