#!/bin/bash

set -e

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_DIR="$SCRIPT_DIR/Voxier"

echo -e "${GREEN}Voxier — Setup${NC}"
echo ""

# ============================================
# PART 1: Install Godot
# ============================================

ARCH="$(uname -m)"
GODOT_INSTALLED=false

if command -v godot &> /dev/null; then
    echo -e "${GREEN}✓ Godot already installed!${NC}"
    godot --version 2>/dev/null || true
    GODOT_INSTALLED=true
fi

if [ "$GODOT_INSTALLED" = false ]; then
    echo -e "${YELLOW}Installing Godot 4.2...${NC}"
    
    case "$ARCH" in
        x86_64)  GODOT_NAME="Godot_v4.2.2-stable_linux.x86_64" ;;
        arm64|aarch64)  GODOT_NAME="Godot_v4.2.2-stable_linux.arm64" ;;
        *)  echo -e "${RED}Unsupported: $ARCH" && exit 1 ;;
    esac
    
    if [ ! -f "$HOME/.local/bin/godot" ]; then
        echo -e "${BLUE}Downloading Godot...${NC}"
        TMP=$(mktemp -d)
        cd "$TMP"
        
        curl -sLo godot.zip "https://github.com/godotengine/godot/releases/download/4.2.2-stable/${GODOT_NAME}.zip"
        
        if [ ! -s godot.zip ]; then
            echo -e "${RED}Download failed!${NC}"
            exit 1
        fi
        
        echo -e "${BLUE}Extracting...${NC}"
        python3 -c "import zipfile; zipfile.ZipFile('godot.zip','r').extractall('.')"
        
        mkdir -p "$HOME/.local/bin"
        mv "$GODOT_NAME" "$HOME/.local/bin/godot"
        chmod +x "$HOME/.local/bin/godot"
        
        cd ~
        rm -rf "$TMP"
    fi
    
    echo -e "${GREEN}✓ Godot installed!${NC}"
fi

echo ""

# ============================================
# PART 2: Setup Godot Project
# ============================================

echo -e "${GREEN}📦 Setting up project...${NC}"

if [ ! -f "$PROJECT_DIR/project.godot" ]; then
    echo -e "${RED}Project not found at $PROJECT_DIR${NC}"
    exit 1
fi

# Headless editor import (--import); plain --headless --path runs the game loop.
echo -e "${BLUE}Importing project (first time setup)...${NC}"
cd "$PROJECT_DIR"

export PATH="$HOME/.local/bin:$PATH"
godot --headless --path . --import

echo -e "${GREEN}✓ Project ready!${NC}"
echo ""

# ============================================
# PART 3: Ask to run
# ============================================

echo -e "${YELLOW}🚀 Do you want to run the game now?${NC}"
echo -e "${BLUE}(y/n): ${NC}"
read -r RUN

if [ "$RUN" = "y" ] || [ "$RUN" = "Y" ]; then
    echo -e "${GREEN}Starting game...${NC}"
    godot --path .
else
    echo ""
    echo -e "${GREEN}To run later:${NC}"
    echo "  ./start.sh"
    echo ""
    echo -e "${YELLOW}Or in Godot: F5 to run${NC}"
fi