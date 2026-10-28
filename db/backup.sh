#!/usr/bin/env bash
# Compressed dump of the PostgreSQL container, keeping the last KEEP_DAYS days.
# Run it from anywhere on the server; cron example and restore steps are in the README.
set -euo pipefail

cd "$(dirname "$0")/.."

BACKUP_DIR="${BACKUP_DIR:-$HOME/backups/infectionskillhub}"
KEEP_DAYS="${KEEP_DAYS:-14}"

# Database name and user come from the same .env that docker compose reads.
set -a
. ./.env
set +a

mkdir -p "$BACKUP_DIR"
file="$BACKUP_DIR/infection_skillhub-$(date +%Y%m%d-%H%M%S).sql.gz"

# A half-written dump never stays behind with a final name.
trap 'rm -f "$file.partial"' EXIT

# stdin comes from /dev/null: run from an interactive terminal, sudo and "docker compose exec" would
# otherwise wait on the terminal and pg_dump would never start.
sudo -n docker compose exec -T db pg_dump -U "$POSTGRES_USER" -d "$POSTGRES_DB" --clean --if-exists --no-owner \
  < /dev/null | gzip > "$file.partial"
mv "$file.partial" "$file"
trap - EXIT

find "$BACKUP_DIR" -name 'infection_skillhub-*.sql.gz' -mtime +"$KEEP_DAYS" -delete

echo "$(date '+%F %T') backup written to $file ($(du -h "$file" | cut -f1))"
