#!/usr/bin/env python3
"""
FOX ROCKET ARCADE - Industry Standard Arcade Game
A fully-featured classic arcade shooter with pseudo-3D visuals
"""

import pygame
import random
import math
import sys
from dataclasses import dataclass, field
from typing import List, Optional
from enum import Enum, auto

pygame.init()

SCREEN_WIDTH = 800
SCREEN_HEIGHT = 600
FPS = 60

WHITE = (255, 255, 255)
BLACK = (10, 10, 18)
ORANGE = (255, 102, 0)
GREEN = (68, 170, 68)
DARK_GREEN = (51, 136, 51)
RED = (255, 68, 68)
CYAN = (0, 255, 255)
YELLOW = (255, 255, 0)
PURPLE = (180, 100, 220)

BG_COLORS = {
    'space': ((26, 10, 46), (46, 28, 78)),
    'city': ((10, 23, 41), (26, 41, 72)),
    'desert': ((46, 26, 10), (78, 43, 28)),
    'forest': ((10, 46, 26), (28, 78, 46))
}

DIRECTION_NAMES = ['N', 'NE', 'E', 'SE', 'S', 'SW', 'W', 'NW']


class GameState(Enum):
    MENU = auto()
    PLAYING = auto()
    GAME_OVER = auto()
    ROCKET_CHANGE = auto()


@dataclass
class Particle:
    x: float
    y: float
    vx: float
    vy: float
    color: tuple
    life: float
    size: float


@dataclass
class Star:
    x: float
    y: float
    speed: float
    size: float
    brightness: float


class Bullet:
    def __init__(self, x: int, y: int, vy: int, is_player: bool = True):
        self.x = x
        self.y = y
        self.vy = vy
        self.is_player = is_player
        self.rect = pygame.Rect(x - 4, y - 4, 8, 8)
        self.active = True

    def update(self, dt: float):
        self.y += self.vy * dt
        self.rect.y = int(self.y)
        if self.y < -20 or self.y > SCREEN_HEIGHT + 20:
            self.active = False

    def draw(self, surf: pygame.Surface):
        color = YELLOW if self.is_player else RED
        pygame.draw.circle(surf, color, (int(self.x), int(self.y)), 4)
        if self.is_player:
            pygame.draw.circle(surf, (200, 255, 200), (int(self.x), int(self.y)), 2)


