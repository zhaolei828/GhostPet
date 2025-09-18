#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
重新生成所有游戏素材 - 规范文件夹结构
按照正确的分类重新生成所有素材
"""

import requests
import json
import os
import time
from datetime import datetime

# API配置
API_BASE_URL = "http://localhost:8000"
SESSIONID = "4bdbdb72ceda00f6007b6d249c1c6879"

# 基础输出目录
BASE_OUTPUT_DIR = "Assets/GeneratedAssets"

# 规范的文件夹结构
FOLDERS = {
    "characters": f"{BASE_OUTPUT_DIR}/characters",
    "weapons": f"{BASE_OUTPUT_DIR}/weapons", 
    "ui_elements": f"{BASE_OUTPUT_DIR}/ui_elements",
    "effects": f"{BASE_OUTPUT_DIR}/effects"
}

# 完整的游戏素材提示词 - 优化版
GAME_ASSETS = {
    "characters": {
        "player_character": {
            "prompt": "mystical warrior cultivator, ancient chinese robes, floating meditation pose, ethereal aura, controlling flying swords, 2D game character sprite, clean white background, isolated, PNG format, game asset",
            "filename": "player_character.png"
        },
        "basic_ghost": {
            "prompt": "chinese ghost spirit, translucent white appearance, glowing red eyes, tattered traditional robes, menacing floating pose, 2D game enemy sprite, white background, isolated character, PNG format",
            "filename": "basic_ghost.png"
        },
        "strong_ghost": {
            "prompt": "powerful ghost demon, dark smoky aura, fierce red glowing eyes, larger intimidating size, torn black robes, boss enemy sprite, white background, isolated, PNG format, 2D game asset",
            "filename": "strong_ghost.png"
        }
    },
    "weapons": {
        "flying_sword": {
            "prompt": "elegant chinese flying sword, mystical silver blade, ornate golden hilt with jade details, floating horizontally, clean white background, isolated weapon, PNG format, 2D game sprite",
            "filename": "flying_sword.png"
        },
        "sword_energy": {
            "prompt": "sword energy aura, blue mystical energy surrounding blade, glowing effect, transparent energy wisps, white background, isolated effect, PNG format, game weapon enhancement",
            "filename": "sword_energy.png"
        }
    },
    "effects": {
        "sword_trail": {
            "prompt": "sword afterimage trail, blue energy motion blur, fading opacity effect, mystical sword path, white background, isolated effect, PNG format, 2D game trail effect",
            "filename": "sword_trail.png"
        },
        "hit_effect": {
            "prompt": "sword strike impact sparks, golden energy burst, hit collision effect, impact particles, white background, isolated effect, PNG format, 2D game impact animation",
            "filename": "hit_effect.png"
        },
        "enemy_death": {
            "prompt": "ghost dissipation effect, white smoke particles, spirit fading away, death animation effect, white background, isolated effect, PNG format, enemy destruction effect",
            "filename": "enemy_death.png"
        }
    },
    "ui_elements": {
        "health_bar_frame": {
            "prompt": "ancient chinese health bar frame, wooden texture with gold inlay, jade corner decorations, traditional border design, white background, isolated UI element, PNG format",
            "filename": "health_bar_frame.png"
        },
        "score_panel": {
            "prompt": "traditional chinese scroll panel, aged parchment texture, gold decorative borders, corner tassels, score display background, white background, isolated UI, PNG format",
            "filename": "score_panel.png"
        },
        "sword_icon": {
            "prompt": "small sword status icon, minimalist chinese sword symbol, golden color, simple design for UI, white background, isolated icon, PNG format, 64x64 size",
            "filename": "sword_icon.png"
        }
    }
}

class AssetGenerator:
    def __init__(self):
        self.session = requests.Session()
        self.session.headers.update({
            "Authorization": f"Bearer {SESSIONID}",
            "Content-Type": "application/json",
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
        })
    
    def create_folders(self):
        """创建文件夹结构"""
        for folder_name, folder_path in FOLDERS.items():
            os.makedirs(folder_path, exist_ok=True)
            print(f"📁 创建文件夹: {folder_name}")
    
    def generate_image(self, prompt, category, filename):
        """生成单张图片"""
        url = f"{API_BASE_URL}/v1/images/generations"
        
        payload = {
            "model": "jimeng",
            "prompt": prompt,
            "sessionid": SESSIONID,
            "size": "1024x1024",
            "style": "realistic",
            "quality": "standard"
        }
        
        print(f"🎨 正在生成: {category}/{filename}")
        print(f"📝 提示词: {prompt[:80]}...")
        
        try:
            response = self.session.post(url, json=payload, timeout=60)
            response.raise_for_status()
            
            result = response.json()
            
            if 'data' in result and result['data'] and len(result['data']) > 0:
                image_url = result['data'][0]['url']
                return self.download_image(image_url, category, filename)
            else:
                print(f"❌ 响应中没有图片数据: {result}")
                return False
                
        except requests.exceptions.RequestException as e:
            print(f"❌ API请求失败: {e}")
            return False
        except json.JSONDecodeError as e:
            print(f"❌ 响应解析失败: {e}")
            return False
    
    def download_image(self, url, category, filename):
        """下载图片到指定文件夹"""
        try:
            print(f"📥 正在下载...")
            response = requests.get(url, timeout=30)
            response.raise_for_status()
            
            filepath = os.path.join(FOLDERS[category], filename)
            with open(filepath, 'wb') as f:
                f.write(response.content)
            
            print(f"✅ 保存成功: {category}/{filename}")
            return True
            
        except Exception as e:
            print(f"❌ 下载失败: {e}")
            return False

def main():
    generator = AssetGenerator()
    
    print("🎮 GhostPet 游戏素材重新生成")
    print("=" * 60)
    
    # 创建文件夹结构
    print("\n📁 创建文件夹结构...")
    generator.create_folders()
    
    total_assets = 0
    success_count = 0
    
    # 计算总数
    for category in GAME_ASSETS.values():
        total_assets += len(category)
    
    current_asset = 0
    
    # 生成所有素材
    for category_name, assets in GAME_ASSETS.items():
        print(f"\n🎨 生成 {category_name} 类别素材")
        print("-" * 40)
        
        for asset_name, asset_data in assets.items():
            current_asset += 1
            print(f"\n📍 [{current_asset}/{total_assets}] {asset_name}")
            
            if generator.generate_image(
                asset_data["prompt"], 
                category_name, 
                asset_data["filename"]
            ):
                success_count += 1
                print(f"✅ 成功")
            else:
                print(f"❌ 失败")
            
            # 添加延迟避免API限制
            if current_asset < total_assets:
                print("⏳ 等待3秒...")
                time.sleep(3)
    
    print("\n" + "=" * 60)
    print(f"🎯 生成完成！成功: {success_count}/{total_assets}")
    
    # 显示文件夹结构
    print(f"\n📂 素材文件夹结构:")
    for folder_name, folder_path in FOLDERS.items():
        print(f"   {folder_name}/")
        if os.path.exists(folder_path):
            files = [f for f in os.listdir(folder_path) if f.endswith('.png')]
            for file in files:
                print(f"   ├── {file}")
    
    print(f"\n🎮 现在可以在Unity中按分类导入这些素材了！")

if __name__ == "__main__":
    main()
