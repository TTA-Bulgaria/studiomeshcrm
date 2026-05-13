#!/usr/bin/env bash
# Deploy to app.studiomeshcrm.com (PRODUCTION)
# Only run this after verifying staging.studiomeshcrm.com works correctly.
set -e

PROD=/var/www/agency-crm
API_OUT=$PROD/publish/api
WEB=$PROD/web

echo "==> [Production] Pulling latest code..."
cd $PROD
git pull

echo "==> [Production] Building backend..."
cd $PROD/backend
dotnet publish -c Release -o "$API_OUT"

echo "==> [Production] Restarting backend..."
sudo systemctl restart crmapi.service

echo "==> [Production] Building frontend..."
cd $WEB
npm ci --omit=dev
npm run build

echo "==> [Production] Copying static assets..."
cp -r "$WEB/.next/static"  "$WEB/.next/standalone/web/.next/"
cp -r "$WEB/public"        "$WEB/.next/standalone/web/"

echo "==> [Production] Restarting frontend..."
pm2 restart agencycrm-web

echo ""
echo "✓ Production deploy complete — https://app.studiomeshcrm.com"
