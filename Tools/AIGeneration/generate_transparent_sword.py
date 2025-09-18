#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
专门生成透明背景飞剑素材的脚本
"""

import requests
import json
import os
import time
from datetime import datetime

# API配置
API_BASE_URL = "http://localhost:8000"
SESSIONID = "4bdbdb72ceda00f6007b6d249c1c6879"

# 输出目录
OUTPUT_DIR = "Assets/GeneratedAssets"

class TransparentSwordGenerator:
    def __init__(self):
        self.session = requests.Session()
        self.session.headers.update({
            "Authorization": f"Bearer {SESSIONID}",
            "Content-Type": "application/json",
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
        })
    
    def generate_image(self, prompt, filename):
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
        
        print(f"🎨 正在生成图片: {filename}")
        print(f"📝 提示词: {prompt}")
        
        try:
            response = self.session.post(url, json=payload, timeout=60)
            response.raise_for_status()
            
            result = response.json()
            print(f"✅ API响应成功")
            
            if 'data' in result and result['data'] and len(result['data']) > 0:
                image_url = result['data'][0]['url']
                return self.download_image(image_url, filename)
            else:
                print(f"❌ 响应中没有图片数据: {result}")
                return False
                
        except requests.exceptions.RequestException as e:
            print(f"❌ API请求失败: {e}")
            return False
        except json.JSONDecodeError as e:
            print(f"❌ 响应解析失败: {e}")
            return False
    
    def download_image(self, url, filename):
        """下载图片"""
        try:
            print(f"📥 正在下载图片...")
            response = requests.get(url, timeout=30)
            response.raise_for_status()
            
            # 确保输出目录存在
            os.makedirs(OUTPUT_DIR, exist_ok=True)
            
            filepath = os.path.join(OUTPUT_DIR, filename)
            with open(filepath, 'wb') as f:
                f.write(response.content)
            
            print(f"✅ 图片已保存: {filepath}")
            return True
            
        except Exception as e:
            print(f"❌ 下载失败: {e}")
            return False

def main():
    generator = TransparentSwordGenerator()
    
    # 优化的透明背景飞剑提示词
    transparent_sword_prompts = [
        {
            "prompt": "ancient chinese flying sword, mystical golden blade, ornate handle with jade, floating in air, transparent background, no background, cutout style, PNG format, isolated object, game asset, side view, detailed metalwork",
            "filename": "flying_sword_transparent_v1.png"
        },
        {
            "prompt": "elegant chinese dao sword, silver blade with blue energy aura, traditional handle, levitating, transparent background, white background, isolated, PNG style, game weapon sprite, horizontal orientation",
            "filename": "flying_sword_transparent_v2.png"
        },
        {
            "prompt": "mystical jian sword, celestial blue blade, golden hilt with dragon motif, floating weapon, clean white background, cutout, PNG format, 2D game asset, profile view, no shadows",
            "filename": "flying_sword_transparent_v3.png"
        }
    ]
    
    print("🗡️  开始生成透明背景飞剑素材")
    print("=" * 50)
    
    success_count = 0
    
    for i, sword_data in enumerate(transparent_sword_prompts, 1):
        print(f"\n📍 生成第 {i}/{len(transparent_sword_prompts)} 张飞剑图片")
        
        if generator.generate_image(sword_data["prompt"], sword_data["filename"]):
            success_count += 1
            print(f"✅ 成功生成: {sword_data['filename']}")
        else:
            print(f"❌ 生成失败: {sword_data['filename']}")
        
        # 添加延迟避免API限制
        if i < len(transparent_sword_prompts):
            print("⏳ 等待3秒...")
            time.sleep(3)
    
    print("\n" + "=" * 50)
    print(f"🎯 生成完成！成功: {success_count}/{len(transparent_sword_prompts)}")
    
    if success_count > 0:
        print(f"📂 素材已保存到: {OUTPUT_DIR}")
        print("🎮 现在可以在Unity中导入这些素材了！")

if __name__ == "__main__":
    main()
