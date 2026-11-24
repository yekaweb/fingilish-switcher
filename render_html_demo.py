import os
import sys
import time
import shutil
import subprocess
from DrissionPage import ChromiumPage, ChromiumOptions
from PIL import Image

BASE_DIR = r"d:\Fingilish"
ASSETS_DIR = os.path.join(BASE_DIR, "assets")
FRAMES_DIR = os.path.join(BASE_DIR, "temp_html_frames")
HTML_PATH = os.path.join(ASSETS_DIR, "demo_interactive.html")

os.makedirs(ASSETS_DIR, exist_ok=True)
if os.path.exists(FRAMES_DIR):
    shutil.rmtree(FRAMES_DIR)
os.makedirs(FRAMES_DIR, exist_ok=True)

# Generate high-fidelity HTML file
html_content = """<!DOCTYPE html>
<html lang="fa" dir="rtl">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=960, height=540">
<title>Fingilish Switcher - Live Demo</title>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Vazirmatn:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;500;600;700&family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet">
<style>
  * {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }
  
  body {
    width: 960px;
    height: 540px;
    overflow: hidden;
    background: #0c0e12;
    background-image: 
      radial-gradient(ellipse 700px 350px at 50% 35%, rgba(14, 165, 233, 0.09), transparent 70%),
      radial-gradient(ellipse 500px 250px at 75% 65%, rgba(16, 185, 129, 0.06), transparent 60%);
    font-family: 'Plus Jakarta Sans', 'Vazirmatn', -apple-system, sans-serif;
    color: #e2e8f0;
    position: relative;
    user-select: none;
  }

  /* Header Branding Bar */
  .header-bar {
    position: absolute;
    top: 20px;
    left: 48px;
    right: 48px;
    height: 38px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    z-index: 10;
  }
  
  .brand-pill {
    display: flex;
    align-items: center;
    gap: 10px;
    background: rgba(30, 41, 59, 0.65);
    border: 1px solid rgba(56, 189, 248, 0.35);
    border-radius: 20px;
    padding: 6px 14px;
    backdrop-filter: blur(10px);
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.35);
  }
  
  .brand-icon {
    width: 22px;
    height: 22px;
    background: linear-gradient(135deg, #0284c7, #06b6d4);
    border-radius: 6px;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 0 10px rgba(6, 182, 212, 0.5);
  }
  
  .brand-icon svg {
    width: 14px;
    height: 14px;
    fill: none;
    stroke: #ffffff;
    stroke-width: 2.2;
    stroke-linecap: round;
    stroke-linejoin: round;
  }
  
  .brand-title {
    font-size: 13.5px;
    font-weight: 700;
    letter-spacing: -0.2px;
    color: #f8fafc;
    direction: ltr;
  }
  
  .brand-tag {
    font-size: 10.5px;
    font-weight: 700;
    background: rgba(56, 189, 248, 0.16);
    color: #38bdf8;
    padding: 2px 8px;
    border-radius: 10px;
    border: 1px solid rgba(56, 189, 248, 0.3);
  }

  .status-badge {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 6px 16px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 600;
    font-family: 'Vazirmatn', sans-serif;
    transition: all 0.25s ease;
  }
  
  .status-badge.warning {
    background: rgba(245, 158, 11, 0.14);
    border: 1px solid rgba(245, 158, 11, 0.4);
    color: #fbbf24;
  }
  
  .status-badge.success {
    background: rgba(16, 185, 129, 0.16);
    border: 1px solid rgba(16, 185, 129, 0.5);
    color: #34d399;
    box-shadow: 0 0 18px rgba(16, 185, 129, 0.25);
  }

  .status-dot {
    width: 7px;
    height: 7px;
    border-radius: 50%;
  }
  
  .status-badge.warning .status-dot {
    background: #f59e0b;
    box-shadow: 0 0 8px #f59e0b;
  }
  
  .status-badge.success .status-dot {
    background: #10b981;
    box-shadow: 0 0 8px #10b981;
  }

  /* Gemini Main Container Card */
  .gemini-container {
    position: absolute;
    top: 72px;
    left: 48px;
    right: 48px;
    height: 350px;
    background: #1e1f20;
    border: 1.5px solid #333538;
    border-radius: 26px;
    box-shadow: 
      0 20px 50px -10px rgba(0, 0, 0, 0.7),
      0 0 1px 1px rgba(255, 255, 255, 0.05);
    padding: 26px 28px 18px 28px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    transition: border-color 0.2s ease, box-shadow 0.2s ease;
  }
  
  .gemini-container.converting {
    border-color: rgba(6, 182, 212, 0.7);
    box-shadow: 0 20px 50px -10px rgba(0, 0, 0, 0.7), 0 0 35px rgba(6, 182, 212, 0.3);
  }

  /* Text Display Area */
  .prompt-content-area {
    position: relative;
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .text-box {
    font-size: 21px;
    line-height: 1.6;
    letter-spacing: -0.2px;
    min-height: 100px;
    word-break: break-word;
    position: relative;
    z-index: 2;
  }
  
  .text-box.en {
    direction: ltr;
    text-align: left;
    font-family: 'JetBrains Mono', monospace;
    color: #f1f5f9;
  }
  
  .text-box.fa {
    direction: rtl;
    text-align: right;
    font-family: 'Vazirmatn', sans-serif;
    font-weight: 600;
    color: #ffffff;
    font-size: 22px;
  }

  /* Selection Highlighting */
  .selection-highlight {
    background: rgba(37, 99, 235, 0.45);
    border-radius: 4px;
    box-shadow: 0 0 0 1px rgba(96, 165, 250, 0.4);
    color: #ffffff;
  }
  
  .selection-highlight.flash-success {
    background: rgba(16, 185, 129, 0.35);
    box-shadow: 0 0 0 1.5px rgba(52, 211, 153, 0.7);
  }

  /* Blinking Cursor */
  .cursor {
    display: inline-block;
    width: 2.2px;
    height: 25px;
    vertical-align: middle;
    margin: 0 2px;
    background-color: #38bdf8;
    box-shadow: 0 0 8px rgba(56, 189, 248, 0.9);
    border-radius: 1px;
  }
  
  .cursor.hidden {
    opacity: 0;
  }
  
  .cursor.fa-cursor {
    background-color: #10b981;
    box-shadow: 0 0 8px rgba(16, 185, 129, 0.9);
  }

  /* Floating Fingilish Switcher Bubble */
  .floating-bubble {
    position: absolute;
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 7px 15px;
    background: rgba(15, 23, 42, 0.95);
    border: 1.5px solid #06b6d4;
    border-radius: 20px;
    color: #f8fafc;
    font-size: 13px;
    font-weight: 600;
    font-family: 'Vazirmatn', sans-serif;
    backdrop-filter: blur(12px);
    box-shadow: 
      0 10px 25px -3px rgba(0, 0, 0, 0.6),
      0 0 20px rgba(6, 182, 212, 0.4);
    z-index: 30;
    pointer-events: none;
    transform: translateY(0);
    transition: all 0.2s ease;
  }
  
  .floating-bubble.pulse {
    transform: scale(1.08);
    border-color: #10b981;
    box-shadow: 0 0 25px rgba(16, 185, 129, 0.6);
  }
  
  .floating-bubble svg {
    width: 16px;
    height: 16px;
    stroke: #38bdf8;
    fill: none;
    stroke-width: 2.2;
    stroke-linecap: round;
    stroke-linejoin: round;
  }

  /* Bottom Toolbar */
  .gemini-toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-top: 14px;
    border-top: 1px solid rgba(255, 255, 255, 0.06);
  }
  
  .toolbar-left {
    display: flex;
    align-items: center;
    gap: 8px;
  }
  
  .model-pill {
    display: flex;
    align-items: center;
    gap: 7px;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.08);
    border-radius: 20px;
    padding: 7px 13px;
    font-size: 13px;
    font-weight: 500;
    color: #c4c7c5;
    direction: ltr;
  }
  
  .model-plus-icon {
    width: 15px;
    height: 15px;
    stroke: #c4c7c5;
    stroke-width: 2;
    fill: none;
  }
  
  .model-chevron {
    width: 12px;
    height: 12px;
    stroke: #8e918f;
    stroke-width: 2;
    fill: none;
  }

  .toolbar-right {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  
  .turbo-btn {
    display: flex;
    align-items: center;
    gap: 5px;
    background: transparent;
    border: none;
    font-size: 13px;
    font-weight: 500;
    color: #c4c7c5;
    padding: 6px 10px;
    border-radius: 16px;
    direction: ltr;
  }
  
  .turbo-icon {
    width: 14px;
    height: 14px;
    stroke: #c4c7c5;
    stroke-width: 2;
    fill: none;
  }
  
  .mic-btn {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: transparent;
    border: none;
  }
  
  .mic-btn svg {
    width: 18px;
    height: 18px;
    stroke: #c4c7c5;
    stroke-width: 2;
    fill: none;
  }
  
  .send-btn {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: #1a73e8;
    display: flex;
    align-items: center;
    justify-content: center;
    border: none;
    box-shadow: 0 4px 12px rgba(26, 115, 232, 0.4);
    transition: background 0.3s;
  }
  
  .send-btn.success {
    background: #10b981;
    box-shadow: 0 4px 12px rgba(16, 185, 129, 0.4);
  }
  
  .send-btn svg {
    width: 18px;
    height: 18px;
    stroke: #ffffff;
    stroke-width: 2.2;
    stroke-linecap: round;
    stroke-linejoin: round;
    fill: none;
  }

  /* Keyboard Shortcut HUD - Bottom Pill */
  .shortcut-hud {
    position: absolute;
    bottom: 22px;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    align-items: center;
    gap: 8px;
    background: rgba(15, 23, 42, 0.96);
    border: 1.5px solid rgba(56, 189, 248, 0.35);
    border-radius: 16px;
    padding: 8px 20px;
    backdrop-filter: blur(12px);
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.7);
    z-index: 25;
    direction: ltr;
    transition: all 0.15s ease;
  }
  
  .shortcut-hud.active {
    border-color: #38bdf8;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.7), 0 0 25px rgba(56, 189, 248, 0.5);
  }
  
  .keycap {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 46px;
    height: 34px;
    padding: 0 10px;
    background: #1e293b;
    border: 1px solid #475569;
    border-bottom: 3px solid #334155;
    border-radius: 8px;
    font-size: 13.5px;
    font-weight: 700;
    color: #f8fafc;
    font-family: 'Plus Jakarta Sans', sans-serif;
    transition: all 0.15s ease;
  }
  
  .shortcut-hud.active .keycap {
    background: #0284c7;
    border-color: #38bdf8;
    border-bottom-width: 1px;
    transform: translateY(2px);
    color: #ffffff;
    box-shadow: 0 0 14px rgba(56, 189, 248, 0.7);
  }
  
  .key-plus {
    font-size: 14px;
    font-weight: 600;
    color: #64748b;
  }
  
  .shortcut-hud.active .key-plus {
    color: #94a3b8;
  }

  /* Realistic OS Mouse Cursor */
  .mouse-cursor {
    position: absolute;
    width: 26px;
    height: 26px;
    z-index: 100;
    pointer-events: none;
    filter: drop-shadow(0 4px 8px rgba(0,0,0,0.6));
    transition: opacity 0.2s ease;
  }
  
  .mouse-cursor svg {
    width: 26px;
    height: 26px;
  }
</style>
</head>
<body>

  <!-- Top Header Bar -->
  <div class="header-bar">
    <div class="brand-pill">
      <div class="brand-icon">
        <svg viewBox="0 0 24 24">
          <path d="M4 17h16M16 13l4 4-4 4M20 7H4M8 11L4 7l4-4"/>
        </svg>
      </div>
      <div class="brand-title">Fingilish Switcher</div>
      <div class="brand-tag">v1.3.0 Industrial</div>
    </div>
    
    <div id="statusBadge" class="status-badge warning">
      <div class="status-dot"></div>
      <span id="statusText">کیبورد روی انگلیسی جا مانده (متن اشتباه)</span>
    </div>
  </div>

  <!-- Main Gemini Prompt Card -->
  <div id="geminiBox" class="gemini-container">
    <div class="prompt-content-area">
      <div id="textBox" class="text-box en">
        <span id="textContent"></span><span id="cursor" class="cursor"></span>
      </div>
      
      <!-- Floating Bubble (⇄ Fingilish Switcher) -->
      <div id="floatingBubble" class="floating-bubble" style="display: none; left: 470px; top: -14px;">
        <svg viewBox="0 0 24 24">
          <path d="M4 17h16M16 13l4 4-4 4M20 7H4M8 11L4 7l4-4"/>
        </svg>
        <span id="bubbleText">تبدیل هوشمند (Ctrl+Shift+F)</span>
      </div>
    </div>

    <!-- Bottom Toolbar -->
    <div class="gemini-toolbar">
      <div class="toolbar-left">
        <div class="model-pill">
          <svg class="model-plus-icon" viewBox="0 0 24 24">
            <line x1="12" y1="5" x2="12" y2="19"></line>
            <line x1="5" y1="12" x2="19" y2="12"></line>
          </svg>
          <span>Gemini 3.8 Flash High</span>
          <svg class="model-chevron" viewBox="0 0 24 24">
            <polyline points="6 9 12 15 18 9"></polyline>
          </svg>
        </div>
      </div>
      
      <div class="toolbar-right">
        <div class="turbo-btn">
          <svg class="turbo-icon" viewBox="0 0 24 24">
            <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon>
          </svg>
          <span>Turbo</span>
          <svg class="model-chevron" viewBox="0 0 24 24" style="width: 10px; height: 10px;">
            <polyline points="6 9 12 15 18 9"></polyline>
          </svg>
        </div>
        
        <div class="mic-btn">
          <svg viewBox="0 0 24 24">
            <path d="M12 1a3 3 0 0 0-3 3v8a3 3 0 0 0 6 0V4a3 3 0 0 0-3-3z"></path>
            <path d="M19 10v2a7 7 0 0 1-14 0v-2"></path>
            <line x1="12" y1="19" x2="12" y2="23"></line>
            <line x1="8" y1="23" x2="16" y2="23"></line>
          </svg>
        </div>
        
        <div id="sendBtn" class="send-btn">
          <svg viewBox="0 0 24 24">
            <line x1="22" y1="2" x2="11" y2="13"></line>
            <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
          </svg>
        </div>
      </div>
    </div>
  </div>

  <!-- Keyboard Shortcut HUD -->
  <div id="shortcutHud" class="shortcut-hud" style="opacity: 0;">
    <span class="keycap">Ctrl</span>
    <span class="key-plus">+</span>
    <span class="keycap">Shift</span>
    <span class="key-plus">+</span>
    <span class="keycap" style="min-width: 38px;">F</span>
  </div>

  <!-- Vector Mouse Cursor -->
  <div id="mouseCursor" class="mouse-cursor" style="opacity: 0; left: 800px; top: 400px;">
    <svg viewBox="0 0 24 24">
      <defs>
        <filter id="shadow" x="-20%" y="-20%" width="150%" height="150%">
          <feDropShadow dx="1" dy="2" stdDeviation="1.5" flood-color="#000000" flood-opacity="0.6"/>
        </filter>
      </defs>
      <path d="M3 3l7 18 3-7 7-3L3 3z" fill="#ffffff" stroke="#0f172a" stroke-width="1.5" stroke-linejoin="round" filter="url(#shadow)"/>
    </svg>
  </div>

<script>
  const WRONG_TEXT = "lk ldohl fvl fi hkjhgdh hc ;[h fgdx f'dvl";
  const CORRECT_TEXT = "من میخام برم به انتالیا از کجا بلیط بگیرم";

  const textBox = document.getElementById('textBox');
  const textContent = document.getElementById('textContent');
  const cursor = document.getElementById('cursor');
  const floatingBubble = document.getElementById('floatingBubble');
  const bubbleText = document.getElementById('bubbleText');
  const statusBadge = document.getElementById('statusBadge');
  const statusText = document.getElementById('statusText');
  const shortcutHud = document.getElementById('shortcutHud');
  const mouseCursor = document.getElementById('mouseCursor');
  const geminiBox = document.getElementById('geminiBox');
  const sendBtn = document.getElementById('sendBtn');

  // Timeline (240 frames @ 30 FPS = 8.0 seconds)
  window.renderStep = function(frame) {
    // 1. Initial State (0-15)
    if (frame < 15) {
      textBox.className = "text-box en";
      cursor.className = "cursor";
      cursor.style.opacity = (frame % 10 < 5) ? "1" : "0";
      textContent.innerHTML = "";
      floatingBubble.style.display = "none";
      shortcutHud.style.opacity = "0";
      shortcutHud.className = "shortcut-hud";
      mouseCursor.style.opacity = "0";
      geminiBox.className = "gemini-container";
      sendBtn.className = "send-btn";
      statusBadge.className = "status-badge warning";
      statusText.innerText = "کیبورد روی انگلیسی جا مانده (متن اشتباه)";
      document.body.style.opacity = "1";
      return;
    }

    // 2. Typing Mistyped String (15-85)
    if (frame >= 15 && frame < 85) {
      textBox.className = "text-box en";
      const progress = (frame - 15) / 70;
      const charCount = Math.min(WRONG_TEXT.length, Math.floor(progress * WRONG_TEXT.length) + 1);
      textContent.innerHTML = WRONG_TEXT.substring(0, charCount);
      cursor.className = "cursor";
      cursor.style.opacity = "1";
      floatingBubble.style.display = "none";
      shortcutHud.style.opacity = "0";
      mouseCursor.style.opacity = "0";
      return;
    }

    // 3. Pause & Observe Mistake (85-105)
    if (frame >= 85 && frame < 105) {
      textContent.innerHTML = WRONG_TEXT;
      cursor.className = "cursor";
      cursor.style.opacity = (frame % 10 < 5) ? "1" : "0";
      floatingBubble.style.display = "none";
      shortcutHud.style.opacity = "0";
      mouseCursor.style.opacity = "0";
      return;
    }

    // 4. Mouse Enters & Moves to Start of Text (105-125)
    if (frame >= 105 && frame < 125) {
      textContent.innerHTML = WRONG_TEXT;
      cursor.className = "cursor hidden";
      floatingBubble.style.display = "none";
      shortcutHud.style.opacity = "0";
      mouseCursor.style.opacity = "1";
      
      const t = (frame - 105) / 20;
      // Start: (750, 420), End: (76, 105) near start of text
      const mx = 750 + (76 - 750) * Math.sin(t * Math.PI / 2);
      const my = 420 + (105 - 420) * Math.sin(t * Math.PI / 2);
      mouseCursor.style.left = mx + "px";
      mouseCursor.style.top = my + "px";
      return;
    }

    // 5. Mouse Drag Selection (125-155)
    if (frame >= 125 && frame < 155) {
      const selRatio = (frame - 125) / 30;
      const selLen = Math.floor(selRatio * WRONG_TEXT.length);
      const selectedPart = WRONG_TEXT.substring(0, selLen);
      const unselectedPart = WRONG_TEXT.substring(selLen);
      textContent.innerHTML = `<span class="selection-highlight">${selectedPart}</span>${unselectedPart}`;
      cursor.className = "cursor hidden";

      mouseCursor.style.opacity = "1";
      // Drag from 76px to ~600px
      const mx = 76 + (600 - 76) * selRatio;
      const my = 105;
      mouseCursor.style.left = mx + "px";
      mouseCursor.style.top = my + "px";
      floatingBubble.style.display = "none";
      shortcutHud.style.opacity = "0";
      return;
    }

    // 6. Selection Complete, Floating Bubble & Shortcut HUD Appear (155-170)
    if (frame >= 155 && frame < 170) {
      textContent.innerHTML = `<span class="selection-highlight">${WRONG_TEXT}</span>`;
      cursor.className = "cursor hidden";
      
      // Floating bubble springs up right above selection
      floatingBubble.style.display = "flex";
      floatingBubble.className = "floating-bubble";
      floatingBubble.style.left = "400px";
      floatingBubble.style.top = "-16px";
      bubbleText.innerText = "تبدیل هوشمند (Ctrl+Shift+F)";

      // Shortcut HUD slides up
      const hudProgress = Math.min(1, (frame - 155) / 10);
      shortcutHud.style.opacity = hudProgress;
      shortcutHud.className = "shortcut-hud";

      mouseCursor.style.opacity = "1";
      mouseCursor.style.left = "600px";
      mouseCursor.style.top = "105px";
      return;
    }

    // 7. Shortcut Pressed! (170-180)
    if (frame >= 170 && frame < 180) {
      textContent.innerHTML = `<span class="selection-highlight">${WRONG_TEXT}</span>`;
      cursor.className = "cursor hidden";
      
      floatingBubble.style.display = "flex";
      floatingBubble.className = "floating-bubble pulse";
      floatingBubble.style.left = "400px";
      floatingBubble.style.top = "-16px";

      shortcutHud.style.opacity = "1";
      shortcutHud.className = "shortcut-hud active";
      geminiBox.className = "gemini-container converting";

      mouseCursor.style.opacity = "1";
      mouseCursor.style.left = "600px";
      mouseCursor.style.top = "105px";
      return;
    }

    // 8. Instant Transformation Magic! (180-188)
    if (frame >= 180 && frame < 188) {
      textBox.className = "text-box fa";
      textContent.innerHTML = `<span class="selection-highlight flash-success">${CORRECT_TEXT}</span>`;
      cursor.className = "cursor fa-cursor hidden";

      floatingBubble.style.display = "flex";
      floatingBubble.className = "floating-bubble";
      floatingBubble.style.borderColor = "#10b981";
      bubbleText.innerText = "✓ به فارسی تبدیل شد";

      statusBadge.className = "status-badge success";
      statusText.innerText = "✓ تبدیل و اصلاح آنی با موفقیت انجام شد";

      shortcutHud.style.opacity = "1";
      shortcutHud.className = "shortcut-hud";
      geminiBox.className = "gemini-container";
      sendBtn.className = "send-btn success";

      mouseCursor.style.opacity = "0.4";
      return;
    }

    // 9. Stable Converted State - Persian Text Displayed (188-232)
    if (frame >= 188 && frame < 232) {
      textBox.className = "text-box fa";
      textContent.innerHTML = CORRECT_TEXT;
      cursor.className = "cursor fa-cursor";
      cursor.style.opacity = (frame % 10 < 5) ? "1" : "0";

      floatingBubble.style.display = "flex";
      floatingBubble.style.borderColor = "#10b981";
      bubbleText.innerText = "✓ به فارسی تبدیل شد";

      statusBadge.className = "status-badge success";
      statusText.innerText = "✓ اصلاح و جایگزینی هوشمند انجام شد";

      const fadeOut = Math.max(0, 1 - (frame - 200) / 20);
      shortcutHud.style.opacity = fadeOut;

      mouseCursor.style.opacity = "0";
      geminiBox.className = "gemini-container";
      sendBtn.className = "send-btn success";
      return;
    }

    // 10. Loop Transition / Reset Fade (232-240)
    if (frame >= 232) {
      const alpha = Math.max(0, 1 - (frame - 232) / 8);
      document.body.style.opacity = alpha;
    } else {
      document.body.style.opacity = "1";
    }
  };

  // For interactive browser preview
  let curFrame = 0;
  let isPlaying = true;
  function loop() {
    if (isPlaying) {
      window.renderStep(curFrame);
      curFrame = (curFrame + 1) % 240;
    }
    requestAnimationFrame(loop);
  }
  if (!window.headlessRunner) {
    loop();
  }
</script>
</body>
</html>
"""

