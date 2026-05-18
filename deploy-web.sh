#!/usr/bin/env bash
set -e

WEB=/var/www/agency-crm/web

cd "$WEB"
git pull
npm ci --omit=dev
npm run build

# Standalone mode requires manual copy of static assets
cp -r "$WEB/.next/static"  "$WEB/.next/standalone/web/.next/"
cp -r "$WEB/public"        "$WEB/.next/standalone/web/"

pm2 restart agencycrm-web
echo "Deploy complete."