class Enemy:
    ENEMY_TYPES = {
        'drone': {'health': 1, 'score': 100, 'speed': 150, 'fire_rate': 0.5, 'size': 15},
        'fighter': {'health': 2, 'score': 250, 'speed': 200, 'fire_rate': 0.8, 'size': 20},
        'mother': {'health': 10, 'score': 1000, 'speed': 50, 'fire_rate': 1.5, 'size': 35}
    }

    def __init__(self, enemy_type: str, x: int, y: int):
        self.type = enemy_type
        stats = self.ENEMY_TYPES[enemy_type]
        self.health = stats['health']
        self.score_value = stats['score']
        self.speed = stats['speed']
        self.fire_rate = stats['fire_rate']
        self.size = stats['size']
        
        self.x = x
        self.y = y
        self.rect = pygame.Rect(x - self.size, y - self.size, self.size * 2, self.size * 2)
        self.active = True
        
        self.zigzag_offset = random.random() * math.pi * 2
        self.last_fire = 0
        self.do_zigzag = enemy_type == 'drone'
        self.do_chase = enemy_type == 'fighter'

    def update(self, dt: float, player_x: int, player_y: int, current_time: float, bullets: List):
        self.y -= self.speed * dt
        
        if self.do_zigzag:
            self.x += math.sin(current_time * 3 + self.zigzag_offset) * 50 * dt
        
        if self.do_chase:
            dx = player_x - self.x
            if abs(dx) > 10:
                self.x += math.copysign(self.speed * 0.3, dx) * dt
        
        self.rect.x = int(self.x - self.size)
        self.rect.y = int(self.y - self.size)
        
        if current_time - self.last_fire >= self.fire_rate and player_y > 0:
            angle = math.atan2(player_y - self.y, player_x - self.x)
            vx = math.cos(angle) * 250
            vy = math.sin(angle) * 250
            bullets.append(Bullet(int(self.x), int(self.y), int(vy), False))
            self.last_fire = current_time
        
        if self.y > SCREEN_HEIGHT + 50:
            self.active = False

    def draw(self, surf: pygame.Surface):
        if self.type == 'drone':
            points = [(self.x, self.y - self.size), (self.x + self.size, self.y + self.size), (self.x - self.size, self.y + self.size)]
            pygame.draw.polygon(surf, RED, points)
            pygame.draw.circle(surf, WHITE, (int(self.x), int(self.y + self.size // 2)), 4)
        elif self.type == 'fighter':
            points = [(self.x, self.y - self.size), (self.x + self.size, self.y + self.size), (self.x, self.y + self.size // 2), (self.x - self.size, self.y + self.size)]
            pygame.draw.polygon(surf, (255, 136, 68), points)
            pygame.draw.circle(surf, YELLOW, (int(self.x), int(self.y)), 3)
        elif self.type == 'mother':
            pygame.draw.circle(surf, (68, 0, 0), (int(self.x), int(self.y)), self.size)
            pygame.draw.circle(surf, RED, (int(self.x), int(self.y)), self.size - 8)
            for i in range(4):
                angle = i * math.pi / 2
                px = self.x + math.cos(angle) * 12
                py = self.y + math.sin(angle) * 12
                pygame.draw.circle(surf, WHITE, (int(px), int(py)), 5)

    def take_damage(self, dmg: int) -> bool:
        self.health -= dmg
        if self.health <= 0:
            self.active = False
            return True
        return False


class Fox:
    def __init__(self):
        self.x = SCREEN_WIDTH // 2
        self.y = SCREEN_HEIGHT - 150
        self.velocity = [0, 0]
        self.speed = 300
        self.size = 40
        self.alive = True
        self.last_fire = 0
        self.fire_cooldown = 0.15
        self.rect = pygame.Rect(self.x - 20, self.y - 20, 40, 40)
        self.facing_right = True
        self.rocket: Optional['Rocket'] = None

    def update(self, dt: float, keys: dict, current_time: float, bullets: List, state: GameState):
        if not self.alive or state != GameState.PLAYING:
            self.velocity = [0, 0]
            return

        self.velocity = [0, 0]
        
        if keys[pygame.K_LEFT] or keys[pygame.K_a]:
            self.velocity[0] = -self.speed
        elif keys[pygame.K_RIGHT] or keys[pygame.K_d]:
            self.velocity[0] = self.speed
        
        if keys[pygame.K_UP] or keys[pygame.K_w]:
            self.velocity[1] = self.speed
        elif keys[pygame.K_DOWN] or keys[pygame.K_s]:
            self.velocity[1] = -self.speed

        speed = math.sqrt(self.velocity[0]**2 + self.velocity[1]**2)
        if speed > 0:
            factor = self.speed / speed
            self.velocity[0] *= factor
            self.velocity[1] *= factor

        self.x += self.velocity[0] * dt
        self.y += self.velocity[1] * dt

        self.x = max(20, min(SCREEN_WIDTH - 20, self.x))
        self.y = max(150 + 20, min(SCREEN_HEIGHT - 20, self.y))
        
        self.rect.x = int(self.x - 20)
        self.rect.y = int(self.y - 20)

        if self.velocity[0] < 0:
            self.facing_right = False
        elif self.velocity[0] > 0:
            self.facing_right = True

        if (keys[pygame.K_SPACE] or keys[pygame.K_RCTRL]) and current_time - self.last_fire >= self.fire_cooldown:
            bullets.append(Bullet(int(self.x), int(self.y) - 25, -500, True))
            self.last_fire = current_time

    def draw(self, surf: pygame.Surface):
        if not self.alive:
            return
        
        x, y = int(self.x), int(self.y)
        
        if self.rocket:
            self.rocket.x = self.x
            self.rocket.y = self.y - 70
            self.rocket.draw(surf)

        scale = -1 if not self.facing_right else 1
        
        body = [(x - 18, y + 5), (x + 18, y + 5), (x + 18, y + 15), (x - 18, y + 15)]
        if not self.facing_right:
            body = [(SCREEN_WIDTH - px, py) for px, py in body]
        pygame.draw.polygon(surf, ORANGE, body)
        
        pygame.draw.ellipse(surf, WHITE, (x - 12, y + 10, 24, 10))
        
        ears = [(x - 10, y - 22), (x - 5, y - 28), (x, y - 22), (x, y - 22), (x + 5, y - 28), (x + 10, y - 22)]
        if not self.facing_right:
            ears = [(SCREEN_WIDTH - px, py) for px, py in ears]
        pygame.draw.polygon(surf, WHITE, ears)
        
        head = [(x - 12, y - 15), (x + 12, y - 15), (x + 12, y + 5), (x - 12, y + 5)]
        if not self.facing_right:
            head = [(SCREEN_WIDTH - px, py) for px, py in head]
        pygame.draw.polygon(surf, ORANGE, head)
        
        pygame.draw.circle(surf, BLACK, (x - 4, y - 8), 4)
        pygame.draw.circle(surf, BLACK, (x + 4, y - 8), 4)
        
        nose = [(x - 2, y + 2), (x + 2, y + 2), (x, y + 6)]
        if not self.facing_right:
            nose = [(SCREEN_WIDTH - px, py) for px, py in nose]
        pygame.draw.polygon(surf, (68, 68, 255), nose)
        
        shirt_rect = pygame.Rect(x - 10, y - 5, 20, 15)
        pygame.draw.rect(surf, GREEN, shirt_rect)
        pygame.draw.rect(surf, DARK_GREEN, (x - 8, y - 3, 6, 10))
        pygame.draw.rect(surf, DARK_GREEN, (x + 2, y - 3, 6, 10))
        
        bp_x = x - 16 if self.facing_right else x + 4
        backpack = pygame.Rect(bp_x, y - 10, 8, 12)
        pygame.draw.rect(surf, (136, 68, 34), backpack)
        pygame.draw.rect(surf, (102, 51, 34), backpack, 2)

    def die(self):
        self.alive = False

    def revive(self):
        self.alive = True
        self.x = SCREEN_WIDTH // 2
        self.y = SCREEN_HEIGHT - 150


class Rocket:
    def __init__(self, x: int, y: int):
        self.x = x
        self.y = y
        self.timer = 25.0
        self.max_timer = 25.0
        self.exploding = False
        self.active = True
        self.start_y = y

    def update(self, dt: float, current_time: float):
        if not self.active or self.exploding:
            return

        self.timer -= dt
        
        hover = math.sin(current_time * 3) * 3
        self.y = self.start_y + hover

        if self.timer <= 0:
            self.explode()

    def draw(self, surf: pygame.Surface):
        x, y = int(self.x), int(self.y)
        
        if self.exploding:
            size = int((2 - self.timer) * 50)
            if size > 0:
                pygame.draw.circle(surf, WHITE, (x, y), size // 2)
                pygame.draw.circle(surf, (255, 170, 0), (x, y), size)
                pygame.draw.circle(surf, (255, 68, 0), (x, y), size + 10, 3)
            return

        body = [(x - 15, y - 25), (x + 15, y - 25), (x + 12, y + 25), (x - 12, y + 25)]
        pygame.draw.polygon(surf, ORANGE, body)
        
        pygame.draw.rect(surf, (200, 100, 50), (x - 12, y - 22, 24, 10))
        pygame.draw.ellipse(surf, WHITE, (x - 5, y - 10, 10, 10))
        
        pygame.draw.rect(surf, DARK_GREEN, (x - 20, y + 18, 8, 12))
        pygame.draw.rect(surf, DARK_GREEN, (x + 12, y + 18, 8, 12))
        
        if self.timer > 0:
            flame_size = 10 + math.sin(pygame.time.get_ticks() / 50) * 5
            flame = [(x - 8, y + 25), (x, y + 25 + flame_size), (x + 8, y + 25)]
            pygame.draw.polygon(surf, YELLOW, flame)
            flame_inner = [(x - 4, y + 25), (x, y + 25 + flame_size * 0.6), (x + 4, y + 25)]
            pygame.draw.polygon(surf, WHITE, flame_inner)

    def explode(self):
        self.exploding = True

    def activate(self):
        self.exploding = False
        self.timer = self.max_timer
        self.active = True


class Powerup:
    def __init__(self, x: int, y: int):
        self.x = x
        self.y = y
        self.type = random.choice(['rapid', 'shield', 'multi'])
        self.active = True
        self.rect = pygame.Rect(x - 12, y - 12, 24, 24)

    def update(self, dt: float):
        self.y += 50 * dt
        self.rect.y = int(self.y)
        if self.y > SCREEN_HEIGHT + 30:
            self.active = False

    def draw(self, surf: pygame.Surface):
        color = {'rapid': RED, 'shield': CYAN, 'multi': YELLOW}[self.type]
        
        glow = 0.6 + math.sin(pygame.time.get_ticks() / 100) * 0.3
        color_alpha = (*color, int(glow * 255))
        
        pygame.draw.circle(surf, color, (int(self.x), int(self.y)), 12)
        
        symbol = {'rapid': '⚡', 'shield': '🛡', 'multi': '×2'}[self.type]
        font = pygame.font.Font(None, 20)
        text = font.render(symbol, True, WHITE)
        surf.blit(text, (self.x - text.get_width() // 2, self.y - text.get_height() // 2))


class Game:
    def __init__(self):
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("FOX ROCKET ARCADE")
        self.clock = pygame.time.Clock()
        
        self.state = GameState.MENU
        self.score = 0
        
        self.fox = Fox()
        self.rocket = Rocket(SCREEN_WIDTH // 2, SCREEN_HEIGHT - 220)
        self.fox.rocket = self.rocket
        self.new_rocket = None
        self.rocket_timer = 0
        
        self.bullets: List[Bullet] = []
        self.enemies: List[Enemy] = []
        self.powerups: List[Powerup] = []
        self.particles: List[Particle] = []
        
        self.stars = [Star(random.random() * SCREEN_WIDTH, random.random() * SCREEN_HEIGHT, 
                        random.uniform(1, 3), random.uniform(1, 3), random.random()) 
                    for _ in range(100)]
        
        self.direction = 0
        self.direction_angle = 0
        self.target_angle = 0
        self.screen_rotation = 0
        
        self.bg_index = 0
        self.enemy_spawn_timer = 0
        self.difficulty = 1
        
        self.font = pygame.font.Font(None, 32)
        self.big_font = pygame.font.Font(None, 64)
        
        self.create_particles_explosion(self.rocket.x, self.rocket.y, 20)

    def create_particles_explosion(self, x: int, y: int, count: int):
        for _ in range(count):
            angle = random.random() * math.pi * 2
            speed = random.uniform(50, 200)
            self.particles.append(Particle(
                x, y,
                math.cos(angle) * speed,
                math.sin(angle) * speed,
                random.choice([ORANGE, YELLOW, RED, WHITE]),
                random.uniform(0.5, 1.5),
                random.uniform(2, 6)
            ))

    def reset(self):
        self.state = GameState.PLAYING
        self.score = 0
        
        self.fox = Fox()
        self.rocket = Rocket(SCREEN_WIDTH // 2, SCREEN_HEIGHT - 220)
        self.fox.rocket = self.rocket
        self.new_rocket = None
        self.rocket_timer = 0
        
        self.bullets = []
        self.enemies = []
        self.powerups = []
        self.particles = []
        
        self.direction = 0
        self.direction_angle = 0
        self.target_angle = 0
        self.screen_rotation = 0
        
        self.bg_index = 0
        self.enemy_spawn_timer = 0
        self.difficulty = 1

    def run(self):
        running = True
        
        while running:
            current_time = pygame.time.get_ticks() / 1000.0
            dt = self.clock.tick(FPS) / 1000.0
            
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                elif event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_ESCAPE:
                        running = False
                    elif event.key == pygame.K_RETURN:
                        if self.state == GameState.MENU:
                            self.reset()
                        elif self.state == GameState.GAME_OVER:
                            self.reset()
                        elif self.state == GameState.ROCKET_CHANGE and self.new_rocket:
                            self.hop_to_rocket()
                    elif event.key == pygame.K_q:
                        self.turn(-1)
                    elif event.key == pygame.K_e:
                        self.turn(1)

            keys = pygame.key.get_pressed()
            
            if self.state == GameState.PLAYING:
                self.update(dt, current_time, keys)
            elif self.state == GameState.ROCKET_CHANGE:
                self.rocket_timer -= dt
                if self.rocket_timer <= 0:
                    self.game_over()
            
            self.render()
            pygame.display.flip()

        pygame.quit()
        sys.exit()

    def update(self, dt: float, current_time: float, keys: dict):
        self.fox.update(dt, keys, current_time, self.bullets, self.state)
        self.rocket.update(dt, current_time)
        
        if self.rocket.exploding:
            self.rocket.timer -= dt
            if self.rocket.timer < -2:
                if self.new_rocket:
                    self.rocket = self.new_rocket
                    self.fox.rocket = self.rocket
                    self.rocket.activate()
                    self.new_rocket = None
                    self.state = GameState.PLAYING
                    self.bg_index = (self.bg_index + 1) % 4
                else:
                    self.game_over()

        for b in self.bullets:
            b.update(dt)
        self.bullets = [b for b in self.bullets if b.active]

        for e in self.enemies:
            e.update(dt, self.fox.x, self.fox.y, current_time, self.bullets)
        self.enemies = [e for e in self.enemies if e.active]

        for p in self.powerups:
            p.update(dt)
        self.powerups = [p for p in self.powerups if p.active]

        for p in self.particles:
            p.x += p.vx * dt
            p.y += p.vy * dt
            p.life -= dt
            p.vx *= 0.98
            p.vy *= 0.98
        self.particles = [p for p in self.particles if p.life > 0]

        for b in self.bullets:
            if b.is_player:
                for e in self.enemies:
                    if e.rect.colliderect(b.rect):
                        b.active = False
                        if e.take_damage(1):
                            self.score += e.score_value
                            self.create_particles_explosion(int(e.x), int(e.y), 15)
                            if e.type == 'mother' and random.random() < 0.3:
                                self.powerups.append(Powerup(int(e.x), int(e.y)))
                        break
            else:
                if self.fox.alive and self.fox.rect.colliderect(b.rect):
                    b.active = False
                    self.fox_die()

        for e in self.enemies:
            if self.fox.alive and self.fox.rect.colliderect(e.rect):
                self.fox_die()

        for p in self.powerups:
            if self.fox.alive and self.fox.rect.colliderect(p.rect):
                p.active = False
                self.score += 50

        self.enemy_spawn_timer -= dt
        if self.enemy_spawn_timer <= 0 and len(self.enemies) < 15:
            self.spawn_enemy()
            self.enemy_spawn_timer = max(0.5, 2 - self.difficulty * 0.02)
            self.difficulty += 0.1

        for star in self.stars:
            star.y += star.speed * (1 + self.direction_angle / 45) * dt * 60
            if star.y > SCREEN_HEIGHT:
                star.y = 0
                star.x = random.random() * SCREEN_WIDTH

        if abs(self.target_angle - self.screen_rotation) > 0.5:
            self.screen_rotation += (self.target_angle - self.screen_rotation) * 3 * dt

    def spawn_enemy(self):
        weights = [0.7, 0.25, 0.05] if self.difficulty < 3 else [0.5, 0.35, 0.15] if self.difficulty < 6 else [0.4, 0.35, 0.25]
        etype = random.choices(['drone', 'fighter', 'mother'], weights=weights)[0]
        x = random.randint(50, SCREEN_WIDTH - 50)
        self.enemies.append(Enemy(etype, x, -30))

    def turn(self, direction: int):
        self.direction = (self.direction + direction) % 8
        self.target_angle = self.direction * 45
        
        if self.direction % 2 == 0:
            self.score += 200

    def fox_die(self):
        self.fox.die()
        self.create_particles_explosion(int(self.fox.x), int(self.fox.y), 30)
        self.game_over()

    def game_over(self):
        self.state = GameState.GAME_OVER

    def hop_to_rocket(self):
        if not self.new_rocket:
            return
        
        self.rocket = self.new_rocket
        self.rocket.activate()
        self.fox.rocket = self.rocket
        self.fox.x = self.rocket.x
        self.fox.y = self.rocket.y + 70
        self.new_rocket = None
        self.state = GameState.PLAYING
        self.bg_index = (self.bg_index + 1) % 4

    def render(self):
        bg_c1, bg_c2 = list(BG_COLORS.values())[self.bg_index]
        
        surf = self.screen
        surf.fill(bg_c1)
        
        pygame.draw.rect(surf, bg_c2, (10, 10, SCREEN_WIDTH - 20, SCREEN_HEIGHT - 20))

        for star in self.stars:
            brightness = int(star.brightness * 255)
            pygame.draw.circle(surf, (brightness, brightness, brightness), 
                          (int(star.x), int(star.y)), int(star.size))

        for p in self.particles:
            alpha = min(255, int(p.life * 255))
            color = (*p.color[:3], alpha)
            pygame.draw.circle(surf, p.color, (int(p.x), int(p.y)), int(p.size))

        for p in self.powerups:
            p.draw(surf)

        for e in self.enemies:
            e.draw(surf)

        for b in self.bullets:
            b.draw(surf)

        self.fox.draw(surf)

        if self.state == GameState.PLAYING or self.state == GameState.ROCKET_CHANGE:
            score_text = self.font.render(f"SCORE: {self.score}", True, YELLOW)
            surf.blit(score_text, (20, 20))
            
            timer_text = self.font.render(f"ROCKET: {max(0, math.ceil(self.rocket.timer))}s", True, RED if self.rocket.timer < 5 else WHITE)
            surf.blit(timer_text, (SCREEN_WIDTH // 2 - 50, 20))
            
            dir_text = self.font.render(f"DIR: {DIRECTION_NAMES[self.direction]}", True, CYAN)
            surf.blit(dir_text, (SCREEN_WIDTH - 100, 20))

            if self.state == GameState.ROCKET_CHANGE:
                warning = self.big_font.render("NEW ROCKET! PRESS ENTER!", True, RED)
                warning_rect = warning.get_rect(center=(SCREEN_WIDTH // 2, SCREEN_HEIGHT // 2))
                pygame.draw.rect(surf, BLACK, warning_rect.inflate(20, 20))
                surf.blit(warning, warning_rect)

        if self.state == GameState.MENU:
            title = self.big_font.render("FOX ROCKET ARCADE", True, ORANGE)
            title_rect = title.get_rect(center=(SCREEN_WIDTH // 2, 150))
            surf.blit(title, title_rect)
            
            start_text = self.font.render("PRESS ENTER TO START", True, WHITE)
            start_rect = start_text.get_rect(center=(SCREEN_WIDTH // 2, 400))
            surf.blit(start_text, start_rect)
            
            controls = self.font.render("ARROWS: MOVE | SPACE: FIRE | Q/E: TURN | ENTER: HOP", True, (150, 150, 150))
            controls_rect = controls.get_rect(center=(SCREEN_WIDTH // 2, 500))
            surf.blit(controls, controls_rect)

        elif self.state == GameState.GAME_OVER:
            game_over = self.big_font.render("GAME OVER", True, RED)
            game_over_rect = game_over.get_rect(center=(SCREEN_WIDTH // 2, 200))
            surf.blit(game_over, game_over_rect)
            
            final_score = self.big_font.render(f"SCORE: {self.score}", True, YELLOW)
            final_rect = final_score.get_rect(center=(SCREEN_WIDTH // 2, 300))
            surf.blit(final_score, final_rect)
            
            retry = self.font.render("PRESS ENTER TO RETRY", True, WHITE)
            retry_rect = retry.get_rect(center=(SCREEN_WIDTH // 2, 420))
            surf.blit(retry, retry_rect)


if __name__ == "__main__":
    pygame.font.init()
    game = Game()
    game.run()