with open(HTML_PATH, "w", encoding="utf-8") as f:
    f.write(html_content)

print(f"Generated HTML template at {HTML_PATH}")

# Launch DrissionPage to capture frames
print("Starting Chromium headless with exact 960x540 viewport...")
co = ChromiumOptions()
co.headless(True)
co.set_argument('--window-size=1200,800')
co.set_argument('--hide-scrollbars')

page = ChromiumPage(co)
page.get(f"file:///{HTML_PATH.replace(os.sep, '/')}")
page.run_js("window.headlessRunner = true;")
page.run_cdp('Emulation.setDeviceMetricsOverride', width=960, height=540, deviceScaleFactor=1, mobile=False)

# Wait for fonts to load
time.sleep(1.5)

TOTAL_FRAMES = 240
print(f"Capturing {TOTAL_FRAMES} frames...")
for f in range(TOTAL_FRAMES):
    page.run_js(f"window.renderStep({f});")
    img_bytes = page.get_screenshot(as_bytes='png')
    
    # Crop to exact (960, 540) if needed
    frame_file = os.path.join(FRAMES_DIR, f"frame_{f:04d}.png")
    with open(frame_file, "wb") as fp:
        fp.write(img_bytes)
    
    # Save key preview frames
    if f in [45, 140, 175, 210]:
        with open(os.path.join(ASSETS_DIR, f"inspect_frame_{f}.png"), "wb") as fp:
            fp.write(img_bytes)
            
    if f % 30 == 0:
        print(f"Captured frame {f}/{TOTAL_FRAMES} ({f/TOTAL_FRAMES*100:.1f}%)")

