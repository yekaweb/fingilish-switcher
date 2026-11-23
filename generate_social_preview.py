import os
from PIL import Image, ImageDraw, ImageFont

BASE_DIR = r"d:\Fingilish"
FONT_DIR = os.path.join(BASE_DIR, "src", "FingilishSwitcher", "Resources", "Fonts")
ICON_PATH = os.path.join(BASE_DIR, "src", "FingilishSwitcher", "app_icon_1024.png")
OUT_PATH = os.path.join(BASE_DIR, "assets", "social-preview.png")

os.makedirs(os.path.dirname(OUT_PATH), exist_ok=True)

# Official GitHub Social Preview dimensions: 1280 x 640
W, H = 1280, 640

VAZIR_REG = os.path.join(FONT_DIR, "Vazirmatn-Regular.ttf")
VAZIR_MED = os.path.join(FONT_DIR, "Vazirmatn-Medium.ttf")
VAZIR_BOLD = os.path.join(FONT_DIR, "Vazirmatn-Bold.ttf")
SEGOE_UI = r"C:\Windows\Fonts\segoeui.ttf"
SEGOE_BOLD = r"C:\Windows\Fonts\segoeuib.ttf"

font_title = ImageFont.truetype(SEGOE_BOLD, 44)
font_version = ImageFont.truetype(SEGOE_BOLD, 18)
font_subtitle_fa = ImageFont.truetype(VAZIR_BOLD, 25)
font_card_label = ImageFont.truetype(VAZIR_BOLD, 16)
font_en_sample = ImageFont.truetype(SEGOE_UI, 26)
font_fa_sample = ImageFont.truetype(VAZIR_BOLD, 28)
font_key = ImageFont.truetype(SEGOE_BOLD, 20)
font_footer = ImageFont.truetype(SEGOE_BOLD, 16)

# Colors
BG_DARK = (11, 15, 25)            # #0B0F19
GRID_LINE = (20, 29, 47)          # Subtle technical grid
CARD_BG = (19, 26, 41)            # #131A29
CARD_BORDER = (45, 58, 84)
CYAN = (56, 189, 248)             # #38BDF8
EMERALD = (16, 185, 129)          # #10B981
AMBER = (245, 158, 11)            # #F59E0B
WHITE = (248, 250, 252)
MUTED = (148, 163, 184)

def draw_swap_arrows(draw, cx, cy, color, scale=1.0):
    dx = int(10 * scale)
    dy = int(5 * scale)
    ah = int(4 * scale)
    draw.line([(cx - dx, cy - dy), (cx + dx, cy - dy)], fill=color, width=3)
    draw.line([(cx + dx - ah, cy - dy - ah), (cx + dx, cy - dy), (cx + dx - ah, cy - dy + ah)], fill=color, width=3)
    draw.line([(cx - dx, cy + dy), (cx + dx, cy + dy)], fill=color, width=3)
    draw.line([(cx - dx + ah, cy + dy - ah), (cx - dx, cy + dy), (cx - dx + ah, cy + dy + ah)], fill=color, width=3)

def draw_checkmark(draw, cx, cy, color):
    draw.line([(cx - 7, cy), (cx - 2, cy + 5), (cx + 8, cy - 5)], fill=color, width=3)

