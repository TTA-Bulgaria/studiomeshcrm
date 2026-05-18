#!/usr/bin/env bash
# Health check for production and staging backends.
# Cron: */5 * * * * /var/www/agency-crm/healthcheck.sh
# Expects a 401 (unauthenticated but alive) from /api/auth/me.

ALERT_EMAIL="heyoogun@gmail.com"
PROD_URL="https://app.studiomeshcrm.com/api/auth/me"
STAGING_URL="https://staging.studiomeshcrm.com/api/auth/me"

check() {
  local name=$1
  local url=$2
  local status
  status=$(curl -s -o /dev/null -w "%{http_code}" --max-time 10 "$url")
  if [ "$status" != "401" ] && [ "$status" != "200" ]; then
    echo "DOWN: $name returned HTTP $status" | \
      mail -s "[ALERT] $name is DOWN (HTTP $status)" "$ALERT_EMAIL"
    echo "$(date): $name DOWN — HTTP $status" >> /var/log/crm-healthcheck.log
  fi
}

check "Production" "$PROD_URL"
check "Staging"    "$STAGING_URL"
