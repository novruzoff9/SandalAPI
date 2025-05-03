#!/bin/bash

set -e  # Əgər hər hansı əmrdə xəta olarsa, skript dayansın

# ANSI rəng kodları
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[1;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Sənin Docker Hub istifadəçi adın
DOCKER_USERNAME="novruzoff999"

# Mövcud servis adları
ALL_SERVICES=("identityserver" "organizationwebapi" "orderwebapi" "webapigateway")

SELECTED_SERVICES=()

# İstifadəçidən seçim al
echo -e "${YELLOW}🤔 Bütün servisləri push etmək istəyirsiniz? (y/n): ${NC}"
read -r PUSH_ALL

if [[ "$PUSH_ALL" =~ ^[Yy]$ ]]; then
    SELECTED_SERVICES=("${ALL_SERVICES[@]}")
else
    echo -e "${YELLOW}🔘 Zəhmət olmasa push etmək istədiyiniz servisləri seçin (y/n):${NC}"
    for SERVICE in "${ALL_SERVICES[@]}"
    do
        echo -ne "${YELLOW}➡️  $SERVICE? (y/n): ${NC}"
        read -r REPLY
        if [[ "$REPLY" =~ ^[Yy]$ ]]; then
            SELECTED_SERVICES+=("$SERVICE")
        fi
    done

    if [ ${#SELECTED_SERVICES[@]} -eq 0 ]; then
        echo -e "${RED}❌ Heç bir servis seçilmədi. Skript dayandırılır.${NC}"
        exit 1
    fi
fi

# 1. Əsas qovluğa keç
cd "$(dirname "$0")/.."

# 2. Bütün imajları build et (override daxil olmaqla)
echo -e "${BLUE}🔧 Docker image-lər build edilir...${NC}"
docker-compose -f docker-compose.yml -f docker-compose.override.yml build

# 3. Hər bir seçilmiş servisi tag və push et
for SERVICE in "${SELECTED_SERVICES[@]}"
do
    echo -e "${BLUE}📦 Tagging and pushing ${SERVICE} ...${NC}"
    docker tag "${SERVICE}" "${DOCKER_USERNAME}/${SERVICE}:latest"
    docker push "${DOCKER_USERNAME}/${SERVICE}:latest"
done

echo -e "${GREEN}✅ Seçilmiş servis image-ləri push edildi!${NC}"
sleep 10