def main():
    img = Image.new("RGB", (W, H), BG_DARK)
    draw = ImageDraw.Draw(img)

    # 1. Subtle technical background grid
    for gx in range(0, W, 40):
        draw.line([(gx, 0), (gx, H)], fill=GRID_LINE, width=1)
    for gy in range(0, H, 40):
        draw.line([(0, gy), (W, gy)], fill=GRID_LINE, width=1)

    # Outer frame border inside 32px safe zone
    draw.rounded_rectangle([32, 32, W - 32, H - 32], radius=24, fill=(13, 18, 30), outline=(38, 52, 77), width=2)

    # Top accent glow bar
    draw.rounded_rectangle([420, 32, W - 420, 36], radius=2, fill=CYAN)

    # 2. Header: App Icon + Title + Version Badge
    if os.path.exists(ICON_PATH):
        icon_img = Image.open(ICON_PATH).convert("RGBA").resize((88, 88), Image.Resampling.LANCZOS)
        img.paste(icon_img, (84, 68), icon_img)

    draw.text((194, 92), "FINGILISH SWITCHER", font=font_title, fill=WHITE, anchor="lm")

    # Version pill
    draw.rounded_rectangle([685, 74, 905, 110], radius=18, fill=(14, 45, 71), outline=CYAN, width=2)
    draw.text((795, 92), "v1.3.0 Industrial", font=font_version, fill=CYAN, anchor="mm")

    # Persian subtitle under header
    draw.text(
        (194, 140),
        "سامانه هوشمند تبدیل آنی متون اشتباه تایپ‌شده بین کیبورد فارسی و انگلیسی در ویندوز",
        font=font_subtitle_fa,
        fill=MUTED,
        anchor="lm",
        direction="rtl"
    )

    # 3. Center Before -> After Transformation Showcase
    # Left Card: Before (Mistyped English)
    c1_x1, c1_y1, c1_x2, c1_y2 = 84, 205, 1196, 320
    draw.rounded_rectangle([c1_x1, c1_y1, c1_x2, c1_y2], radius=16, fill=CARD_BG, outline=(85, 60, 25), width=2)

    # Mistyped badge inside top card
    draw.rounded_rectangle([c1_x2 - 270, c1_y1 + 16, c1_x2 - 20, c1_y1 + 48], radius=12, fill=(48, 31, 14), outline=AMBER, width=1)
    draw.text((c1_x2 - 145, c1_y1 + 32), "۱. تایپ اشتباه (کیبورد انگلیسی)", font=font_card_label, fill=(252, 211, 77), anchor="mm", direction="rtl")

    # Selected Blue Text inside top card
    wrong_str = "lk ldohl fvl fi hkjhgdh hc ;[h fgdx f'dvl"
    wbbox = draw.textbbox((0, 0), wrong_str, font=font_en_sample)
    ww = wbbox[2] - wbbox[0]
    draw.rounded_rectangle([c1_x1 + 28, c1_y1 + 48, c1_x1 + 42 + ww, c1_y1 + 94], radius=6, fill=(37, 99, 235))
    draw.text((c1_x1 + 35, c1_y1 + 71), wrong_str, font=font_en_sample, fill=WHITE, anchor="lm")

    # Floating bubble next to selection
    bx, by = c1_x1 + 55 + ww, c1_y1 + 52
    draw.rounded_rectangle([bx, by, bx + 52, by + 38], radius=8, fill=(15, 23, 42), outline=CYAN, width=2)
    draw_swap_arrows(draw, bx + 26, by + 19, CYAN, scale=1.0)

    # Middle Trigger Bar: [ Ctrl ] + [ Shift ] + [ F ]
    mid_y = 362
    keys = [("Ctrl", 88), ("Shift", 96), ("F", 64)]
    cur_x = 465
    for i, (klabel, kw) in enumerate(keys):
        draw.rounded_rectangle([cur_x, mid_y - 22, cur_x + kw, mid_y + 22], radius=9, fill=(14, 116, 144), outline=CYAN, width=2)
        draw.text((cur_x + kw // 2, mid_y), klabel, font=font_key, fill=WHITE, anchor="mm")
        cur_x += kw
        if i < len(keys) - 1:
            draw.text((cur_x + 16, mid_y), "+", font=font_key, fill=CYAN, anchor="mm")
            cur_x += 32

    # Bottom Card: After (Converted Persian RTL)
    c2_x1, c2_y1, c2_x2, c2_y2 = 84, 405, 1196, 520
    draw.rounded_rectangle([c2_x1, c2_y1, c2_x2, c2_y2], radius=16, fill=CARD_BG, outline=EMERALD, width=2)

    # Success badge inside bottom card
    draw.rounded_rectangle([c2_x2 - 270, c2_y1 + 16, c2_x2 - 20, c2_y1 + 48], radius=12, fill=(6, 48, 35), outline=EMERALD, width=1)
    draw_checkmark(draw, c2_x2 - 248, c2_y1 + 32, EMERALD)
    draw.text((c2_x2 - 135, c2_y1 + 32), "۲. تبدیل آنی در همان محل", font=font_card_label, fill=(110, 231, 183), anchor="mm", direction="rtl")

    # Converted Persian Text inside bottom card
    correct_str = "من میخام برم به انتالیا از کجا بلیط بگیرم"
    fbbox = draw.textbbox((0, 0), correct_str, font=font_fa_sample, direction="rtl")
    fw = fbbox[2] - fbbox[0]
    draw.text((c2_x1 + 42, c2_y1 + 71), correct_str, font=font_fa_sample, fill=WHITE, anchor="lm", direction="rtl")
    # Green blinking cursor at the end (left of 'بگیرم')
    draw.line([(c2_x1 + 34, c2_y1 + 52), (c2_x1 + 34, c2_y1 + 90)], fill=EMERALD, width=3)

    # 4. Bottom Feature Pills Footer
    pills = [
        ("Windows 10 / 11 (x64)", CYAN),
        ("Win32 Native Clipboard", EMERALD),
        ("0.0% CPU Standby", CYAN),
        ("Vazirmatn Typography", EMERALD),
        ("MIT Open Source", MUTED),
    ]
    px = 84
    py = 562
    for text, col in pills:
        tb = draw.textbbox((0, 0), text, font=font_footer)
        pw = (tb[2] - tb[0]) + 34
        draw.rounded_rectangle([px, py - 18, px + pw, py + 18], radius=18, fill=(19, 28, 46), outline=col, width=1)
        draw.text((px + pw // 2, py), text, font=font_footer, fill=WHITE, anchor="mm")
        px += pw + 18

    img.save(OUT_PATH, "PNG", optimize=True)
    print(f"Saved social preview to {OUT_PATH} ({os.path.getsize(OUT_PATH) / 1024:.1f} KB)")

if __name__ == "__main__":
    main()