page.quit()
print("All frames captured! Encoding GIF, MP4, and WebM with FFmpeg...")

gif_path = os.path.join(ASSETS_DIR, "demo.gif")
palette_path = os.path.join(FRAMES_DIR, "palette.png")

# Generate optimized palette
subprocess.run([
    "ffmpeg", "-y",
    "-framerate", "30",
    "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
    "-vf", "palettegen=max_colors=128:stats_mode=diff",
    "-update", "1",
    palette_path
], check=True)

# Encode GIF
subprocess.run([
    "ffmpeg", "-y",
    "-framerate", "30",
    "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
    "-i", palette_path,
    "-lavfi", "paletteuse=dither=bayer:bayer_scale=3:diff_mode=rectangle",
    gif_path
], check=True)

# Encode MP4
mp4_path = os.path.join(ASSETS_DIR, "demo.mp4")
subprocess.run([
    "ffmpeg", "-y",
    "-framerate", "30",
    "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
    "-c:v", "libx264",
    "-pix_fmt", "yuv420p",
    "-crf", "18",
    mp4_path
], check=True)

# Encode WebM
webm_path = os.path.join(ASSETS_DIR, "demo.webm")
subprocess.run([
    "ffmpeg", "-y",
    "-framerate", "30",
    "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
    "-c:v", "libvpx-vp9",
    "-crf", "24",
    "-b:v", "0",
    webm_path
], check=True)

gif_size_kb = os.path.getsize(gif_path) / 1024
mp4_size_kb = os.path.getsize(mp4_path) / 1024
print(f"DONE! GIF: {gif_size_kb:.1f} KB | MP4: {mp4_size_kb:.1f} KB")

# Cleanup temp frames
shutil.rmtree(FRAMES_DIR, ignore_errors=True)
